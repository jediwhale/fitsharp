// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using fitSharp.Fit.Model;

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
			    cell.SetAttribute(CellAttributes.StatusKey, CellAttributes.FailStatus);
			}
			else
			{
			    cell.SetAttribute(CellAttributes.StatusKey, CellAttributes.PassStatus);
			}
		}
	}
}