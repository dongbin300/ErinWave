using System.Globalization;
using System.Windows.Data;

namespace ErinWave.Tests.WpfTest
{
	public class BarWidthMultiConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length < 2) return 0;

			// 첫 번째 값은 Quantity
			if (values[0] is decimal val && values[1] is decimal max && max > 0)
			{
				// 두 번째 값은 maxWidth (컨버터 외부에서 전달된 값)
				double maxWidth = values.Length > 2 && values[2] is double width ? width : 146; // 기본값은 146

				double ratio = (double)(val / max);
				return ratio * maxWidth;
			}
			return 0;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}
