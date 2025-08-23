using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace KiraraLoopScroll
{
    public abstract class Scroller : MonoBehaviour,
        IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IScrollHandler, IPointerUpHandler
    {
        // 内容
        [FormerlySerializedAs("content")] public RectTransform viewport;

        // 滚动条
        public Scrollbar scrollbar;

        /// <summary>
        /// 滚动方向
        /// </summary>
        public EDirection direction = EDirection.Vertical;

        public bool isClamped = false;

        // 一阶粘性阻尼
        // 阻尼率
        public float dampingRatio = 7f;

        /// <summary>
        /// 回弹时长，用于鼠标释放时超出范围，回弹到边界
        /// </summary>
        public float elasticDuration = 0.3f;

        /// <summary>
        /// 鼠标滚轮灵敏度，用于控制滚动距离
        /// </summary>
        public float wheelSensitivity = 0.1f;

        /// <summary>
        /// 鼠标滚轮滚动时长
        /// </summary>
        public float wheelScrollDuration = 0.3f;

        /// <summary>
        /// 是否为无限滚动
        /// </summary>
        public bool isInfinite;

        // 对齐功能
        public bool enableSnap;

        #region 抽象方法区

        protected abstract float PosToEdge { get; }

        /// <summary>
        /// 存放Item的内容区域大小，如果大小不易获得，请返回-1
        /// </summary>
        protected abstract float ContentSize { get; }

        /// <summary>
        /// 更新Items
        /// </summary>
        protected abstract void UpdateItems();

        #endregion

        /// <summary>
        /// 视口大小
        /// </summary>
        protected float ViewSize
        {
            get
            {
                return direction switch
                {
                    EDirection.Vertical => viewport.rect.height,
                    EDirection.Horizontal => viewport.rect.width,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        // 状态
        private EScrollerState state = EScrollerState.Idle;

        /// <summary>
        /// 获取对齐后位置，默认实现为不变
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected virtual float GetSnapPos(float pos) => pos;

        // Idle状态下，Pos可以在的范围
        protected ScrollRange ValidRange => (isInfinite || ContentSize < 0)
            ? ScrollRange.Infinity
            : new ScrollRange(0f, Mathf.Max(0f, ContentSize - ViewSize));

        // 视口中的内容大小，因为窗口可能超出内容，比如滑到了边缘外，视口有部分没有内容
        protected float ContentSizeInView
        {
            get
            {
                if (isInfinite) return ViewSize;

                if (ContentSize < 0f) return 0f;

                float l = Mathf.Max(Pos, 0f);
                float r = Mathf.Min(Pos + ViewSize, ContentSize);
                return Mathf.Max(r - l, 0f);
            }
        }

        private float _pos;

        /// <summary>
        /// 滚动位置
        /// </summary>
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

        #region 获取和返还

        public int _totalCount;

        public delegate GameObject GetObject(int index);

        public delegate void ReturnObject(GameObject go);

        public delegate void ProvideData(GameObject go, int index);

        public GetObject getObject;
        public ReturnObject returnObject;
        public ProvideData provideData;

        public void SetSourceFunc(GetObject getObject, ReturnObject returnObject)
        {
            this.getObject = getObject;
            this.returnObject = returnObject;
        }

        public void SetSource(IGameObjectSource source)
        {
            SetSourceFunc(source.GetObject, source.ReturnObject);
        }

        #endregion

        protected void SetPos(float scrollPos, bool updateScrollbar)
        {
            _pos = scrollPos;
            if (updateScrollbar && scrollbar && ContentSize >= 0f)
            {
                // 值为0时上方对齐顶部，值为1时下方对齐底部
                float input;
                if (ContentSize <= ViewSize)
                {
                    input = PosToEdge >= 0f ? 0f : 1f;
                }
                else
                {
                    input = scrollPos / ValidRange.Length;
                }
                scrollbar.SetValueWithoutNotify(input);
            }
            UpdateItems();
        }

        protected virtual void Awake()
        {
            if (!viewport)
            {
                viewport = (RectTransform)transform;
            }
        }

        protected virtual void Start()
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
            if (scrollbar && ContentSize >= 0)
            {
                // 滚动条的手柄长 = 可见内容大小 / 内容总大小
                scrollbar.size = ContentSizeInView / ContentSize;
            }
        }

        // 拖拽开始
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            state = EScrollerState.Drag;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, eventData.position,
                eventData.pressEventCamera, out prevPointerPos);
        }

        // 拖拽结束
        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, eventData.position,
                eventData.pressEventCamera, out var pointerPos);
            var delta = pointerPos - prevPointerPos;
            prevPointerPos = pointerPos;

            float offset = CalcDeltaProj(delta);
            scrollVelocity = offset / Time.unscaledDeltaTime;

            // 如果释放位置超出合法范围，滚动到合法边界
            if (!isInfinite && !Mathf.Approximately(PosToEdge, 0f))
            {
                ScrollTo(Pos + PosToEdge, elasticDuration);
            }
            else
            {
                // 释放位置在范围内
                if (enableSnap)
                {
                    // 计算停止点
                    AnimState.CalcInertiaEndPos(Pos, out float endPos, scrollVelocity, dampingRatio);
                    if (ValidRange.Contains(endPos))
                    {
                        // 对齐停止点
                        endPos = GetSnapPos(endPos);
                        state = EScrollerState.Inertia;
                        animState.SetInertia(Pos, endPos, scrollVelocity, dampingRatio);
                    }
                    else
                    {
                        // 停止点超出范围，不对齐
                        state = EScrollerState.Inertia;
                        animState.SetInertiaV0DampingRatio(Pos, scrollVelocity, dampingRatio);
                    }
                }
                else
                {
                    state = EScrollerState.Inertia;
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

            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, eventData.position,
                eventData.pressEventCamera, out var pointerPos);
            var delta = pointerPos - prevPointerPos;
            prevPointerPos = pointerPos;

            float deltaProj = CalcDeltaProj(delta);
            scrollVelocity = deltaProj / Time.deltaTime;

            if (!isInfinite && !Mathf.Approximately(PosToEdge, 0f))
            {
                if (isClamped)
                {
                    Pos += PosToEdge;
                }
                else
                {
                    float k = 0.25f;
                    Pos += k * deltaProj;
                }
            }
            else
            {
                Pos += deltaProj;
            }
        }

        // 鼠标按下
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            // 鼠标按下，强制停止，保证跟手
            scrollVelocity = 0f;
            state = EScrollerState.Idle;
        }

        // 鼠标抬起
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

        protected virtual void Update()
        {
            if (state == EScrollerState.Inertia)
            {
                if (!animState.IsComplete)
                {
                    Pos = animState.Update(Time.unscaledDeltaTime);
                    if (!Mathf.Approximately(PosToEdge, 0f))
                    {
                        // 动画从边缘滑出
                        animState._outOfContent = true;
                        animState.Kill();
                        Pos += PosToEdge;
                        state = EScrollerState.Idle;
                    }
                }
                else
                {
                    state = EScrollerState.Idle;
                }
            }
            else if (state == EScrollerState.Anim)
            {
                if (!animState.IsComplete)
                {
                    Pos = animState.Update(Time.unscaledDeltaTime);
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

        public void Scroll(float delta, float duration, Action onComplete = null)
        {
            ScrollTo(Pos + delta, duration, onComplete);
        }
    }
}