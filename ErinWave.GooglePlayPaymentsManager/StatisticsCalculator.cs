using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ErinWave.GooglePlayPaymentsManager
{
    public class StatisticsCalculator
    {
        public SummaryStatistics CalculateSummary(List<PaymentItem> payments)
        {
            var validPayments = payments.Where(p => p.Amount < 0).ToList();

            if (!validPayments.Any())
            {
                return new SummaryStatistics();
            }

            var summary = new SummaryStatistics
            {
                TotalSpent = validPayments.Sum(p => Math.Abs(p.Amount)),
                TotalTransactions = validPayments.Count,
                AverageTransaction = validPayments.Average(p => Math.Abs(p.Amount)),
                FirstTransactionDate = validPayments.Min(p => ParseDate(p.Date)),
                LastTransactionDate = validPayments.Max(p => ParseDate(p.Date))
            };

            // 월별 통계 계산
            var monthlyStats = CalculateMonthlyStatistics(validPayments);
            if (monthlyStats.Any())
            {
                var mostExpensiveMonth = monthlyStats.OrderByDescending(m => m.TotalAmount).First();
                summary.MostExpensiveMonth = $"{mostExpensiveMonth.DisplayText} ({mostExpensiveMonth.FormattedTotal})";
            }

            // 일별 통계 계산
            var dailyStats = CalculateDailyStatistics(validPayments);
            if (dailyStats.Any())
            {
                var mostExpensiveDay = dailyStats.OrderByDescending(d => d.TotalAmount).First();
                summary.MostExpensiveDay = $"{mostExpensiveDay.Date} ({mostExpensiveDay.FormattedTotal})";
            }

            return summary;
        }

        public List<DailyStatistics> CalculateDailyStatistics(List<PaymentItem> payments)
        {
            var validPayments = payments.Where(p => p.Amount < 0).ToList();

            // LINQ를 사용하지 않고 직접 그룹화하여 성능 향상
            var dailyDict = new Dictionary<string, DailyStatistics>();

            foreach (var payment in validPayments)
            {
                if (!dailyDict.ContainsKey(payment.Date))
                {
                    dailyDict[payment.Date] = new DailyStatistics
                    {
                        Date = payment.Date,
                        TotalAmount = 0,
                        TransactionCount = 0
                    };
                }

                dailyDict[payment.Date].TotalAmount += Math.Abs(payment.Amount);
                dailyDict[payment.Date].TransactionCount++;
            }

            return dailyDict.Values.OrderBy(d => ParseDate(d.Date)).ToList();
        }

        public List<MonthlyStatistics> CalculateMonthlyStatistics(List<PaymentItem> payments)
        {
            var validPayments = payments.Where(p => p.Amount < 0).ToList();
            var monthlyDict = new Dictionary<string, MonthlyStatistics>();

            foreach (var payment in validPayments)
            {
                string yearMonth = GetYearMonth(payment.Date);
                if (!monthlyDict.ContainsKey(yearMonth))
                {
                    monthlyDict[yearMonth] = new MonthlyStatistics
                    {
                        YearMonth = yearMonth,
                        DisplayText = FormatYearMonth(yearMonth),
                        TotalAmount = 0,
                        TransactionCount = 0
                    };
                }

                monthlyDict[yearMonth].TotalAmount += Math.Abs(payment.Amount);
                monthlyDict[yearMonth].TransactionCount++;
            }

            return monthlyDict.Values.OrderBy(m => m.YearMonth).ToList();
        }

        public List<YearlyStatistics> CalculateYearlyStatistics(List<PaymentItem> payments)
        {
            var validPayments = payments.Where(p => p.Amount < 0).ToList();
            var yearlyDict = new Dictionary<string, YearlyStatistics>();

            foreach (var payment in validPayments)
            {
                string year = GetYear(payment.Date);
                if (!yearlyDict.ContainsKey(year))
                {
                    yearlyDict[year] = new YearlyStatistics
                    {
                        Year = year,
                        TotalAmount = 0,
                        TransactionCount = 0
                    };
                }

                yearlyDict[year].TotalAmount += Math.Abs(payment.Amount);
                yearlyDict[year].TransactionCount++;
            }

            return yearlyDict.Values.OrderBy(y => y.Year).ToList();
        }

        private DateTime ParseDate(string dateStr)
        {
            // "10월 25일", "2024년 4월 6일" 형식 처리
            try
            {
                if (dateStr.Contains("년"))
                {
                    // "2024년 4월 6일" 형식
                    return DateTime.ParseExact(dateStr.Replace(" ", ""), "yyyy년MM월dd일", CultureInfo.InvariantCulture);
                }
                else
                {
                    // "10월 25일" 형식 - 현재 연도로 가정
                    int year = DateTime.Now.Year;
                    string fullDate = $"{year}년 {dateStr}";
                    return DateTime.ParseExact(fullDate.Replace(" ", ""), "yyyy년MM월dd일", CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        private string GetYearMonth(string dateStr)
        {
            var date = ParseDate(dateStr);
            return date.ToString("yyyy-MM");
        }

        private string GetYear(string dateStr)
        {
            var date = ParseDate(dateStr);
            return date.ToString("yyyy");
        }

        private string FormatYearMonth(string yearMonth)
        {
            if (DateTime.TryParseExact(yearMonth + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date.ToString("yyyy년 MM월");
            }
            return yearMonth;
        }
    }
}