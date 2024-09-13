using MediatR;

namespace Banking.Account.Command.Application.Features.BankAccounts.Commands.WithDrawnFund
{
    public class WithDrawFundsCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;
        public double Amount { get; set; }
    }
}
