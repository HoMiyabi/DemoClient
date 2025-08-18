using System;

namespace Kirara
{
    public class LiveData<T>
    {
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value)) return;

                _value = value;
                OnValueChanged?.Invoke(value);
            }
        }

        public event Action<T> OnValueChanged;

        public LiveData(T value)
        {
            _value = value;
        }

        public void Add(Action<T> callback)
        {
            OnValueChanged += callback;
        }

        public void Remove(Action<T> callback)
        {
            OnValueChanged -= callback;
        }

        public void Observe(Action<T> callback)
        {
            if (callback == null) return;
            OnValueChanged += callback;
            callback(_value);
        }

        public static implicit operator T(LiveData<T> liveData)
        {
            return liveData._value;
        }

        public void Set(T value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}