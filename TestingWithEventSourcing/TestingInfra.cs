using System.Collections.Generic;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;
using System.Linq;

namespace TestingWithEventSourcing
{
    [TestFixture]
    public abstract class TestBase
    {
        public abstract Dictionary<object, List<object>> GivenTheseEvents();
        public abstract object WhenThisHappens();
        public abstract IEnumerable<object> TheseEventsShouldOccur();
        public abstract void RegisterHandler(MessageBus bus, IRepository repo);

        [Test]
        public void Test()
        {
            var newMessages = new List<object>();
            var bus = new MessageBus();
            bus.RegisterHandler<object>(newMessages.Add);

            var eventStore = new InMemoryEventStore(bus, GivenTheseEvents());
            var repository = new DomainRepository(eventStore);

            RegisterHandler(bus, repository);
            bus.Handle(WhenThisHappens());

            var expected = TheseEventsShouldOccur();
            Assert.AreEqual(expected.Count(), newMessages.Count);
        }
    }
}