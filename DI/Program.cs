using Rough;

namespace DI
{
    class Program
    {
        public static void Main()
        {
            var p = new Program();
            p.RunDIDemo();
            p.RunDIDemo2();
            p.RunDIDemo3();
        }

        void RunDIDemo()
        {
            ISayHello hello = new DIDemo(new ConsolePrinter()); //<- this is what you register in composition root

            //...
            hello.SayHello("John");
        }

        void RunDIDemo2()
        {
            var clock = new UtcClock();
            var printer = new ConsolePrinterWithTime(clock, new ConsolePrinter());
            ISayHello hello = new DIDemo(printer); //<- this is what you register in composition root

            //......
            hello.SayHello("Tim");
        }

        //..............................................

        void RunDIDemo3()
        {
            var factory = new Factory();
            factory.RegisterTransient(getSayHello);

            //......
            var hello = factory.Resolve<ISayHello>();
            hello.SayHello("Tim");
        }

        ISayHello getSayHello()
        {
            var clock = new UtcClock();
            var printer = new ConsolePrinterWithTime(clock, new ConsolePrinter());
            ISayHello hello = new DIDemo(printer);

            return hello;
        }
    }
}
