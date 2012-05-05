using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rough;

namespace DI2
{
    class Program
    {
        static void Main()
        {
            var p = new Program();
            p.Run1();
            p.Run2();

        }

        void Run1()
        {
            var dispatcher = new Dispatcher1();
            dispatcher.Register<SayHelloCommand>(
                new SayHelloExecutor(
                    new ConsolePrinterWithTime(
                        new UtcClock(), new ConsolePrinter())).Handle);


            //.................
            dispatcher.Dispatch(new SayHelloCommand("Tom"));
        }

        void Run2()
        {
            var dispatcher = new Dispatcher1();
            dispatcher.Register<SayHelloCommand>(
                command =>
                    GreetingsExecutors
                    .Handle(command,
                        new ConsolePrinterWithTime(new UtcClock(), new ConsolePrinter())));

            //.................
            dispatcher.Dispatch(new SayHelloCommand("Tom"));
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

    public class SayHelloExecutor
    {
        readonly IPrint _printer;

        public SayHelloExecutor(IPrint printer)
        {
            _printer = printer;
        }

        public void Handle(SayHelloCommand command)
        {
            _printer.Print(string.Format("Hello {0}", command.Name));
        }
    }

    //-----------------------------------------------------------------------
    public class GreetingsExecutors
    {
        public static void Handle(SayHelloCommand command, IPrint printer)
        {
            printer.Print(string.Format("Hello {0}", command.Name));                        
        }
    }
    
}
