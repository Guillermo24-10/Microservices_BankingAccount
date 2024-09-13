using Banking.Account.Query.Application.Contracts.Persistence;
using Banking.Account.Query.Application.Models;
using Banking.Account.Query.Domain;
using Banking.Cqrs.Core.Events;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Banking.Account.Query.Infrastructure.Consumers
{
    public class BankAccountConsumerService : IHostedService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        public KafkaSettings _KafkaSettings { get; }

        public BankAccountConsumerService(IServiceScopeFactory factory)
        {
            _bankAccountRepository = factory.CreateScope().ServiceProvider.GetRequiredService<IBankAccountRepository>();
            _KafkaSettings = (factory.CreateScope().ServiceProvider.GetRequiredService<IOptions<KafkaSettings>>()).Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = _KafkaSettings.GroupId,
                BootstrapServers = $"{_KafkaSettings.Hostname}:{_KafkaSettings.Port}",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            try
            {
                using (var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    var bankTopics = new string[]
                    {
                        typeof(AccountOpenedEvent).Name,
                        typeof(AccountClosedEvent).Name,
                        typeof(FundsDepositedEvent).Name,
                        typeof(FundsWithdrawnEvent).Name,
                    };

                    consumerBuilder.Subscribe(bankTopics);
                    var cancelToken = new CancellationTokenSource();

                    try
                    {
                        while (true)
                        {
                            var consumer = consumerBuilder.Consume(cancelToken.Token);

                            if (consumer == null)
                            {
                                continue;
                            }

                            if (consumer.Topic == typeof(AccountOpenedEvent).Name)
                            {
                                var accountOpenedEvent = JsonConvert.DeserializeObject<AccountOpenedEvent>(consumer.Message.Value);

                                var bankAccount = new BankAccount
                                {
                                    Identifier = accountOpenedEvent!.Id,
                                    AccountHolder = accountOpenedEvent.AccountHolder,
                                    AccountType = accountOpenedEvent.AccountType,
                                    Balance = accountOpenedEvent.OpeningBalance,
                                    CreationDate = accountOpenedEvent.CreateDate,
                                };

                                _bankAccountRepository.AddAsync(bankAccount).Wait();
                            }

                            if (consumer.Topic == typeof(AccountClosedEvent).Name)
                            {
                                var accountCloseEvent = JsonConvert.DeserializeObject<AccountClosedEvent>(consumer.Message.Value);


                                _bankAccountRepository.DeleteByIdentifier(accountCloseEvent!.Id).Wait();
                            }

                            if (consumer.Topic == typeof(FundsDepositedEvent).Name)
                            {
                                var accountDepositEvent = JsonConvert.DeserializeObject<FundsDepositedEvent>(consumer.Message.Value);

                                var bankAccount = new BankAccount
                                {
                                    Identifier = accountDepositEvent!.Id,
                                    Balance = accountDepositEvent.Amount,
                                };

                                _bankAccountRepository.DepositBankAccountByIdentifier(bankAccount).Wait();
                            }

                            if (consumer.Topic == typeof(FundsWithdrawnEvent).Name)
                            {
                                var accountWithdrawnEvent = JsonConvert.DeserializeObject<FundsWithdrawnEvent>(consumer.Message.Value);

                                var bankAccount = new BankAccount
                                {
                                    Identifier = accountWithdrawnEvent!.Id,
                                    Balance = accountWithdrawnEvent.Amount,
                                };

                                _bankAccountRepository.WithdrawnBankAccountByIdentifier(bankAccount).Wait();
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        consumerBuilder.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
