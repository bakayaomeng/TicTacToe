using Framework.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Signal
{
    public class SignalBase
    {
        protected object _handler;

        public Delegate Func => _handler as Delegate;
    }

    public class Signal<T> : SignalBase
    {
        public T Handler
        {
            get
            {
                if (_handler != null)
                {
                    return (T)_handler;
                }

                return default(T);
            }
        }

        public void Subscribe(T listener)
        {
            var func = listener as Delegate;
            if (func != null)
            {
                var handler = _handler as Delegate;
                if (handler == null)
                {
                    _handler = listener;
                }
                else
                {
                    var invocations = handler.GetInvocationList();
                    if (!invocations.ArrayContains(func))
                    {
                        _handler = Delegate.Combine(handler, func);
                    }
                }
            }
        }

        public void Unsubscribe(T listener)
        {
            var func = listener as Delegate;
            if (func != null)
            {
                var handler = _handler as Delegate;
                if (handler != null)
                {
                    _handler = Delegate.RemoveAll(handler, func);
                }
            }
        }
    }

    public class SignalManager : IDisposable
    {
        private Dictionary<string, SignalBase> _signalMap = new Dictionary<string, SignalBase>();
        private Action<string, object[]> _handlers;

        public void Dispose()
        {
            _signalMap.Clear();
            _handlers = null;
        }

        public void Subscribe<T>(T listener)
        {
            Get<T>().Subscribe(listener);
        }

        public void Unsubscribe<T>(T listener)
        {
            Get<T>().Unsubscribe(listener);
        }

        public T Find<T>()
        {
            return Get<T>().Handler;
        }

        private Signal<T> Get<T>()
        {
            var name = typeof(T).Name;
            if (!_signalMap.TryGetValue(name, out SignalBase wrapper))
            {
                var ret = new Signal<T>();
                _signalMap[name] = ret;
                return ret;
            }

            return (Signal<T>)wrapper;
        }
    }
}