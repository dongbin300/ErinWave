using System;

namespace ErinWave.GooglePlayPaymentsManager
{
    public class PaymentItem
    {
        public string Store { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string FormattedAmount { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public string DisplayText => $"{Date} · {ProductName}";
        public string AmountDisplay => $"{(Amount < 0 ? "-" : "")}{Currency}{Math.Abs(Amount):N0}";
    }
}