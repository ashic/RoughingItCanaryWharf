using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MessageBasedTesting
{
    [TestFixture]
    public class when_overdrawing_three_times
    {
        object[] events;
        Guid accountId;


        [TestFixtureSetUp]
        public void ArrangeAndAct()
        {
            accountId = Guid.NewGuid();
            var account = new Account(accountId, 200);
            account.Debit(300);
            account.Debit(300);
            account.ClearEvents();
            account.Debit(300);

            events = account.UncommittedEvents.ToArray();
        }

        [Test]
        public void account_should_be_locked()
        {
            var e = events.OfType<AccountLockedEvent>().Single();
            Assert.AreEqual(accountId, e.AccountId);
        }
    }
}
