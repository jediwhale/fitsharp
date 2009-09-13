// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using fit.Model;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fit
{
	public abstract class RowFixture : BoundFixture
	{
	    private Parse headerCells;

	    public override void DoTable(Parse theTable) {
            if (theTable.Parts.More == null) TestStatus.MarkException(theTable.Parts.Parts, new ApplicationException("Header row missing."));
			else DoRows(theTable.Parts.More);
		}

		public override void DoRows(Parse rows)
		{
		    headerCells = rows.Parts;
			var queryResults = new ArrayList(Query());
			Parse row = rows;
			while ((row = row.More) != null)
			{
				object match = FindMatchingObject(queryResults, row);
				if (match == null)
				{
					MarkRowAsMissing(row);
				}
				else
				{
					EvaluateCellsInMatchingRow(row, match);
					queryResults.Remove(match);
				}
			}
			AddSurplusRows(rows, queryResults);
		}

		private void EvaluateCellsInMatchingRow(Parse row, object match)
		{
			SetTargetObject(match);
			Parse cell = row.Parts;
            foreach (Parse headerCell in new CellRange(headerCells).Cells)
			{
                if (cell == null) {
                    cell = new Parse("td", Label("missing"), null, null);
                    TestStatus.MarkWrong(cell);
                    row.Parts.Last.More = cell;
                }
                else {
                    CheckCalled();
                    CellOperation.Check(TestStatus, GetTargetObject(), headerCell, cell);

                }
				cell = cell.More;
			}
		}

		private void AddSurplusRows(Parse rows, ArrayList remaining)
		{
			foreach (object obj in remaining)
				AddSurplusRow(rows, obj);
		}

		private void AddSurplusRow(Parse rows, object extraObject)
		{
			Parse cell = null;
			SetTargetObject(extraObject);
            foreach (Parse headerCell in new CellRange(headerCells).Cells)
			{
		        TypedValue actual = CellOperation.Invoke(this, headerCell);
                var newCell = (Parse)Processor.Compose(new TypedValue(actual.Value ?? "null"));
				if (cell == null)
					cell = newCell;
				else
					cell.Last.More = newCell;
			}
			AddRowToTable(cell, rows);
			MarkRowAsSurplus(rows.Last);
		}

		private static void AddRowToTable(Parse cells, Parse rows)
		{
			rows.Last.More = new Parse("tr", null, cells, null);
		}

		private object FindMatchingObject(ArrayList queryItems, Parse row)
		{
			return FindMatchingObject(queryItems, row, 0);
		}

		private object FindMatchingObject(ArrayList queryItems, Parse row, int col)
		{
			if (!ColumnHasBinding(col))
				return null;
			var matches = new ArrayList();
			foreach (object queryItem in queryItems)
			{
				SetTargetObject(queryItem);
				if (IsMatch(row, col))
					matches.Add(queryItem);
			}
			if (UniqueMatchFound(matches))	
				return UniqueMatch(matches);
		    if (matches.Count > 0 && !ColumnHasBinding(col + 1))
		        return matches[0];
		    return FindMatchingObject(matches, row, col + 1);
		}

		private bool IsMatch(Parse row, int col)
		{
		    TypedValue actual = CellOperation.Invoke(this, headerCells.At(col));
		    return CellOperation.Compare(actual, GetCellForColumn(row, col));
		    //return new ExpectedValueCell(GetCellForColumn(row, col)).IsEqual(actual);
		}

		private static Parse GetCellForColumn(Parse row, int col)
		{
			Parse cell = row.Parts;
			for (int i = 0; i < col; i++)
				cell = cell.More;
			return cell;
		}

		private bool ColumnHasBinding(int col)
		{
			return col < headerCells.Size;
		}

		private static bool UniqueMatchFound(ICollection matches)
		{
			return matches.Count == 1;
		}

		private static object UniqueMatch(IList matches)
		{
			return matches[0];
		}

		private void MarkRowAsMissing(Parse row)
		{
			Parse cell = row.Parts;
			cell.SetAttribute(CellAttributes.LabelKey, "missing");
			TestStatus.MarkWrong(cell);
		}

		private void MarkRowAsSurplus(Parse row)
		{
			TestStatus.MarkWrong(row.Parts);
			row.Parts.SetAttribute(CellAttributes.LabelKey, "surplus");
		}

		public override object GetTargetObject()
		{
			return targetObject;
		}

		private void SetTargetObject(object obj)
		{
			targetObject = obj;
		}

		public abstract object[] Query(); // get rows to be compared
		public abstract Type GetTargetClass(); // get expected type of row

		private object targetObject;
	}
}