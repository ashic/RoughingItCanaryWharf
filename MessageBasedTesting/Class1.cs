using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageBasedTesting
{
    public abstract class Aggregate
    {
        public Guid Id { get; private set; }

        protected Aggregate(Guid id)
        {
            Id = id;
            UncommittedEvents = new List<object>();
        }

        public List<object> UncommittedEvents { get; private set; }

        public void Apply(object @event)
        {
            UncommittedEvents.Add(@event);
        }

        public void Apply<T>(T @event, Action<T> update)
        {
            Apply(@event);
            update(@event);
        }

        public void ClearEvents()
        {
            UncommittedEvents.Clear();
        }
    }



    public class Account : Aggregate
    {
        public decimal Balance { get; set; }
        public int CurrentOverdrawAttempts { get; set; }

        public bool IsLocked
        {
            get { return CurrentOverdrawAttempts > 2; }
        }

        public Account(Guid id, decimal balance) : base(id)
        {
            Apply(new AccountRegisteredEvent(id, balance), updateFrom);
        }

        public void Debit(decimal amount)
        {
            if (IsLocked)
                throw new InvalidOperationException();

            if (Balance < amount)
            {
                Apply(new OverdrawAttemptedEvent(Id), updateFrom);
                if (CurrentOverdrawAttempts == 3)
                    Apply(new AccountLockedEvent(Id));
            }
            else
                Apply(new AccountDebitedEvent(Id, amount, Balance - amount), updateFrom);
        }

        void updateFrom(AccountRegisteredEvent e)
        {
            Balance = e.AccountBalance;
        }

        private void updateFrom(OverdrawAttemptedEvent e)
        {
            CurrentOverdrawAttempts++;
        }

        private void updateFrom(AccountDebitedEvent e)
        {
            Balance = e.NewBalance;
        }
    }





    class AccountRegisteredEvent
    {
        public Guid AccountId { get; private set; }
        public decimal AccountBalance { get; private set; }

        public AccountRegisteredEvent(Guid accountId, decimal accountBalance)
        {
            AccountId = accountId;
            AccountBalance = accountBalance;
        }
    }

    class AccountLockedEvent
    {
        public Guid AccountId { get; private set; }

        public AccountLockedEvent(Guid accountId)
        {
            AccountId = accountId;
        }
    }

    public class AccountDebitedEvent
    {
        public Guid AccountId { get; private set; }
        public decimal Amount { get; private set; }
        public decimal NewBalance { get; private set; }

        public AccountDebitedEvent(Guid accountId, decimal amount, decimal @decimal)
        {
            AccountId = accountId;
            Amount = amount;
            NewBalance = @decimal;
        }
    }

    public class OverdrawAttemptedEvent
    {
        public Guid AccountId { get; private set; }

        public OverdrawAttemptedEvent(Guid accountId)
        {
            AccountId = accountId;
        }
    }
}