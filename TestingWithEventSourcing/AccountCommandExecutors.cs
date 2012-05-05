using System;

namespace TestingWithEventSourcing
{
    public class DebitCommand
    {
        public Guid AccountId { get; private set; }
        public decimal Amount { get; private set; }

        public DebitCommand(Guid accountId, decimal amount)
        {
            AccountId = accountId;
            Amount = amount;
        }

        public override string ToString()
        {
            return string.Format("Attempt to debit {0} from account {1}", Amount, AccountId);
        }
    }
    
    public class AccountCommandExecutors
    {
        readonly IRepository repo;

        public AccountCommandExecutors(IRepository repo)
        {
            this.repo = repo;
        }

        public void Handle(DebitCommand command)
        {
            var account = repo.GetById<Account>(command.AccountId);
            account.Debit(command.Amount);

            repo.Save(account);
        }
    }
}