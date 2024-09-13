using Banking.Account.Query.Domain;
using MediatR;

namespace Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAllAccount
{
    public class FindAllAccountsQuery : IRequest<IEnumerable<BankAccount>>
    {
    }
}
