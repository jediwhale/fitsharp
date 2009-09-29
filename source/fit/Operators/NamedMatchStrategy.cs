// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Model;
using fitlibrary.exception;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public abstract class NamedMatchStrategy: ListMatchStrategy {
        protected NamedMatchStrategy(Parse theHeaderRow) {
            myHeaderRow = theHeaderRow;
        }

        public abstract bool SurplusAllowed {get;}

        public abstract bool IsOrdered { get;}

        public TypedValue[] ActualValues(CellProcessor processor, object theActualRow) {
            if (myColumnsUsed == null) myColumnsUsed = new bool[myHeaderRow.Parts.Size];
            var result = new TypedValue[myHeaderRow.Parts.Size];
            int column = 0;
            foreach (Parse headerCell in new CellRange(myHeaderRow.Parts).Cells) {
                TypedValue memberResult = new CellOperation(processor).TryInvoke(theActualRow, headerCell);
                if (memberResult.IsValid) {
                    result[column] = memberResult;
                    myColumnsUsed[column] = true;
                }
                else {
                    TypedValue itemResult = new CellOperation(processor).TryInvoke(theActualRow,
                                                                 new StringCellLeaf("getitem"),
                                                                 new CellRange(headerCell, 1));
                    if (itemResult.IsValid) {
                        result[column] = itemResult;
                        myColumnsUsed[column] = true;
                    }
                    else {
                        result[column] = TypedValue.Void;
                    }
                }
                column++;
            }
            return result;
        }

        public bool IsExpectedSize(Parse theExpectedCells, object theActualRow) {
            if (theExpectedCells.Size != myHeaderRow.Parts.Size) throw new RowWidthException(myHeaderRow.Parts.Size);
            return true;
        }

        public bool FinalCheck(TestStatus testStatus) {
            if (myColumnsUsed == null) return true;
            for (int column = 0; column < myHeaderRow.Parts.Size; column++) {
                if (!myColumnsUsed[column]) {
                    testStatus.MarkException(myHeaderRow.Parts.At(column),
                        new FitFailureException(String.Format("Column '{0}' not used.", myHeaderRow.Parts.At(column).Text)));
                    return false;
                }
            }
            return true;
        }

        private readonly Parse myHeaderRow;
        private bool[] myColumnsUsed;
    }
}
