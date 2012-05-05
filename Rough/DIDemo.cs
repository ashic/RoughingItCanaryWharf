using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rough
{
    public interface IPrint
    {
        void Print(string message);
    }

    public interface ISayHello
    {
        void SayHello(string name);
    }

    public class DIDemo : ISayHello
    {
        readonly IPrint _printer;

        public DIDemo(IPrint printer)
        {
            _printer = printer;
        }

        public void SayHello(string name)
        {
            _printer.Print(string.Format("Hello {0}", name));
        }
    }

    public class ConsolePrinter : IPrint
    {
        public void Print(string message)
        {
            Console.WriteLine(message);
        }
    }

    //.................................................
    
    public interface Clock
    {
        DateTime GetTime();
    }

    public class ConsolePrinterWithTime : IPrint
    {
        readonly Clock _clock;
        readonly IPrint _printer;

        public ConsolePrinterWithTime(Clock clock, IPrint printer)
        {
            _clock = clock;
            _printer = printer;
        }

        public void Print(string message)
        {
            _printer.Print(string.Format("{0}, at {1}", message, _clock.GetTime()));
        }
    }

    public class UtcClock : Clock
    {
        public DateTime GetTime()
        {
            return DateTime.UtcNow;
        }
    }

    //............................
    public class Factory
    {
        readonly Dictionary<Type, Func<object>> factories = new Dictionary<Type, Func<object>>(); 

        public T Resolve<T>()
        {
            var type = typeof (T);
            return (T) factories[type]();
        }

        public void RegisterSingleton<T>(Func<T> func)
        {
            var singleInstance = func();
            factories[typeof(T)] = () => singleInstance;
        }

        public void RegisterTransient<T>(Func<T> func)
        {
            factories[typeof(T)] = ()=> func();
        }
    }
}
