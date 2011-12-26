// Copyright © 2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit;
using fit.Model;
using fitlibrary.exception;
using fitSharp.Fit.Model;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitlibrary {

	public class CombinationFixture: DoFixtureBase {
        static readonly Tree<Cell> combineMember = new CellTreeLeaf("combine");
	    Parse firstRowCells;

        public CombinationFixture(): this(null) {}

        public CombinationFixture(object theSystemUnderTest): base(theSystemUnderTest) {}

        public override void DoRows(Parse theRows) {
            firstRowCells = theRows.Parts.More;
            if (firstRowCells == null) return;
            base.DoRows(theRows.More);
        }

	    public override void DoRow(Parse theRow) {
	        if (theRow.Parts.More.Size != firstRowCells.Size) {
	            TestStatus.MarkException(theRow.Parts, new RowWidthException(firstRowCells.Size));
	            return;
	        }
	        Parse headerCell = firstRowCells;
	        foreach (Parse expectedValueCell in new CellRange(theRow.Parts.More).Cells) {
	            try {
	                Processor.Check(GetTargetObject(), combineMember,
	                                    new CellTree(theRow.Parts, headerCell),
	                                    expectedValueCell);
	            }
	            catch (IgnoredException) {
	                TestStatus.MarkIgnore(expectedValueCell);
	            }
	            catch (Exception e) {
	                TestStatus.MarkException(expectedValueCell, e);
	            }
	            headerCell = headerCell.More;
	        }
	    }
    }
}
