using System;

namespace Kuci.Core.ReactiveProperty
{ 
    public interface IReactiveProperty<out T>
    {
        T Value { get; }
        void Subscribe(Action<T> subscriber, bool triggerUponSubscription = true);
        void Unsubscribe(Action<T> subscriber);
    }
}
