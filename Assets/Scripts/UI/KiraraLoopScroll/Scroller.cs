﻿using System;
using Kirara.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Scroller : MonoBehaviour,
    IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IScrollHandler, IPointerUpHandler
{
    // 内容
    public RectTransform content;

    // 滚动条
    public Scrollbar scrollbar;

    // 滚动方向
    public EDirection direction = EDirection.Vertical;

    // 一阶粘性阻尼
    // 阻尼率
    public float dampingRatio = 7f;

    // 回弹时长，用于鼠标释放时超出范围，回弹到边界
    public float elasticDuration = 0.3f;

    // 鼠标滚轮灵敏度
    public float wheelSensitivity = 0.1f;

    // 鼠标滚轮滚动时长
    public float wheelScrollDuration = 0.3f;

    // 是否为无限滚动
    public bool isInfinite;

    // 对齐功能
    public Snap snap = new()
    {
        enable = false,
        duration = 0.3f,
        speedThreshold = 50f
    };

    // 状态
    private EScrollerState state = EScrollerState.Idle;

    protected virtual float GetSnapPos(float pos) => pos;

    // 内容长度
    protected abstract float ContentLength { get; }

    // 窗口长度
    protected abstract float WindowLength { get; }

    // Idle状态下，Pos可以在的范围
    protected ScrollRange ValidRange => isInfinite ?
        ScrollRange.Infinity : new ScrollRange(0f, Mathf.Max(0f, ContentLength - WindowLength));

    // 窗口中的内容长度，因为窗口可能超出内容，比如滑到了边缘外，窗口有部分没有内容
    protected float ContentLengthInWindow
    {
        get
        {
            if (isInfinite) return WindowLength;

            float l = Mathf.Max(Pos, 0f);
            float r = Mathf.Min(Pos + WindowLength, ContentLength);
            return Mathf.Max(r - l, 0f);
        }
    }

    // 滚动位置
    private float _pos;
    protected float Pos
    {
        get => _pos;
        set
        {
            if (Mathf.Approximately(value, _pos)) return;

            SetPos(value, true);
        }
    }

    private float scrollVelocity;
    private Vector2 prevPointerPos;

    private readonly AnimState animState = new();

    #region Pooling
    public int totalCount;
    public delegate GameObject GetObject(int idx);
    public delegate void ReturnObject(GameObject go);
    public delegate void ProvideData(GameObject go, int idx);

    public GetObject getObject;
    public ReturnObject returnObject;
    public ProvideData provideData;

    public void SetPoolFunc(GetObject getObject, ReturnObject returnObject)
    {
        this.getObject = getObject;
        this.returnObject = returnObject;
    }
    #endregion


    protected abstract void CullCells();
    protected abstract void UpdateCellsPos();

    private void SetPos(float scrollPos, bool updateScrollbar)
    {
        _pos = scrollPos;
        if (updateScrollbar && scrollbar)
        {
            scrollbar.SetValueWithoutNotify(scrollPos / ValidRange.Length);
        }
        CullCells();
        UpdateCellsPos();
    }

    private void Awake()
    {
        if (!content)
        {
            content = (RectTransform)transform;
        }
    }

    private void Start()
    {
        if (scrollbar)
        {
            scrollbar.onValueChanged.AddListener(OnScrollBarValueChanged);
        }
        SetPos(0f, true);
    }

    // 滚动条返回的值是0-1
    private void OnScrollBarValueChanged(float x)
    {
        state = EScrollerState.Idle;
        SetPos(x * ValidRange.Length, false);
    }

    private void UpdateScrollBarSize()
    {
        if (scrollbar)
        {
            // 滚动条的手柄长 = 可见内容长度 / 内容总长度
            scrollbar.size = ContentLengthInWindow / ContentLength;
        }
    }

    // 拖拽开始
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        state = EScrollerState.Drag;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(content, eventData.position,
            eventData.pressEventCamera, out prevPointerPos);
    }

    // 拖拽结束
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(content, eventData.position,
            eventData.pressEventCamera, out var pointerPos);
        var delta = pointerPos - prevPointerPos;
        prevPointerPos = pointerPos;

        float offset = CalcDeltaProj(delta);
        scrollVelocity = offset / Time.unscaledDeltaTime;

        // 如果释放位置超出合法范围，滚动到合法边界
        if (!ValidRange.Contains(Pos))
        {
            ScrollTo(ValidRange.GetNearEdge(Pos), elasticDuration);
        }
        else
        {
            // 释放位置在范围内
            if (snap.enable)
            {
                // 计算停止点
                AnimState.CalcInertiaEndPos(Pos, out float endPos, scrollVelocity, dampingRatio);
                if (ValidRange.Contains(endPos))
                {
                    // 对齐停止点
                    endPos = GetSnapPos(endPos);
                    state = EScrollerState.Anim;
                    animState.SetInertia(Pos, endPos, scrollVelocity, dampingRatio);
                }
                else
                {
                    // 停止点超出范围，不对齐
                    state = EScrollerState.Anim;
                    animState.SetInertiaV0DampingRatio(Pos, scrollVelocity, dampingRatio);
                }
            }
            else
            {
                state = EScrollerState.Anim;
                animState.SetInertiaV0DampingRatio(Pos, scrollVelocity, dampingRatio);
            }
        }
    }

    private void CheckSetElastic()
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(content, eventData.position,
            eventData.pressEventCamera, out var pointerPos);
        var delta = pointerPos - prevPointerPos;
        prevPointerPos = pointerPos;

        float deltaProj = CalcDeltaProj(delta);
        scrollVelocity = deltaProj / Time.deltaTime;

        if (!isInfinite && (_pos < 0f || _pos > ContentLength - WindowLength))
        {
            deltaProj *= 0.25f;
        }

        Pos += deltaProj;
    }

    // 鼠标按下，强制停止
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        scrollVelocity = 0f;
        state = EScrollerState.Idle;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
    }

    // 鼠标滚轮滚动
    public void OnScroll(PointerEventData eventData)
    {
        float delta = eventData.scrollDelta.y;
        // 滚轮向下为负，向上为正
        // 滚轮向下对应视窗向下，坐标增加
        delta = -delta;
        scrollVelocity = 0f;
        ScrollTo(Pos + delta * wheelSensitivity, wheelScrollDuration);
    }

    private float CalcDeltaProj(Vector2 delta)
    {
        // 向上向右划delta为正
        // 竖向向上划pos增加
        // 横向向右划pos减少
        return direction switch
        {
            EDirection.Vertical => delta.y,
            EDirection.Horizontal => -delta.x,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void Update()
    {
        if (state == EScrollerState.Anim)
        {
            if (!animState.IsComplete)
            {
                float pos1 = Pos;
                Pos = animState.Update(Time.unscaledDeltaTime);
                if (ValidRange.Contains(pos1) && !ValidRange.Contains(Pos))
                {
                    // 动画从边缘滑出
                    animState.Kill();
                    Pos = ValidRange.GetNearEdge(Pos);
                    state = EScrollerState.Idle;
                }
            }
            else
            {
                state = EScrollerState.Idle;
            }
        }
        UpdateScrollBarSize();
    }

    // private void UpdateInertia()
    // {
    //     float k = Mathf.Pow(decelerationRate, Time.unscaledDeltaTime);
    //     if (!ValidRange.Contains(Pos))
    //     {
    //         // 惯性超出空间，额外减速
    //         k *= 0.5f;
    //     }
    //     scrollVelocity *= k;
    //
    //     Pos += scrollVelocity * Time.unscaledDeltaTime;
    //
    //     if (isInfinite)
    //     {
    //         // 无限滚动
    //         if (Mathf.Abs(scrollVelocity) < 1f)
    //         {
    //             scrollVelocity = 0f;
    //             state = EScrollerState.Idle;
    //         }
    //         if (snap.enable && Mathf.Abs(scrollVelocity) < snap.speedThreshold)
    //         {
    //             ScrollTo(GetSnapPos(Pos), snap.duration);
    //         }
    //     }
    //     else
    //     {
    //         // 非无限滚动
    //         if (!ValidRange.Contains(Pos) && Mathf.Abs(scrollVelocity) < 1f)
    //         {
    //             ScrollTo(ValidRange.GetNearEdge(Pos), elasticDuration);
    //         }
    //         else
    //         {
    //             // 在空间内
    //             if (snap.enable && Mathf.Abs(scrollVelocity) < snap.speedThreshold)
    //             {
    //                 ScrollTo(GetSnapPos(Pos), snap.duration);
    //             }
    //         }
    //     }
    // }

    public void ScrollTo(float pos, float duration, Action onComplete = null)
    {
        state = EScrollerState.Anim;
        scrollVelocity = 0f;
        animState.Set(Pos, pos, duration, onComplete);
    }
}