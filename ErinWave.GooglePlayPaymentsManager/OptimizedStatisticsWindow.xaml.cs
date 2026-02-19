using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ErinWave.GooglePlayPaymentsManager
{
    public partial class OptimizedStatisticsWindow : Window
    {
        private readonly List<PaymentItem> _payments;
        private readonly FastStatisticsCalculator _calculator;
        private SummaryStatistics _summary;

        public OptimizedStatisticsWindow(List<PaymentItem> payments)
        {
            InitializeComponent();
            _payments = payments ?? new List<PaymentItem>();
            _calculator = new FastStatisticsCalculator();

            Loaded += OptimizedStatisticsWindow_Loaded;
        }

        private async void OptimizedStatisticsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadStatisticsAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            try
            {
                ShowLoadingPanel(true);
                UpdateLoadingStatus("데이터 계산 시작...");

                // 모든 계산을 한 번에 백그라운드에서 수행
                var stats = await Task.Run(() =>
                {
                    var summary = _calculator.CalculateSummary(_payments);
                    var dailyStats = _calculator.FastCalculateDailyStatistics(_payments)
                        .Take(100).ToList(); // 이미 정렬되어 있음
                    var monthlyStats = _calculator.FastCalculateMonthlyStatistics(_payments); // 이미 정렬되어 있음
                    var yearlyStats = _calculator.FastCalculateYearlyStatistics(_payments); // 이미 정렬되어 있음
                    var gameStats = _calculator.FastCalculateGameStatistics(_payments); // 이미 정렬되어 있음

                    return new { Summary = summary, Daily = dailyStats, Monthly = monthlyStats, Yearly = yearlyStats, Game = gameStats };
                });

                // 한 번에 UI 업데이트
                _summary = stats.Summary;
                UpdateSummaryDisplay();
                DailyStatisticsGrid.ItemsSource = stats.Daily;
                MonthlyStatisticsGrid.ItemsSource = stats.Monthly;
                YearlyStatisticsGrid.ItemsSource = stats.Yearly;
                GameStatisticsGrid.ItemsSource = stats.Game;

                UpdateLoadingStatus("완료!");
                await Task.Delay(300);
                ShowLoadingPanel(false);
            }
            catch (Exception ex)
            {
                ShowLoadingPanel(false);
                MessageBox.Show($"통계 로드 중 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSummaryDisplay()
        {
            PeriodText.Text = $"기간: {_summary.PeriodText}";
            TotalSpentText.Text = _summary.FormattedTotal;
            TransactionCountText.Text = $"{_summary.TotalTransactions:N0}회";
            AverageAmountText.Text = _summary.FormattedAverage;
            MostExpensiveMonthText.Text = _summary.MostExpensiveMonth;
            MostExpensiveDayText.Text = _summary.MostExpensiveDay;
            MostSpentGameText.Text = _summary.MostSpentGame;
        }

        private void RefreshDailyButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshDailyStatisticsAsync();
        }

        private void RefreshMonthlyButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshMonthlyStatisticsAsync();
        }

        private void RefreshYearlyButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshYearlyStatisticsAsync();
        }

        private void RefreshGameButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshGameStatisticsAsync();
        }

        private async void RefreshDailyStatisticsAsync()
        {
            try
            {
                UpdateLoadingStatus("일별 통계 새로고침 중...");
                ShowLoadingPanel(true);

                var dailyStats = await Task.Run(() =>
                    _calculator.FastCalculateDailyStatistics(_payments)
                        .Take(100).ToList());

                DailyStatisticsGrid.ItemsSource = dailyStats;
                await Task.Delay(200);
                ShowLoadingPanel(false);
            }
            catch (Exception ex)
            {
                ShowLoadingPanel(false);
                MessageBox.Show($"일별 통계 새로고침 중 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RefreshMonthlyStatisticsAsync()
        {
            try
            {
                UpdateLoadingStatus("월별 통계 새로고침 중...");
                ShowLoadingPanel(true);

                var monthlyStats = await Task.Run(() =>
                    _calculator.FastCalculateMonthlyStatistics(_payments));

                MonthlyStatisticsGrid.ItemsSource = monthlyStats;
                await Task.Delay(200);
                ShowLoadingPanel(false);
            }
            catch (Exception ex)
            {
                ShowLoadingPanel(false);
                MessageBox.Show($"월별 통계 새로고침 중 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RefreshYearlyStatisticsAsync()
        {
            try
            {
                UpdateLoadingStatus("연별 통계 새로고침 중...");
                ShowLoadingPanel(true);

                var yearlyStats = await Task.Run(() =>
                    _calculator.FastCalculateYearlyStatistics(_payments));

                YearlyStatisticsGrid.ItemsSource = yearlyStats;
                await Task.Delay(200);
                ShowLoadingPanel(false);
            }
            catch (Exception ex)
            {
                ShowLoadingPanel(false);
                MessageBox.Show($"연별 통계 새로고침 중 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RefreshGameStatisticsAsync()
        {
            try
            {
                UpdateLoadingStatus("게임별 통계 새로고침 중...");
                ShowLoadingPanel(true);

                var gameStats = await Task.Run(() =>
                    _calculator.FastCalculateGameStatistics(_payments));

                GameStatisticsGrid.ItemsSource = gameStats;
                await Task.Delay(200);
                ShowLoadingPanel(false);
            }
            catch (Exception ex)
            {
                ShowLoadingPanel(false);
                MessageBox.Show($"게임별 통계 새로고침 중 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "통계 데이터 내보내기",
                    Filter = "CSV 파일 (*.csv)|*.csv|텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*",
                    DefaultExt = "csv",
                    FileName = $"GooglePlay_통계_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportStatistics(saveFileDialog.FileName);
                    MessageBox.Show("통계 데이터가 성공적으로 내보내기 되었습니다.", "완료", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"내보내기 중 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportStatistics(string filePath)
        {
            var sb = new StringBuilder();

            // 요약 정보
            sb.AppendLine("Google Play 결제 통계 요약");
            sb.AppendLine($"기간: {_summary.PeriodText}");
            sb.AppendLine($"총 지출액: {_summary.FormattedTotal}");
            sb.AppendLine($"거래 횟수: {_summary.TotalTransactions:N0}회");
            sb.AppendLine($"평균 결제액: {_summary.FormattedAverage}");
            sb.AppendLine($"가장 많이 소비한 달: {_summary.MostExpensiveMonth}");
            sb.AppendLine($"가장 많이 소비한 날: {_summary.MostExpensiveDay}");
            sb.AppendLine();

            // 월별 통계
            sb.AppendLine("월별 통계");
            sb.AppendLine("년월,거래 횟수,총액,평균");
            var monthlyStats = _calculator.FastCalculateMonthlyStatistics(_payments);
            foreach (var stat in monthlyStats) // 이미 정렬되어 있음
            {
                sb.AppendLine($"{stat.DisplayText},{stat.TransactionCount},{stat.FormattedTotal},{stat.FormattedAverage}");
            }
            sb.AppendLine();

            // 일별 통계 (상위 30개)
            sb.AppendLine("일별 통계 (상위 30개)");
            sb.AppendLine("날짜,거래 횟수,총액,평균");
            var dailyStats = _calculator.FastCalculateDailyStatistics(_payments).Take(30);
            foreach (var stat in dailyStats)
            {
                sb.AppendLine($"{stat.Date},{stat.TransactionCount},{stat.FormattedTotal},{stat.FormattedAverage}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ShowLoadingPanel(bool show)
        {
            if (show)
            {
                LoadingPanel.Visibility = Visibility.Visible;
                StatisticsTabControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoadingPanel.Visibility = Visibility.Collapsed;
                StatisticsTabControl.Visibility = Visibility.Visible;
            }
        }

        private void UpdateLoadingStatus(string status)
        {
            LoadingStatusText.Text = status;
        }
    }
}