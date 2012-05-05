using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rough
{
    public class Dispatcher1
    {
        readonly Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

        public Dispatcher1 Register<T>(Action<T> handler)
        {
            _handlers[typeof (T)] = x => handler((T) x);
            return this;
        }

        public void Dispatch(object message)
        {
            var type = message.GetType();
            ensureHandlerExists(type);
            _handlers[type](message);
        }

        void ensureHandlerExists(Type type)
        {
            if (_handlers.ContainsKey(type) == false)
                throw new ArgumentException(string.Format("There is no handler registered for the message type {0}", type));
        }
    }
}
