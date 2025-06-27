using ErinWave.Richer.Maths;

using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ErinWave.Tests.WpfTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		decimal QuantityMax;

		public MainWindow()
		{
			InitializeComponent();

			var dataTable = new System.Data.DataTable();
			dataTable.Columns.Add("Price", typeof(decimal));
			dataTable.Columns.Add("Quantity", typeof(decimal));

			dataTable.Rows.Add(1.0m, 10.0m);
			dataTable.Rows.Add(2.0m, 20.0m);
			dataTable.Rows.Add(3.0m, 30.0m);
			dataTable.Rows.Add(4.0m, 40.0m);
			dataTable.Rows.Add(5.0m, 50.0m);

			DataGrid1.ItemsSource = dataTable.DefaultView;
			QuantityMax = dataTable.AsEnumerable()
						   .Select(row => row.Field<decimal>("Quantity"))
						   .Max();	
			DataContext = this;
		}
	}
}