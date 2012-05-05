using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rough;

namespace Decorators2
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.Run1();
        }

        void Run1()
        {
            var dispatcher = new Dispatcher1();
            dispatcher.Register(
                WrapWithSecurity(
                WrapWithLogger<SayHelloCommand>(sayHelloHandler)));

            dispatcher.Dispatch(new SayHelloCommand("Tony"));
        }

        void sayHelloHandler(SayHelloCommand command)
        {
            new GreetingsExecutors().Handle(command, new ConsolePrinterWithTime(new UtcClock(), new ConsolePrinter()));
        }

        static Action<T> WrapWithLogger<T>(Action<T> inner)
        {
            return new LoggingExecutor<T>(inner).Handle;
        }

        static Action<T> WrapWithSecurity<T>(Action<T> inner)
        {
            return new SecurityEnabledExecutor<T>(inner).Handle;
        }
    }

    public class SayHelloCommand
    {
        public string Name { get; private set; }

        public SayHelloCommand(string name)
        {
            Name = name;
        }
    }

    public class GreetingsExecutors
    {
        public void Handle(SayHelloCommand command, IPrint printer)
        {
            printer.Print(string.Format("Hello {0}", command.Name));
        }
    }

    public class LoggingExecutor<T>
    {
        readonly Action<T> _next;

        public LoggingExecutor(Action<T> next)
        {
            _next = next;
        }

        public void Handle(T command)
        {
            Console.WriteLine("----Log Entry: Command of type {0} received at {1}", typeof(T), DateTime.UtcNow);
            _next(command);
        }
    }

    public class SecurityEnabledExecutor<T>
    {
        readonly Action<T> _next;

        public SecurityEnabledExecutor(Action<T> next)
        {
            _next = next;
        }

        public void Handle(T command)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done security check");
            Console.ForegroundColor = prev;

            _next(command);
        }
    }
}
