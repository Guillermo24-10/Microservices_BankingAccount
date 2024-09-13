namespace Banking.Cqrs.Core.Events
{
    public class AccountOpenedEvent : BaseEvent
    {
        public string AccountHolder { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public double OpeningBalance { get; set; }

        public AccountOpenedEvent(
            string id,
            string accountHolder,
            string accountType,
            DateTime createDate,
            double openingBalance) : base(id)
        {
            AccountType = accountType;
            CreateDate = createDate;
            OpeningBalance = openingBalance;
            AccountHolder = accountHolder;
        }
    }
}
