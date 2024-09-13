using Banking.Account.Command.Application.Aggregates;
using Banking.Cqrs.Core.Handlers;
using MediatR;

namespace Banking.Account.Command.Application.Features.BankAccounts.Commands.WithDrawnFund
{
    public class WithDrawFundsCommandHandler : IRequestHandler<WithDrawFundsCommand, bool>
    {
        private readonly EventSourcingHandler<AccountAggregate> _eventSourcingHandler;

        public WithDrawFundsCommandHandler(EventSourcingHandler<AccountAggregate> eventSourcingHandler)
        {
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task<bool> Handle(WithDrawFundsCommand request, CancellationToken cancellationToken)
        {
            var aggregate = await _eventSourcingHandler.GetById(request.Id);
            aggregate.WithDrawFunds(request.Amount);
            await _eventSourcingHandler.Save(aggregate);
            return true;
        }
    }
}
