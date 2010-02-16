// Copyright © 2010 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using fit.Model;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit
{
	public abstract class RowFixture : BoundFixture
	{
	    Parse headerCells;
		object targetObject;

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

		void EvaluateCellsInMatchingRow(Parse row, object match)
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
                    CellOperation.Check(GetTargetObject(), headerCell, cell);

                }
				cell = cell.More;
			}
		}

		void AddSurplusRows(Parse rows, ArrayList remaining)
		{
			foreach (object obj in remaining)
				AddSurplusRow(rows, obj);
		}

		void AddSurplusRow(Parse rows, object extraObject)
		{
			Parse cell = null;
			SetTargetObject(extraObject);
            foreach (Parse headerCell in new CellRange(headerCells).Cells)
			{
		        TypedValue actual = CellOperation.Invoke(this, headerCell);
                var newCell = (Parse)Processor.Compose(actual.Value ?? "null");
				if (cell == null)
					cell = newCell;
				else
					cell.Last.More = newCell;
			}
			AddRowToTable(cell, rows);
			MarkRowAsSurplus(rows.Last);
		}

		static void AddRowToTable(Parse cells, Parse rows)
		{
			rows.Last.More = new Parse("tr", null, cells, null);
		}

		object FindMatchingObject(ArrayList queryItems, Parse row)
		{
			return FindMatchingObject(queryItems, row, 0);
		}

		object FindMatchingObject(ArrayList queryItems, Parse row, int col)
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

		bool IsMatch(Parse row, int col)
		{
		    TypedValue actual = CellOperation.Invoke(this, headerCells.At(col));
		    return CellOperation.Compare(actual, GetCellForColumn(row, col));
		}

		static Parse GetCellForColumn(Parse row, int col)
		{
			Parse cell = row.Parts;
			for (int i = 0; i < col; i++)
				cell = cell.More;
			return cell;
		}

		bool ColumnHasBinding(int col)
		{
			return col < headerCells.Size;
		}

		static bool UniqueMatchFound(ICollection matches)
		{
			return matches.Count == 1;
		}

		static object UniqueMatch(IList matches)
		{
			return matches[0];
		}

		void MarkRowAsMissing(Parse row)
		{
			Parse cell = row.Parts;
			cell.SetAttribute(CellAttribute.Label, "missing");
			TestStatus.MarkWrong(cell);
		}

		void MarkRowAsSurplus(Parse row)
		{
			TestStatus.MarkWrong(row.Parts);
			row.Parts.SetAttribute(CellAttribute.Label, "surplus");
		}

		public override object GetTargetObject()
		{
			return targetObject;
		}

		void SetTargetObject(object obj)
		{
			targetObject = obj;
		}

		public abstract object[] Query(); // get rows to be compared
		public abstract Type GetTargetClass(); // get expected type of row

	}
}