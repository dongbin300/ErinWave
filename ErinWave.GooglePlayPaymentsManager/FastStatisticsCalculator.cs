using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ErinWave.GooglePlayPaymentsManager
{
    public class FastStatisticsCalculator
    {
        private Dictionary<string, DateTime> _dateCache = new Dictionary<string, DateTime>();

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
                FirstTransactionDate = validPayments.Min(p => FastParseDate(p.Date)),
                LastTransactionDate = validPayments.Max(p => FastParseDate(p.Date))
            };

            // 월별 통계 계산
            var monthlyStats = FastCalculateMonthlyStatistics(validPayments);
            if (monthlyStats.Any())
            {
                var mostExpensiveMonth = monthlyStats.OrderByDescending(m => m.TotalAmount).First();
                summary.MostExpensiveMonth = $"{mostExpensiveMonth.DisplayText} ({mostExpensiveMonth.FormattedTotal})";
            }

            // 일별 통계 계산
            var dailyStats = FastCalculateDailyStatistics(validPayments);
            if (dailyStats.Any())
            {
                var mostExpensiveDay = dailyStats.OrderByDescending(d => d.TotalAmount).First();
                summary.MostExpensiveDay = $"{mostExpensiveDay.Date} ({mostExpensiveDay.FormattedTotal})";
            }

            // 게임별 통계 계산
            var gameStats = FastCalculateGameStatistics(validPayments);
            if (gameStats.Any())
            {
                var mostSpentGame = gameStats.OrderByDescending(g => g.TotalAmount).First();
                summary.MostSpentGame = $"{mostSpentGame.GameName} ({mostSpentGame.FormattedTotal})";
            }

            return summary;
        }

        public List<DailyStatistics> FastCalculateDailyStatistics(List<PaymentItem> payments)
        {
            var validPayments = payments.Where(p => p.Amount < 0).ToList();
            var dailyDict = new Dictionary<string, DailyStatistics>(validPayments.Count);

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

            return dailyDict.Values.OrderByDescending(d => d.TotalAmount).ToList();
        }

        public List<MonthlyStatistics> FastCalculateMonthlyStatistics(List<PaymentItem> payments)
        {
            var validPayments = payments.Where(p => p.Amount < 0).ToList();
            var monthlyDict = new Dictionary<string, MonthlyStatistics>();

            foreach (var payment in validPayments)
            {
                string yearMonth = FastGetYearMonth(payment.Date);
                if (!monthlyDict.ContainsKey(yearMonth))
                {
                    monthlyDict[yearMonth] = new MonthlyStatistics
                    {
                        YearMonth = yearMonth,
                        DisplayText = FastFormatYearMonth(yearMonth),
                        TotalAmount = 0,
                        TransactionCount = 0
                    };
                }

                monthlyDict[yearMonth].TotalAmount += Math.Abs(payment.Amount);
                monthlyDict[yearMonth].TransactionCount++;
            }

            return monthlyDict.Values.OrderByDescending(m => m.TotalAmount).ToList();
        }

        public List<YearlyStatistics> FastCalculateYearlyStatistics(List<PaymentItem> payments)
        {
            var validPayments = payments.Where(p => p.Amount < 0).ToList();
            var yearlyDict = new Dictionary<string, YearlyStatistics>();

            foreach (var payment in validPayments)
            {
                string year = FastGetYear(payment.Date);
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

            return yearlyDict.Values.OrderByDescending(y => y.TotalAmount).ToList();
        }

        public List<GameStatistics> FastCalculateGameStatistics(List<PaymentItem> payments)
        {
            var validPayments = payments.Where(p => p.Amount < 0).ToList();
            var gameDict = new Dictionary<string, GameStatistics>();
            decimal totalAmount = validPayments.Sum(p => Math.Abs(p.Amount));

            foreach (var payment in validPayments)
            {
                string gameName = ExtractGameName(payment.ProductName);
                if (!gameDict.ContainsKey(gameName))
                {
                    gameDict[gameName] = new GameStatistics
                    {
                        GameName = gameName,
                        TotalAmount = 0,
                        TransactionCount = 0
                    };
                }

                gameDict[gameName].TotalAmount += Math.Abs(payment.Amount);
                gameDict[gameName].TransactionCount++;
            }

            // 비율 계산
            foreach (var game in gameDict.Values)
            {
                game.Percentage = totalAmount > 0 ? (double)(game.TotalAmount / totalAmount * 100) : 0;
            }

            return gameDict.Values.OrderByDescending(g => g.TotalAmount).ToList();
        }

        private string ExtractGameName(string productName)
        {
            // "Eco Golden Mine (Post Apo Tycoon - Idle Builder)" -> "Post Apo Tycoon - Idle Builder"
            int startIndex = productName.LastIndexOf('(');
            int endIndex = productName.LastIndexOf(')');

            if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
            {
                return productName.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
            }

            // 괄호가 없는 경우 전체 상품명 반환
            return productName;
        }

        private DateTime FastParseDate(string dateStr)
        {
            if (_dateCache.TryGetValue(dateStr, out DateTime cachedDate))
            {
                return cachedDate;
            }

            try
            {
                if (dateStr.Contains("년"))
                {
                    // "2024년 4월 6일" 형식
                    var result = DateTime.ParseExact(dateStr.Replace(" ", ""), "yyyy년MM월dd일", CultureInfo.InvariantCulture);
                    _dateCache[dateStr] = result;
                    return result;
                }
                else
                {
                    // "10월 25일" 형식 - 현재 연도로 가정
                    int year = DateTime.Now.Year;
                    string fullDate = $"{year}년 {dateStr}";
                    var result = DateTime.ParseExact(fullDate.Replace(" ", ""), "yyyy년MM월dd일", CultureInfo.InvariantCulture);
                    _dateCache[dateStr] = result;
                    return result;
                }
            }
            catch
            {
                var result = DateTime.MinValue;
                _dateCache[dateStr] = result;
                return result;
            }
        }

        private string FastGetYearMonth(string dateStr)
        {
            var date = FastParseDate(dateStr);
            return date.ToString("yyyy-MM");
        }

        private string FastGetYear(string dateStr)
        {
            var date = FastParseDate(dateStr);
            return date.ToString("yyyy");
        }

        private string FastFormatYearMonth(string yearMonth)
        {
            if (DateTime.TryParseExact(yearMonth + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date.ToString("yyyy년 MM월");
            }
            return yearMonth;
        }
    }
}