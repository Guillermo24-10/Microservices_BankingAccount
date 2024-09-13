using Banking.Account.Command.Application.Features.BankAccounts.Commands.CloseAccount;
using Banking.Account.Command.Application.Features.BankAccounts.Commands.DepositFund;
using Banking.Account.Command.Application.Features.BankAccounts.Commands.OpenAccount;
using Banking.Account.Command.Application.Features.BankAccounts.Commands.WithDrawnFund;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Banking.Account.Command.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BankAccountOperationsAccountController : ControllerBase
    {
        private IMediator _mediator;

        public BankAccountOperationsAccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("OpenAccount", Name = "OpenAccount")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> OpenAccount(OpenAccountCommand command)
        {
            var id = Guid.NewGuid().ToString();
            command.Id = id;
            return await _mediator.Send(command);
        }

        [HttpDelete("CloseAccount/{id}", Name = "CloseAccount")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<bool>> CloseAccount(string id)
        {
            var command = new CloseAccountCommand
            {
                Id = id
            };

            return await _mediator.Send(command);
        }

        [HttpPut("DepositFund/{id}", Name = "DepositFund")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<bool>> DepositFund(string id, [FromBody] DepositFundsCommand command)
        {
            command.Id = id;
            return await _mediator.Send(command);
        }

        [HttpPut("WithDrawFund/{id}", Name = "WithDrawFund")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<bool>> WithDrawFund(string id, [FromBody] WithDrawFundsCommand command)
        {
            command.Id = id;
            return await _mediator.Send(command);
        }

    }
}
