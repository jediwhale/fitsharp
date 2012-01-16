// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitlibrary.exception;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public abstract class NamedMatchStrategy: CellMatcher, ListMatchStrategy {
        protected readonly Tree<Cell> myHeaderRow;
        bool[] myColumnsUsed;

        protected NamedMatchStrategy(CellProcessor processor, Tree<Cell> theHeaderRow): base(processor) {
            myHeaderRow = theHeaderRow;
        }

        public abstract bool SurplusAllowed {get;}

        public abstract bool IsOrdered { get;}

        public TypedValue[] ActualValues(object theActualRow) {
            if (myColumnsUsed == null) myColumnsUsed = new bool[myHeaderRow.Branches.Count];
            var result = new TypedValue[myHeaderRow.Branches.Count];
            int column = 0;
            foreach (Parse headerCell in myHeaderRow.Branches) {
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

        TypedValue InvokeIndexerWithRawHeaderValue(object theActualRow, Tree<Cell> headerCell) {
            return Processor.Execute(theActualRow, new CellTreeLeaf("getitem"), new CellTree(headerCell));
        }

        public bool IsExpectedSize(int expectedSize, object theActualRow) {
            if (expectedSize != myHeaderRow.Branches.Count) throw new RowWidthException(myHeaderRow.Branches.Count);
            return true;
        }

        public bool FinalCheck(TestStatus testStatus) {
            if (myColumnsUsed == null) return true;
            for (int column = 0; column < myHeaderRow.Branches.Count; column++) {
                if (myColumnsUsed[column]) continue;
                testStatus.MarkException(myHeaderRow.Branches[column].Value,
                                         new FitFailureException(String.Format("Column '{0}' not used.", myHeaderRow.Branches[column].Value.Text)));
                return false;
            }
            return true;
        }
    }
}
