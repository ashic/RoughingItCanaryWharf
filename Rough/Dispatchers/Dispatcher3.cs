using System;
using System.Collections.Generic;
using System.Linq;

namespace Rough
{
    public class Dispatcher3
    {
        readonly Dictionary<Type, List<Action<object>>> _handlers = new Dictionary<Type, List<Action<object>>>();

        public Dispatcher3 Register<T>(Action<T> handler)
        {
            var type = typeof(T);
            var list = getList(type);
            list.Add(x => handler((T)x));

            return this;
        }

        List<Action<object>> getList(Type type)
        {
            if (_handlers.ContainsKey(type) == false)
                _handlers[type] = new List<Action<object>>();

            return _handlers[type];
        }

        public void Dispatch(object message)
        {
            var type = message.GetType();
            ensureHandlerExists(type);

            var keys = _handlers.Keys.Where(x=>x.IsAssignableFrom(type));

            foreach (var key in keys)
                foreach (var action in _handlers[key])
                    action(message);
        }

        void ensureHandlerExists(Type type)
        {
            if (_handlers.ContainsKey(type) == false)
                throw new ArgumentException(string.Format("There is no handler registered for the message type {0}", type));
        }
    }
}