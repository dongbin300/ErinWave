//-----------------------------------------------------------------------
//
// MIT License
//
// Copyright (c) 2025 Erin Wave
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//-----------------------------------------------------------------------

using ErinWave.Extensions;

using System.Data;

namespace ErinWave.Collections
{
	public class DataSource
	{
		public DataTable table;
		public DataView Data => table.DefaultView;

		public DataSource()
		{
			table = new DataTable();
		}

		public DataSource(params string[] columns)
		{
			table = new DataTable();
			AddColumns(columns);
		}

		public DataSource(IEnumerable<object> obj)
		{
			table = obj.ToDataTable();
		}

		public DataSource(string csvPath)
		{
			table = new DataTable();
			var data = IO.ErinWaveFile.ReadToArray(csvPath);
			AddColumns(data[0].Split(',').Select(x => x.Replace('ꪪ', ',')).ToArray());
			for (int i = 1; i < data.Length; i++)
			{
				var items = data[i].Split(',').Select(x => x.Replace('ꪪ', ',')).ToArray();
				AddRow(items);
			}
		}

		public void AddColumns(params string[] columns)
		{
			foreach (string col in columns)
			{
				table.Columns.Add(col, typeof(string));
			}
		}

		public void AddRow(params string[] items)
		{
			table.Rows.Add(items);
		}

		public void SaveCsvFile(string path)
		{
			List<string> contents = [string.Join(',', table.Columns.Cast<DataColumn>().Select(c => c.ColumnName.Replace(',', 'ꪪ')).ToArray())];
			foreach (DataRow row in table.Rows)
			{
				contents.Add(string.Join(',', row.ItemArray.Cast<string>().Select(r => r.Replace(',', 'ꪪ')).ToArray()));
			}
			IO.ErinWaveFile.WriteByArray(path, contents);
		}
	}
}
