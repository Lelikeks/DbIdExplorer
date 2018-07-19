using System;
using System.Collections.Generic;
using System.Linq;

namespace DbIdExplorer
{
	internal class ColumnItem
	{
		public string Name { get; set; }

		public int RowsCount { get; set; }
	}

	internal class TableItem
	{
		public string Schema { get; set; }

		public string Name { get; set; }

		public List<ColumnItem> Columns { get; set; }

		public int TotalRowsCount
		{
			get { return Columns.Sum(c => c.RowsCount); }
		}

		internal TableItem()
		{
			Columns = new List<ColumnItem>();
		}
	}
}
