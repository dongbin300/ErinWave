using System.Globalization;
using System.Windows.Data;

namespace ErinWave.Tests.WpfTest
{
	public class BarWidthConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values[0] is int quantity && values[1] is int maxQuantity)
			{
				return (double)quantity / maxQuantity * 100;
			}
			return 0;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
