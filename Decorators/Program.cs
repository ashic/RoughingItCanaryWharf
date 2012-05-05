using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rough;

namespace Decorators
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.Run1();
            p.Run2();
        }

        void Run1()
        {
            var factory = new Factory();
            factory.RegisterTransient(getHello);

            //-------------
            var hello = factory.Resolve<ISayHello>();
            hello.SayHello("Jim");
        }

        ISayHello getHello()
        {
            var hello = new LoggingHello(new DIDemo(new ConsolePrinterWithTime(new UtcClock(), new ConsolePrinter())));
            return hello;
        }

        void Run2()
        {
            var factory = new Factory();
            factory.RegisterTransient(getHello2);

            //-------------
            var hello = factory.Resolve<ISayHello>();
            hello.SayHello("Jim");
        }

        ISayHello getHello2()
        {
            var hello = new SecurityEnabledHello(new LoggingHello(new DIDemo(new ConsolePrinterWithTime(new UtcClock(), new ConsolePrinter()))));
            return hello;
        }
    }

    public class LoggingHello : ISayHello
    {
        readonly ISayHello _hello;

        public LoggingHello(ISayHello hello)
        {
            _hello = hello;
        }

        public void SayHello(string name)
        {
            Logger.Log(name);
            _hello.SayHello(name);
        }
    }

    public class Logger
    {
        public static void Log(string message)
        {
            Console.WriteLine("-----Log Entry: {0}", message);
        }
    }

    //.........................................................

    public class SecurityEnabledHello : ISayHello
    {
        readonly ISayHello _hello;

        public SecurityEnabledHello(ISayHello hello)
        {
            _hello = hello;
        }

        public void SayHello(string name)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("I've checked the user for security");
            Console.ForegroundColor = prev;
            _hello.SayHello(name);
        }
    }
}
