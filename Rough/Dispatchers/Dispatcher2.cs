using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rough
{
    public class Dispatcher2
    {
        readonly Dictionary<Type, List<Action<object>>> _handlers = new Dictionary<Type, List<Action<object>>>();

        public Dispatcher2 Register<T>(Action<T> handler)
        {
            var type = typeof (T);
            var list = getList(type);
            list.Add(x => handler((T) x));

            return this;
        }

        List<Action<object>> getList(Type type)
        {
            if(_handlers.ContainsKey(type) == false)
                _handlers[type] = new List<Action<object>>();

            return _handlers[type];
        }

        public void Dispatch(object message)
        {
            var type = message.GetType();
            ensureHandlerExists(type);
            foreach (var action in _handlers[type])
                action(message);
        }

        void ensureHandlerExists(Type type)
        {
            if (_handlers.ContainsKey(type) == false)
                throw new ArgumentException(string.Format("There is no handler registered for the message type {0}", type));
        }
    }
}
