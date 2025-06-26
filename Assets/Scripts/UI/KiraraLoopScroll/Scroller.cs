using System;
using Kirara.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Scroller : MonoBehaviour,
    IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IScrollHandler, IPointerUpHandler
{
    public RectTransform content;
    public Scrollbar scrollbar;
    public EDirection direction = EDirection.Vertical;
    public float decelerationRate = 0.135f;

    public float elasticDuration = 0.3f;
    public float scrollSensitivity = 0.1f;
    public float scrollDuration = 0.3f;

    public bool isInfinite = false;
    public int totalCount;

    public Snap snap = new()
    {
        enable = false,
        duration = 0.3f,
        speedThreshold = 50f
    };

    private EScrollerState state = EScrollerState.Idle;

    protected virtual float GetSnapPos(float pos) => pos;

    // 滚动空间长度
    protected abstract float SpaceLength { get; }

    // 滚动窗口长度
    protected abstract float ViewLength { get; }

    // 合法空间长度，比如窗口大于滚动空间，合法长度就为0
    protected float ValidSpaceLength => isInfinite ? float.PositiveInfinity : Mathf.Max(0f, SpaceLength - ViewLength);

    // 窗口中的内容长度，因为窗口可能超出空间，超出部分不属于内容长度
    protected float ViewContentLength
    {
        get
        {
            if (isInfinite) return ViewLength;

            float l = Mathf.Max(ViewPos, 0f);
            float r = Mathf.Min(ViewPos + ViewLength, SpaceLength);
            return Mathf.Max(r - l, 0f);
        }
    }

    // 滚动位置
    private float viewPos;
    protected float ViewPos
    {
        get => viewPos;
        set
        {
            if (Mathf.Approximately(value, viewPos)) return;

            SetPos(value, true);
        }
    }

    private float scrollSpeed;
    private Vector2 prevPointerPos;

    private AnimState animState;

    #region Pooling
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
        viewPos = scrollPos;
        if (updateScrollbar && scrollbar)
        {
            scrollbar.SetValueWithoutNotify(scrollPos / ValidSpaceLength);
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

    private void OnScrollBarValueChanged(float x)
    {
        state = EScrollerState.Idle;
        SetPos(x * ValidSpaceLength, false);
    }

    private void UpdateScrollBarSize()
    {
        if (scrollbar)
        {
            scrollbar.size = ViewContentLength / SpaceLength;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        state = EScrollerState.Drag;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(content, eventData.position,
            eventData.pressEventCamera, out prevPointerPos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(content, eventData.position,
            eventData.pressEventCamera, out var pointerPos);
        var delta = pointerPos - prevPointerPos;
        prevPointerPos = pointerPos;

        float offset = CalcDeltaProj(delta);
        scrollSpeed = offset / Time.unscaledDeltaTime;

        // 如果不是无限滚动且释放位置超出范围，滚动到合法位置
        if (!isInfinite && viewPos < 0f)
        {
            ScrollTo(0f, elasticDuration);
        }
        else if (!isInfinite && viewPos > ValidSpaceLength )
        {
            ScrollTo(ValidSpaceLength, elasticDuration);
        }
        else
        {
            // 要么是无限滚动，要么在范围内
            state = EScrollerState.Inertia;
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
        scrollSpeed = deltaProj / Time.deltaTime;

        if (!isInfinite && (viewPos < 0f || viewPos > SpaceLength - ViewLength))
        {
            deltaProj *= 0.25f;
        }

        ViewPos += deltaProj;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        scrollSpeed = 0f;

        state = EScrollerState.Idle;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
    }

    // 鼠标滚轮
    public void OnScroll(PointerEventData eventData)
    {
        float delta = eventData.scrollDelta.y;
        // 滚轮向下为负，向上为正
        // 滚轮向下对应视窗向下，坐标增加
        delta = -delta;
        scrollSpeed = 0f;
        ScrollTo(ViewPos + delta * scrollSensitivity, scrollDuration);
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
            ViewPos = animState.Update(ViewPos, Time.unscaledDeltaTime, out bool isComplete);
            if (isComplete)
            {
                state = EScrollerState.Idle;
            }
        }
        else if (state == EScrollerState.Inertia)
        {
            UpdateInertia();
        }
        UpdateScrollBarSize();
    }

    private void UpdateInertia()
    {
        float k = Mathf.Pow(decelerationRate, Time.unscaledDeltaTime);
        if (!isInfinite && (ViewPos < 0f || ViewPos > ValidSpaceLength))
        {
            // 惯性超出空间，额外减速
            k *= 0.5f;
        }
        scrollSpeed *= k;

        ViewPos += scrollSpeed * Time.unscaledDeltaTime;

        if (isInfinite)
        {
            // 无限滚动
            if (Mathf.Abs(scrollSpeed) < 1f)
            {
                scrollSpeed = 0f;
                state = EScrollerState.Idle;
            }
            if (snap.enable && Mathf.Abs(scrollSpeed) < snap.speedThreshold)
            {
                ScrollTo(GetSnapPos(ViewPos), snap.duration);
            }
        }
        else
        {
            // 非无限滚动
            if (viewPos < 0f)
            {
                if (Mathf.Abs(scrollSpeed) < 1f)
                {
                    // 小于空间且速度很小
                    ScrollTo(0f, elasticDuration);
                }
            }
            else if (viewPos > ValidSpaceLength)
            {
                if (Mathf.Abs(scrollSpeed) < 1f)
                {
                    // 大于空间且速度很小
                    ScrollTo(ValidSpaceLength, elasticDuration);
                }
            }
            else
            {
                // 在空间内
                if (snap.enable && Mathf.Abs(scrollSpeed) < snap.speedThreshold)
                {
                    ScrollTo(GetSnapPos(ViewPos), snap.duration);
                }
            }
        }
    }

    public void ScrollTo(float pos, float duration, Action onComplete = null)
    {
        scrollSpeed = 0f;
        state = EScrollerState.Anim;
        animState.Set(ViewPos, pos, duration, onComplete);
    }
}