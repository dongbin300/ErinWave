using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ErinWave.GooglePlayPaymentsManager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly RealPaymentParser _parser;
		private List<PaymentItem> _payments;

		public MainWindow()
		{
			InitializeComponent();
			_parser = new RealPaymentParser();
			_payments = new List<PaymentItem>();

			// 통계 버튼 초기 비활성화
			StatisticsButton.IsEnabled = false;

			// 자동으로 기본 파일에서 데이터 로드 시도
			LoadDefaultFile();
		}

		private void LoadDefaultFile()
		{
			try
			{
				string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "payments.txt");
				if (File.Exists(defaultPath))
				{
					LoadPaymentsFromFile(defaultPath);
				}
			}
			catch (Exception ex)
			{
				StatusText.Text = $"기본 파일 로드 실패: {ex.Message}";
			}
		}

		private void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				Title = "결제 내역 파일 선택",
				Filter = "텍스트 파일 (*.txt)|*.txt|HTML 파일 (*.html;*.htm)|*.html;*.htm|모든 파일 (*.*)|*.*",
				InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
			};

			if (openFileDialog.ShowDialog() == true)
			{
				LoadPaymentsFromFile(openFileDialog.FileName);
			}
		}

		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			if (_payments.Any())
			{
				// 현재 데이터 새로고침
				PaymentsDataGrid.ItemsSource = null;
				PaymentsDataGrid.ItemsSource = _payments;
				UpdateSummary();
				StatusText.Text = "데이터가 새로고침되었습니다.";
			}
			else
			{
				StatusText.Text = "새로고칠 데이터가 없습니다.";
			}
		}

		private void LoadPaymentsFromFile(string filePath)
		{
			try
			{
				StatusText.Text = "파일 로딩 중...";
				LoadButton.IsEnabled = false;
				RefreshButton.IsEnabled = false;

				_payments = _parser.ParsePaymentsFile(filePath);
				PaymentsDataGrid.ItemsSource = _payments;
				UpdateSummary();

				StatusText.Text = $"총 {_payments.Count}건의 결제 내역을 불러왔습니다.";
			}
			catch (Exception ex)
			{
				StatusText.Text = $"오류 발생: {ex.Message}";
				MessageBox.Show($"파일 로드 중 오류가 발생했습니다:\n{ex.Message}",
					"오류", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				LoadButton.IsEnabled = true;
				RefreshButton.IsEnabled = true;
				StatisticsButton.IsEnabled = _payments.Any();
			}
		}

		private void StatisticsButton_Click(object sender, RoutedEventArgs e)
		{
			if (_payments.Any())
			{
				try
				{
					var statisticsWindow = new OptimizedStatisticsWindow(_payments);
					statisticsWindow.Owner = this;
					statisticsWindow.ShowDialog();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"통계 창 열기 중 오류 발생: {ex.Message}",
						"오류", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else
			{
				MessageBox.Show("통계를 볼 데이터가 없습니다. 먼저 결제 내역 파일을 불러오세요.",
					"알림", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void UpdateSummary()
		{
			if (_payments == null || !_payments.Any())
			{
				TotalCountText.Text = "0건";
				TotalAmountText.Text = "₩0";
				AverageAmountText.Text = "₩0";
				return;
			}

			var totalCount = _payments.Count;
			var totalAmount = _payments.Where(p => p.Amount < 0).Sum(p => Math.Abs(p.Amount));
			var averageAmount = totalCount > 0 ? totalAmount / totalCount : 0;

			TotalCountText.Text = $"{totalCount:N0}건";
			TotalAmountText.Text = $"₩{totalAmount:N0}";
			AverageAmountText.Text = $"₩{averageAmount:N0}";
		}
	}
}