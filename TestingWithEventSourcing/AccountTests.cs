using System;
using System.Collections.Generic;

namespace TestingWithEventSourcing
{
    public class should_lock_account_after_3_overdraw_attempts : TestBase
    {
        Guid accountId;

        public override Dictionary<object, List<object>> GivenTheseEvents()
        {
            accountId = Guid.NewGuid();

            return  new Dictionary<object,List<object>>
            {
                {
                    accountId, new List<object>
                    {
                        new AccountRegisteredEvent(accountId, 200),
                        new OverdrawAttemptedEvent(accountId),
                        new OverdrawAttemptedEvent(accountId)
                    } 
                }
            };
        }

        public override object WhenThisHappens()
        {
            return new DebitCommand(accountId, 300);
        }

        public override IEnumerable<object> TheseEventsShouldOccur()
        {
            yield return new OverdrawAttemptedEvent(accountId);
            yield return new AccountLockedEvent(accountId);
        }

        public override void RegisterHandler(MessageBus bus, IRepository repo)
        {
            var executor = new AccountCommandExecutors(repo);
            bus.RegisterHandler<DebitCommand>(executor.Handle);
        }
    }
}