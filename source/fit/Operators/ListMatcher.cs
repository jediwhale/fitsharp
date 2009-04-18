// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Collections.Generic;
using fit.Model;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public interface ListMatchStrategy {
        bool IsOrdered { get; }
        TypedValue[] ActualValues(Processor<Cell> processor, object theActualRow);
        bool IsExpectedSize(Parse theExpectedCells, object theActualRow);
        bool FinalCheck(TestStatus testStatus);
        bool SurplusAllowed {get;}
    }

    public class ListMatcher {
        private readonly Processor<Cell> processor;
        private readonly ListMatchStrategy strategy;

        public ListMatcher(Processor<Cell> processor, ListMatchStrategy strategy) {
            this.strategy = strategy;
            this.processor = processor;
        }

        public bool IsEqual(object theActualValue, Parse theExpectedValueCell) {
            Actuals actuals = new Actuals((IList)theActualValue, strategy);
            int expectedRow = 0;
            foreach (Parse currentRow in new CellRange(theExpectedValueCell.Parts.Parts.More).Cells) {
                int match = actuals.FindMatch(RowMatches, expectedRow, currentRow.Parts);
                if (match < 0 || (match != expectedRow && strategy.IsOrdered)) return false;
                expectedRow++;
            }
            return (actuals.UnmatchedCount == 0);
        }

        public bool MarkCell(Processor<Cell> processor, TestStatus testStatus, object systemUnderTest, object theActualValue, Parse theTableRows, Parse theRowsToCompare) {
            Actuals actuals = new Actuals((IList)theActualValue, strategy);
            if (theRowsToCompare == null && actuals.UnmatchedCount == 0) {
                testStatus.MarkRight(theTableRows);
            }
            bool result = true;
            int expectedRow = 0;
            foreach (Parse currentRow in new CellRange(theRowsToCompare).Cells) {
                try {
                    int match = actuals.FindMatch(RowMatches, expectedRow, currentRow.Parts);
                    if (match < 0) {
                        MarkAsIncorrect(testStatus, currentRow, "missing");
                        result = false;
                    }
                    expectedRow++;
                }
                catch (Exception e) {
                    testStatus.MarkException(currentRow.Parts, e);
                    return false;
                }
            }
            if (actuals.UnmatchedCount > 0 && !strategy.SurplusAllowed) {
                actuals.ShowSurplus(processor, testStatus, theTableRows.Last);
                result = false;
            }

            Parse markRow = theRowsToCompare;
            for (int row = 0; row < expectedRow; row++) {
                if (strategy.IsOrdered && actuals.IsOutOfOrder(row)) {
                    MarkAsIncorrect(testStatus, markRow, "out of order");
                    result = false;
                }
                else if (actuals.Match(row) != null) {
                    TypedValue[] actualValues = strategy.ActualValues(processor, actuals.Match(row));
                    int i = 0;
                    foreach (Parse cell in new CellRange(markRow.Parts).Cells) {
                        if (actualValues[i].Type != typeof(void) || cell.Text.Length > 0) {
                             new CellOperation(processor).Check(testStatus, systemUnderTest, actualValues[i], cell);

                        }
                        i++;
                    }
                }
                markRow = markRow.More;
            }

            if (!strategy.FinalCheck(testStatus)) return false;
            return result;
        }

        private static void MarkAsIncorrect(TestStatus testStatus, Parse theRow, string theReason) {
            Parse firstCell = theRow.Parts;
            firstCell.SetAttribute(CellAttributes.LabelKey, theReason);
            testStatus.MarkWrong(theRow);
        }

        private bool RowMatches(Parse theExpectedCells, object theActualRow) {
            if (!strategy.IsExpectedSize(theExpectedCells, theActualRow)) return false;
            Parse expectedCell = theExpectedCells;
            int column = 0;
            foreach (TypedValue actualValue in strategy.ActualValues(processor, theActualRow)) {
                if (actualValue.Type != typeof(void) || expectedCell.Text.Length > 0) {
                    if (!new CellOperation(processor).Compare(actualValue, expectedCell)) return false;
                }
                expectedCell = expectedCell.More;
                column++;
            }
            return true;
        }

        private class Actuals {
            public delegate bool Matches(Parse theExpectedCells, object theActual);
            public Actuals(IList theActualValues, ListMatchStrategy theStrategy) {
                myActuals = new List<ActualItem>(theActualValues.Count);
                foreach (object actualValue in theActualValues) {
                    myActuals.Add(new ActualItem(actualValue));
                }
                myStrategy = theStrategy;
                myUnmatchedCount = theActualValues.Count;
            }

            public int UnmatchedCount { get { return myUnmatchedCount; }}
            public object Match(int theIndex) {
                foreach (ActualItem item in myActuals) {
                    if (item.MatchRow == theIndex) return item.Value;
                }
                return null;
            }

            public int FindMatch(Matches theMatcher, int theExpectedRow, Parse theExpectedCells) {
                int result = -1;
                if (myActuals.Count == 0) return result;
                int lastMatched = -1;
                for (int row = 0; row < myActuals.Count; row++) {
                    if (myActuals[row].MatchRow != null) {
                        lastMatched = row;
                        continue;
                    }
                    if (result == -1 && theMatcher(theExpectedCells, myActuals[row].Value)) {
                        result = row;
                    }
                }
                if (result > -1) {
                    myActuals[result].MatchRow = theExpectedRow;
                    myUnmatchedCount--;
                } else {
                    myActuals.Insert(lastMatched + 1, new ActualItem(null, -1));
                }
                return result;
            }

            public void ShowSurplus(Processor<Cell> processor, TestStatus testStatus, Parse theLastRow) {
                Parse lastRow = theLastRow;
                for (int i = 0; i < myActuals.Count;) {
                    ActualItem surplus = myActuals[i];
                    if (surplus.MatchRow != null) {
                        i++;
                        continue;
                    }
                    Parse surplusRow = MakeSurplusRow(processor, testStatus, surplus.Value);
                    lastRow.More = surplusRow;
                    lastRow = surplusRow;
                    myActuals.RemoveAt(i);
                }
            }

            public bool IsOutOfOrder(int theExpectedRow) {
                return
                    (theExpectedRow < myActuals.Count && myActuals[theExpectedRow].MatchRow != theExpectedRow &&
                     myActuals[theExpectedRow].MatchRow != -1);
            }

            private Parse MakeSurplusRow(Processor<Cell> processor, TestStatus testStatus, object theSurplusRow) {
                Parse cells = null;
                foreach (TypedValue actualValue in myStrategy.ActualValues(processor, theSurplusRow)) {
                    var cell = (Parse) processor.Compose(actualValue.Value);
                    if (cells == null) {
                        cell.SetAttribute(CellAttributes.LabelKey, "surplus");
                        cells = cell;
                    }
                    else
                        cells.Last.More = cell;
                }
                Parse row = new Parse("tr", null, cells, null);
                testStatus.MarkWrong(row);
                return row;
            }

            private List<ActualItem> myActuals;
            private ListMatchStrategy myStrategy;
            private int myUnmatchedCount;

            private class ActualItem {
                public object Value;
                public int? MatchRow;
                public ActualItem(object theValue) {
                    Value = theValue;
                    MatchRow = null;
                }
                public ActualItem(object theValue, int theMatchRow) {
                    Value = theValue;
                    MatchRow = theMatchRow;
                }
            }
        }

    }
}