// Modified or written by Object Mentor, Inc. for inclusion with FitNesse.
// Copyright (c) 2002 Cunningham & Cunningham, Inc.
// Released under the terms of the GNU General Public License version 2 or later.
// Copyright (c) 2002 Cunningham & Cunningham, Inc.
// Released under the terms of the GNU General Public License version 2 or later.

using System;
using System.Collections;

namespace fit
{
	public class SummaryFixture : Fixture
	{
		public static String countsKey = "counts";

		public override void DoTable(Parse table)
		{
			TestStatus.Summary[countsKey] = Counts.ToString();
			SortedList entries = new SortedList(TestStatus.Summary);
			table.Parts.More = Rows(entries.Keys.GetEnumerator());
		}

		protected virtual Parse Rows(IEnumerator keys)
		{
			if (keys.MoveNext())
			{
				object key = keys.Current;
				Parse result = TableRow(TableCell(key.ToString(), TableCell(TestStatus.Summary[key].ToString(), null)), Rows(keys));
				if (key.Equals(countsKey))
				{
					Mark(result);
				}
				return result;
			}
			return null;
		}

		protected virtual Parse TableRow(Parse parts, Parse more)
		{
			return new Parse("tr", null, parts, more);
		}

		protected virtual Parse TableCell(string body, Parse more)
		{
			return new Parse("td", Gray(body), null, more);
		}

		protected virtual void Mark(Parse row)
		{
			// mark summary good/bad without counting beyond here
			Parse cell = row.Parts.More;
			if (Counts.Wrong + Counts.Exceptions > 0)
			{
                cell.SetClass("fail");
			}
			else
			{
			    cell.SetClass("pass");
			}
		}
	}
}