using System;

namespace Kirara.TimelineAction
{
    /// <summary>
    /// 数组实现的简易双端队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Deque<T>
    {
        public int Count { get; private set; }
        private int _front; // 空或填满时，_front == _back
        private int _back;
        private T[] _data;

        #region 构造函数

        public Deque()
        {
            _data = Array.Empty<T>();
        }

        public Deque(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "MustBeNonNegative");
            }
            _data = new T[capacity];
        }

        #endregion

        #region 访问操作

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }
                return _data[(index + _front) % _data.Length];
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }
                _data[(index + _front) % _data.Length] = value;
            }
        }

        public T Front
        {
            get
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException("Deque is empty.");
                }
                return _data[_front];
            }
            set
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException("Deque is empty.");
                }
                _data[_front] = value;
            }
        }

        public T Back
        {
            get
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException("Deque is empty.");
                }
                return _data[GetPrev(_back)];
            }
            set
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException("Deque is empty.");
                }
                _data[GetPrev(_back)] = value;
            }
        }

        public int IndexOf(T item)
        {
            if (Count == 0) return -1;

            if (_front < _back)
            {
                // 连续
                int index = Array.IndexOf(_data, item, _front, _back - _front);
                return index >= 0 ? index - _front : -1;
            }
            else
            {
                // 不连续，检查前半段
                int index = Array.IndexOf(_data, item, _front, _data.Length - _front);
                if (index >= 0) return index - _front;

                // 检查后半段
                index = Array.IndexOf(_data, item, 0, _back);
                return index >= 0 ? index + _data.Length - _front : -1;
            }
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public bool TryPeekFront(out T item)
        {
            if (Count > 0)
            {
                item = _data[_front];
                return true;
            }
            item = default;
            return false;
        }

        public bool TryPeekBack(out T item)
        {
            if (Count > 0)
            {
                item = _data[GetPrev(_back)];
                return true;
            }
            item = default;
            return false;
        }

        #endregion

        #region 修改操作

        public void Clear()
        {
            _front = 0;
            _back = 0;
            Count = 0;
        }

        public void PushFront(T item)
        {
            EnsureCapacity(Count + 1);
            _front = GetPrev(_front);
            Count++;
            _data[_front] = item;
        }

        public void PushBack(T item)
        {
            EnsureCapacity(Count + 1);
            _data[_back] = item;
            _back = GetNext(_back);
            Count++;
        }

        public T PopFront()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Deque is empty.");
            }
            var item = _data[_front];
            _data[_front] = default;
            _front = GetNext(_front);
            Count--;
            return item;
        }

        public T PopBack()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Deque is empty.");
            }
            _back = GetPrev(_back);
            var item = _data[_back];
            _data[_back] = default;
            Count--;
            return item;
        }

        public bool TryPopFront(out T item)
        {
            if (Count > 0)
            {
                item = _data[_front];
                _data[_front] = default;
                _front = GetNext(_front);
                Count--;
                return true;
            }
            item = default;
            return false;
        }

        public bool TryPopBack(out T item)
        {
            if (Count > 0)
            {
                _back = GetPrev(_back);
                item = _data[_back];
                _data[_back] = default;
                Count--;
                return true;
            }
            item = default;
            return false;
        }

        #endregion

        private int GetPrev(int index)
        {
            return (index - 1 + _data.Length) % _data.Length;
        }

        private int GetNext(int index)
        {
            return (index + 1) % _data.Length;
        }

        private void EnsureCapacity(int capacity)
        {
            if (capacity > _data.Length)
            {
                var newData = new T[Math.Max(capacity, _data.Length * 2)];
                for (int i = 0; i < Count; i++)
                {
                    newData[i] = _data[(i + _front) % _data.Length];
                }
                _front = 0;
                _back = Count;
                _data = newData;
            }
        }
    }
}