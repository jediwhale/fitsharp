// Copyright © 2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Model;
using fitlibrary.exception;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public abstract class NamedMatchStrategy: CellMatcher, ListMatchStrategy {
        protected readonly Parse myHeaderRow;
        bool[] myColumnsUsed;

        protected NamedMatchStrategy(CellProcessor processor, Parse theHeaderRow): base(processor) {
            myHeaderRow = theHeaderRow;
        }

        public abstract bool SurplusAllowed {get;}

        public abstract bool IsOrdered { get;}

        public TypedValue[] ActualValues(object theActualRow) {
            if (myColumnsUsed == null) myColumnsUsed = new bool[myHeaderRow.Parts.Size];
            var result = new TypedValue[myHeaderRow.Parts.Size];
            int column = 0;
            foreach (Parse headerCell in new CellRange(myHeaderRow.Parts).Cells) {
                TypedValue actual = InvokeMethod(theActualRow, headerCell);
                if (!actual.IsValid) {
                    actual = InvokeIndexerWithRawHeaderValue(theActualRow, headerCell);
                }
                if (actual.IsValid) {
                    result[column] = actual;
                    myColumnsUsed[column] = true;
                }
                else {
                    result[column] = TypedValue.Void;
                }
                column++;
            }
            return result;
        }

        TypedValue InvokeMethod(object theActualRow, Tree<Cell> headerCell) {
            return Processor.Execute(theActualRow, headerCell, new CellTree());
        }

        TypedValue InvokeIndexerWithRawHeaderValue(object theActualRow, Parse headerCell) {
            return Processor.Execute(theActualRow,
                                                              new CellTreeLeaf("getitem"),
                                                              new CellRange(headerCell, 1));
        }

        public bool IsExpectedSize(Parse theExpectedCells, object theActualRow) {
            if (theExpectedCells.Size != myHeaderRow.Parts.Size) throw new RowWidthException(myHeaderRow.Parts.Size);
            return true;
        }

        public bool FinalCheck(TestStatus testStatus) {
            if (myColumnsUsed == null) return true;
            for (int column = 0; column < myHeaderRow.Parts.Size; column++) {
                if (myColumnsUsed[column]) continue;
                testStatus.MarkException(myHeaderRow.Parts.At(column),
                                         new FitFailureException(String.Format("Column '{0}' not used.", myHeaderRow.Parts.At(column).Text)));
                return false;
            }
            return true;
        }
    }
}
