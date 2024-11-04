using System;
using System.Collections.Generic;
using System.Linq;
using Kuci.Logger;

namespace Kuci.Core.ReactiveProperty
{
    public class ReactiveProperty<T> : IReactiveProperty<T>
    {
        private T _value;
        private event Action<T> OnValueChanged;

        public ReactiveProperty()
        {
            Value = default;
        }
        
        public ReactiveProperty(T initialValue)
        {
            _value = initialValue;
        }

        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value))
                {
                    return;
                }
                
                _value = value;
                OnValueChanged?.Invoke(value);
            }
        }

        void IReactiveProperty<T>.Subscribe(Action<T> subscriber, bool triggerUponSubscription)
        {
            if (OnValueChanged != null && OnValueChanged.GetInvocationList().Contains(subscriber))
            {
                DevLogger.LogWarning("Can not subscribe same subscriber twice");
                return;
            }
            
            OnValueChanged += subscriber;
            
            if (triggerUponSubscription)
            {
                subscriber?.Invoke(_value);
            }
        }

        void IReactiveProperty<T>.Unsubscribe(Action<T> subscriber)
        {
            OnValueChanged -= subscriber;
        }
    }
}