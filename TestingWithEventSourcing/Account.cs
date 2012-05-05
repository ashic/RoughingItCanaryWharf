using System;

namespace TestingWithEventSourcing
{
    public class Account : Aggregate
    {
        decimal _balance;
        int _numberOfOverdraws;
        bool isLocked;

        private Account(){}

        public Account(Guid accountId, decimal balance) : base(accountId)
        {
            Apply(new AccountRegisteredEvent(accountId, balance));
        }

        private void UpdateFrom(AccountRegisteredEvent e)
        {
            Id = e.AccountId;
            _balance = e.Balance;
        }

        public void Debit(decimal amount)
        {
            if (isLocked)
                throw new InvalidOperationException();

            if (_balance < amount)
            {
                Apply(new OverdrawAttemptedEvent((Guid)Id));

                if (_numberOfOverdraws == 3)
                    Apply(new AccountLockedEvent((Guid)Id));
            }
            else Apply(new AccountDebitedEvent((Guid) Id, amount, _balance - amount));
        }

        private void UpdateFrom(OverdrawAttemptedEvent e)
        {
            _numberOfOverdraws++;
        }

        private void UpdateFrom(AccountLockedEvent e)
        {
            isLocked = true;
        }

        private void UpdateFrom(AccountDebitedEvent e)
        {
            _balance = e.NewBalance;
        }

    }



    
    public class AccountDebitedEvent
    {
        public Guid AccountId { get; private set; }
        public decimal Amount { get; private set; }
        public decimal NewBalance { get; private set; }

        public AccountDebitedEvent(Guid accountId, decimal amount, decimal newBalance)
        {
            AccountId = accountId;
            Amount = amount;
            NewBalance = newBalance;
        }

        public override string ToString()
        {
            return string.Format("Account {0} debited {1}. New balance {2}.", AccountId, Amount, NewBalance);
        }
    }

    public class AccountLockedEvent
    {
        public Guid AccountId { get; private set; }

        public AccountLockedEvent(Guid id)
        {
            AccountId = id;
        }

        public override string ToString()
        {
            return string.Format("Account {0} locked.", AccountId);
        }
    }

    public class OverdrawAttemptedEvent
    {
        public Guid AccountId { get; private set; }

        public OverdrawAttemptedEvent(Guid id)
        {
            AccountId = id;
        }

        public override string ToString()
        {
            return string.Format("Overdraw attempt on account {0}.", AccountId);
        }
    }

    public class AccountRegisteredEvent
    {
        public Guid AccountId { get; private set; }
        public decimal Balance { get; private set; }

        public AccountRegisteredEvent(Guid accountId, decimal balance)
        {
            AccountId = accountId;
            Balance = balance;
        }

        public override string ToString()
        {
            return string.Format("Account {0} registered with balance {1}", AccountId, Balance);
        }
    }
}