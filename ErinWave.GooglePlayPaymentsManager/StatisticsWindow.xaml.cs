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
    public partial class StatisticsWindow : Window
    {
        private readonly List<PaymentItem> _payments;
        private readonly StatisticsCalculator _calculator;
        private SummaryStatistics _summary;

        public StatisticsWindow(List<PaymentItem> payments)
        {
            InitializeComponent();
            _payments = payments ?? new List<PaymentItem>();
            _calculator = new StatisticsCalculator();

            Loaded += StatisticsWindow_Loaded;
        }

        private async void StatisticsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadStatisticsAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            try
            {
                // 로딩 패널 표시
                ShowLoadingPanel(true);
                UpdateLoadingStatus("데이터 계산 시작...");

                await Task.Run(() =>
                {
                    // 요약 통계 계산
                    Dispatcher.Invoke(() => UpdateLoadingStatus("요약 통계 계산 중..."));
                    _summary = _calculator.CalculateSummary(_payments);

                    Dispatcher.Invoke(() => UpdateSummaryDisplay());

                    // 일별 통계 계산 (최대 100개만 표시)
                    Dispatcher.Invoke(() => UpdateLoadingStatus("일별 통계 계산 중..."));
                    var dailyStats = _calculator.CalculateDailyStatistics(_payments)
                        .OrderByDescending(d => d.TotalAmount).Take(100).ToList();
                    Dispatcher.Invoke(() => DailyStatisticsGrid.ItemsSource = dailyStats);

                    // 월별 통계 계산
                    Dispatcher.Invoke(() => UpdateLoadingStatus("월별 통계 계산 중..."));
                    var monthlyStats = _calculator.CalculateMonthlyStatistics(_payments)
                        .OrderByDescending(m => m.TotalAmount).ToList();
                    Dispatcher.Invoke(() => MonthlyStatisticsGrid.ItemsSource = monthlyStats);

                    // 연별 통계 계산
                    Dispatcher.Invoke(() => UpdateLoadingStatus("연별 통계 계산 중..."));
                    var yearlyStats = _calculator.CalculateYearlyStatistics(_payments)
                        .OrderByDescending(y => y.TotalAmount).ToList();
                    Dispatcher.Invoke(() => YearlyStatisticsGrid.ItemsSource = yearlyStats);

                    Dispatcher.Invoke(() => UpdateLoadingStatus("완료!"));
                });

                // 로딩 패널 숨기기
                await Task.Delay(500); // 완료 표시를 위해 잠시 대기
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

        private async void RefreshDailyStatisticsAsync()
        {
            try
            {
                UpdateLoadingStatus("일별 통계 새로고침 중...");
                ShowLoadingPanel(true);

                await Task.Run(() =>
                {
                    var dailyStats = _calculator.CalculateDailyStatistics(_payments)
                        .OrderByDescending(d => d.TotalAmount).Take(100).ToList();
                    Dispatcher.Invoke(() => DailyStatisticsGrid.ItemsSource = dailyStats);
                });

                await Task.Delay(300);
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

                await Task.Run(() =>
                {
                    var monthlyStats = _calculator.CalculateMonthlyStatistics(_payments)
                        .OrderByDescending(m => m.TotalAmount).ToList();
                    Dispatcher.Invoke(() => MonthlyStatisticsGrid.ItemsSource = monthlyStats);
                });

                await Task.Delay(300);
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

                await Task.Run(() =>
                {
                    var yearlyStats = _calculator.CalculateYearlyStatistics(_payments)
                        .OrderByDescending(y => y.TotalAmount).ToList();
                    Dispatcher.Invoke(() => YearlyStatisticsGrid.ItemsSource = yearlyStats);
                });

                await Task.Delay(300);
                ShowLoadingPanel(false);
            }
            catch (Exception ex)
            {
                ShowLoadingPanel(false);
                MessageBox.Show($"연별 통계 새로고침 중 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
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
            var monthlyStats = _calculator.CalculateMonthlyStatistics(_payments);
            foreach (var stat in monthlyStats.OrderByDescending(m => m.TotalAmount))
            {
                sb.AppendLine($"{stat.DisplayText},{stat.TransactionCount},{stat.FormattedTotal},{stat.FormattedAverage}");
            }
            sb.AppendLine();

            // 일별 통계 (상위 30개)
            sb.AppendLine("일별 통계 (상위 30개)");
            sb.AppendLine("날짜,거래 횟수,총액,평균");
            var dailyStats = _calculator.CalculateDailyStatistics(_payments).Take(30);
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