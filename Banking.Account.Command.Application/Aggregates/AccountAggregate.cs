using Banking.Account.Command.Application.Features.BankAccounts.Commands.OpenAccount;
using Banking.Cqrs.Core.Domain;
using Banking.Cqrs.Core.Events;

namespace Banking.Account.Command.Application.Aggregates
{
    public class AccountAggregate : AggregateRoot
    {
        public bool Active { get; set; }
        public double Balance { get; set; }

        public AccountAggregate()
        {
        }

        public AccountAggregate(OpenAccountCommand command)
        {
            var accountOpenedEvent = new AccountOpenedEvent(
                        command.Id,
                        command.AccountHolder,
                        command.AccountType,
                        DateTime.Now,
                        command.OpeningBalance
                );

            RaiseEvent(accountOpenedEvent);
        }

        public void Apply(AccountOpenedEvent @event)
        {
            Id = @event.Id;
            Active = true;
            Balance = @event.OpeningBalance;
        }

        public void DepositFunds(double amount)
        {
            if (!Active)
            {
                throw new Exception("Los fondos no pueden ser depositados en una cuenta inactiva");
            }

            if(amount < 0)
            {
                throw new Exception("El deposito de dinero debe ser mayor que 0");
            }

            var fundsDepositEvent = new FundsDepositedEvent(Id)
            {
                Id = Id,
                Amount = amount
            };

            RaiseEvent(fundsDepositEvent);
        }

        public void Apply(FundsDepositedEvent @event)
        {
            Id = @event.Id;
            Balance += @event.Amount;            
        }

        public void WithDrawFunds(double amount)
        {
            if(!Active)
            {
                throw new Exception("La cuenta bancaria no esta activa");
            }

            var fundsWithDrawnEvent = new FundsWithdrawnEvent(Id)
            {
                Id = Id,
                Amount = amount
            };

            RaiseEvent(fundsWithDrawnEvent);
        }

        public void Apply(FundsWithdrawnEvent @event)
        {
            Id = @event.Id;
            Balance -= @event.Amount;
        }

        public void CloseAccount()
        {
            if (!Active)
            {
                throw new Exception("La cuenta no esta activa");
            }

            var accountCloseEvent = new AccountClosedEvent(Id);
            RaiseEvent(accountCloseEvent);
        }

        public void Apply(AccountClosedEvent @event)
        {
            Id = @event.Id;
            Active = false;
        }
    }
}
