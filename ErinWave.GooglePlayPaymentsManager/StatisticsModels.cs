using System;
using System.Collections.Generic;

namespace ErinWave.GooglePlayPaymentsManager
{
    // 일별 통계
    public class DailyStatistics
    {
        private string _date = string.Empty;
        public string Date
        {
            get => _date;
            set => _date = FormatDateWithYear(value);
        }
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageAmount => TransactionCount > 0 ? TotalAmount / TransactionCount : 0;
        public string FormattedTotal => $"₩{Math.Abs(TotalAmount):N0}";
        public string FormattedAverage => $"₩{Math.Abs(AverageAmount):N0}";

        private string FormatDateWithYear(string dateStr)
        {
            if (dateStr.Contains("년"))
                return dateStr;

            return $"2025년 {dateStr}";
        }
    }

    // 월별 통계
    public class MonthlyStatistics
    {
        public string YearMonth { get; set; } = string.Empty; // "2024-10"
        public string DisplayText { get; set; } = string.Empty; // "2024년 10월"
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageAmount => TransactionCount > 0 ? TotalAmount / TransactionCount : 0;
        public string FormattedTotal => $"₩{Math.Abs(TotalAmount):N0}";
        public string FormattedAverage => $"₩{Math.Abs(AverageAmount):N0}";
    }

    // 연별 통계
    public class YearlyStatistics
    {
        public string Year { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageAmount => TransactionCount > 0 ? TotalAmount / TransactionCount : 0;
        public decimal MonthlyAverage => 12; // 기본값
        public string FormattedTotal => $"₩{Math.Abs(TotalAmount):N0}";
        public string FormattedAverage => $"₩{Math.Abs(AverageAmount):N0}";
        public string FormattedMonthlyAverage => $"₩{Math.Abs(TotalAmount / 12):N0}";
    }

    // 게임별 통계
    public class GameStatistics
    {
        public string GameName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageAmount => TransactionCount > 0 ? TotalAmount / TransactionCount : 0;
        public string FormattedTotal => $"₩{Math.Abs(TotalAmount):N0}";
        public string FormattedAverage => $"₩{Math.Abs(AverageAmount):N0}";
        public double Percentage { get; set; } // 전체 지출에서 차지하는 비율
        public string FormattedPercentage => $"{Percentage:F1}%";
    }

    // 전체 통계 요약
    public class SummaryStatistics
    {
        public decimal TotalSpent { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageTransaction { get; set; }
        public DateTime? FirstTransactionDate { get; set; }
        public DateTime? LastTransactionDate { get; set; }
        public string MostExpensiveMonth { get; set; } = string.Empty;
        public string MostExpensiveDay { get; set; } = string.Empty;
        public string TopSpendingCategory { get; set; } = "Google Play 앱";
        public string MostSpentGame { get; set; } = string.Empty;

        public string FormattedTotal => $"₩{Math.Abs(TotalSpent):N0}";
        public string FormattedAverage => $"₩{Math.Abs(AverageTransaction):N0}";
        public string PeriodText
        {
            get
            {
                if (FirstTransactionDate.HasValue && LastTransactionDate.HasValue)
                {
                    return $"{FirstTransactionDate.Value:yyyy년 MM월 dd일} ~ {LastTransactionDate.Value:yyyy년 MM월 dd일}";
                }
                return "데이터 없음";
            }
        }
    }
}