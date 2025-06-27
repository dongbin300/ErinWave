using ErinWave.Richer.Enums;

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ErinWave.Richer.Converters
{
	public class OrderSideColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is OrderSide orderSideColor)
			{
				return orderSideColor switch
				{
					OrderSide.Buy => Common.LongColor,
					OrderSide.Sell => Common.ShortColor,
					_ => Brushes.White,
				};
			}
			return Brushes.Transparent;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
