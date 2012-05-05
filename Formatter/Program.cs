using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestingWithEventSourcing;

namespace Formatter
{
    class Program
    {
        static void Main()
        {
            new Program().Run();
        }

        void Run()
        {
            var testTypes = typeof (Account).Assembly.GetTypes().Where(x => typeof(TestBase).IsAssignableFrom(x) && x.IsAbstract == false);
            foreach (var type in testTypes)
            {
                var test = Activator.CreateInstance(type) as TestBase;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(type.Name);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Given - ");
                Console.ForegroundColor = ConsoleColor.Green;

                var events = test.GivenTheseEvents().Values.SelectMany(x=>x).ToList();

                foreach(var e in events)
                    Console.WriteLine(e);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("When - ");
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine(test.WhenThisHappens());


                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Then - ");
                Console.ForegroundColor = ConsoleColor.Green;

                foreach(var e in test.TheseEventsShouldOccur())
                    Console.WriteLine(e);
            }
        }
    }
}
