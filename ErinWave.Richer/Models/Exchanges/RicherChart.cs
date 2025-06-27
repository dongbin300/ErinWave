using ErinWave.Richer.Enums;

namespace ErinWave.Richer.Models.Exchanges
{
	/// <summary>
	/// 파일에 저장하는 객체: Transactions
	/// 실행 최초에 Transactions 불러와서 차트를 만들고
	/// 실행중에 계속 Transaction이 추가되면서 차트를 갱신하는 방식
	/// </summary>
	public class RicherChart
	{
		public List<RicherQuote> S1 { get; set; } = [];
		public List<RicherQuote> M1 { get; set; } = [];
		public List<RicherQuote> H1 { get; set; } = [];

		public RicherChart()
		{

		}

		public RicherChart(List<RicherTransaction> initTransactions)
		{
			S1 = [];
			var groupedByInterval = initTransactions.GroupBy(c => $"{c.Time.Year}-{c.Time.Month}-{c.Time.Day}-{c.Time.Hour}-{c.Time.Minute}-{c.Time.Second / 1}");
			foreach (var group in groupedByInterval)
			{
				var groupCharts = group.ToList();
				var firstChart = groupCharts.First();
				var lastChart = groupCharts.Last();

				S1.Add(new RicherQuote(
					new DateTime(firstChart.Time.Year, firstChart.Time.Month, firstChart.Time.Day, firstChart.Time.Hour, firstChart.Time.Minute, firstChart.Time.Second - (firstChart.Time.Second % 1)),
					firstChart.Price,
					groupCharts.Max(c => c.Price),
					groupCharts.Min(c => c.Price),
					lastChart.Price,
					groupCharts.Sum(c => c.Quantity)
					));
			}

			M1 = [];
			var groupedByIntervalm = initTransactions.GroupBy(c => $"{c.Time.Year}-{c.Time.Month}-{c.Time.Day}-{c.Time.Hour}-{c.Time.Minute}");
			foreach (var group in groupedByIntervalm)
			{
				var groupCharts = group.ToList();
				var firstChart = groupCharts.First();
				var lastChart = groupCharts.Last();

				S1.Add(new RicherQuote(
					new DateTime(firstChart.Time.Year, firstChart.Time.Month, firstChart.Time.Day, firstChart.Time.Hour, firstChart.Time.Minute, 0),
					firstChart.Price,
					groupCharts.Max(c => c.Price),
					groupCharts.Min(c => c.Price),
					lastChart.Price,
					groupCharts.Sum(c => c.Quantity)
					));
			}

			H1 = [];
			var groupedByIntervalh = initTransactions.GroupBy(c => $"{c.Time.Year}-{c.Time.Month}-{c.Time.Day}-{c.Time.Hour}");
			foreach (var group in groupedByIntervalh)
			{
				var groupCharts = group.ToList();
				var firstChart = groupCharts.First();
				var lastChart = groupCharts.Last();

				S1.Add(new RicherQuote(
					new DateTime(firstChart.Time.Year, firstChart.Time.Month, firstChart.Time.Day, firstChart.Time.Hour, 0, 0),
					firstChart.Price,
					groupCharts.Max(c => c.Price),
					groupCharts.Min(c => c.Price),
					lastChart.Price,
					groupCharts.Sum(c => c.Quantity)
					));
			}
		}

		public void Add(RicherTransaction transaction)
		{
			var time = transaction.Time;
			var price = transaction.Price;
			var quantity = transaction.Quantity;

			if (S1.Count == 0)
			{
				S1.Add(
					new RicherQuote(
						new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second),
						price,
						price,
						price,
						price,
						quantity
						)
					);

				M1.Add(
					new RicherQuote(
						new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0),
						price,
						price,
						price,
						price,
						quantity
						)
					);

				H1.Add(
					new RicherQuote(
						new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0),
						price,
						price,
						price,
						price,
						quantity
						)
					);
			}
			else
			{
				if (
					S1[^1].Time.Year == time.Year &&
					S1[^1].Time.Month == time.Month &&
					S1[^1].Time.Day == time.Day &&
					S1[^1].Time.Hour == time.Hour &&
					S1[^1].Time.Minute == time.Minute &&
					S1[^1].Time.Second == time.Second)
				{
					S1[^1].High = Math.Max(S1[^1].High, price);
					S1[^1].Low = Math.Min(S1[^1].Low, price);
					S1[^1].Close = price;
					S1[^1].Volume += quantity;
				}
				else
				{
					S1.Add(
					new RicherQuote(
						new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second),
						price,
						price,
						price,
						price,
						quantity
						)
					);
				}

				if (
					M1[^1].Time.Year == time.Year &&
					M1[^1].Time.Month == time.Month &&
					M1[^1].Time.Day == time.Day &&
					M1[^1].Time.Hour == time.Hour &&
					M1[^1].Time.Minute == time.Minute)
				{
					M1[^1].High = Math.Max(M1[^1].High, price);
					M1[^1].Low = Math.Min(M1[^1].Low, price);
					M1[^1].Close = price;
					M1[^1].Volume += quantity;
				}
				else
				{
					M1.Add(
					new RicherQuote(
						new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0),
						price,
						price,
						price,
						price,
						quantity
						)
					);
				}

				if (
					H1[^1].Time.Year == time.Year &&
					H1[^1].Time.Month == time.Month &&
					H1[^1].Time.Day == time.Day &&
					H1[^1].Time.Hour == time.Hour)
				{
					H1[^1].High = Math.Max(H1[^1].High, price);
					H1[^1].Low = Math.Min(H1[^1].Low, price);
					H1[^1].Close = price;
					H1[^1].Volume += quantity;
				}
			}
		}

		public List<RicherQuote> GetCharts(RicherInterval interval)
		{
			List<RicherQuote> sourceData;

			if ((int)interval < 60)
			{
				sourceData = S1;
			}
			else if ((int)interval < 3600)
			{
				sourceData = M1;
			}
			else
			{
				sourceData = H1;
			}

			var result = new List<RicherQuote>();

			var sourceDataSnapshot = sourceData.ToList();

			var groupedData = sourceDataSnapshot.GroupBy(quote =>
				quote.Time.Ticks / ((int)interval * TimeSpan.TicksPerSecond)
			);


			foreach (var group in groupedData)
			{
				var groupList = group.ToList();
				var first = groupList.First();
				var last = groupList.Last();

				result.Add(new RicherQuote(
					new DateTime(first.Time.Year, first.Time.Month, first.Time.Day,
								 first.Time.Hour, first.Time.Minute, first.Time.Second),
					first.Open,
					groupList.Max(q => q.High),
					groupList.Min(q => q.Low),
					last.Close,
					groupList.Sum(q => q.Volume)
				));
			}

			return result;
		}
	}
}
