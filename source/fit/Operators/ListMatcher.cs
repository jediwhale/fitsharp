// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Linq;
using System.Collections.Generic;
using fit.Model;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Model;

namespace fit.Operators {

    public class ListMatcher {
        readonly CellProcessor processor;
        readonly ListMatchStrategy strategy;

        public ListMatcher(CellProcessor processor, ListMatchStrategy strategy) {
            this.strategy = strategy;
            this.processor = processor;
        }

        public bool IsEqual(IEnumerable<object> theActualValue, Tree<Cell> theExpectedValueCell) {
            var actuals = new Actuals(theActualValue, strategy);
            int expectedRow = 0;
            foreach (Parse currentRow in theExpectedValueCell.Branches[0].Branches.Skip(1)) {
                int match = actuals.FindMatch(RowMatches, expectedRow, currentRow);
                if (match < 0 || (match != expectedRow && strategy.IsOrdered)) return false;
                expectedRow++;
            }
            return (actuals.UnmatchedCount == 0);
        }

        public bool MarkCell(IEnumerable<object> theActualValue, Parse theTableRows) {
            var actuals = new Actuals(theActualValue, strategy);
            if (theTableRows.More == null && actuals.UnmatchedCount == 0) {
                processor.TestStatus.MarkRight(theTableRows);
            }
            bool result = true;
            int expectedRow = 0;
            foreach (Parse currentRow in new CellRange(theTableRows.More).Cells) {
                try {
                    int match = actuals.FindMatch(RowMatches, expectedRow, currentRow);
                    if (match < 0) {
                        MarkAsIncorrect(currentRow, "missing");
                        result = false;
                    }
                    expectedRow++;
                }
                catch (Exception e) {
                    processor.TestStatus.MarkException(currentRow.Parts, e);
                    return false;
                }
            }
            if (actuals.UnmatchedCount > 0 && !strategy.SurplusAllowed) {
                actuals.ShowSurplus(processor, theTableRows.Last);
                result = false;
            }

            Parse markRow = theTableRows.More;
            for (int row = 0; row < expectedRow; row++) {
                if (strategy.IsOrdered && actuals.IsOutOfOrder(row)) {
                    MarkAsIncorrect(markRow, "out of order");
                    result = false;
                }
                else if (actuals.Match(row) != null) {
                    TypedValue[] actualValues = strategy.ActualValues(actuals.Match(row));
                    int i = 0;
                    foreach (Parse cell in markRow.Branches) {
                        if (actualValues[i].Type != typeof(void) || cell.Text.Length > 0) {
                             processor.Check(actualValues[i], cell);

                        }
                        i++;
                    }
                }
                markRow = markRow.More;
            }

            if (!strategy.FinalCheck(processor.TestStatus)) return false;
            return result;
        }

        void MarkAsIncorrect(Parse theRow, string theReason) {
            Parse firstCell = theRow.Parts;
            firstCell.SetAttribute(CellAttribute.Label, theReason);
            processor.TestStatus.MarkWrong(theRow);
        }

        bool RowMatches(Tree<Cell> expectedCells, object theActualRow) {
            if (!strategy.IsExpectedSize(expectedCells.Branches.Count, theActualRow)) return false;
            int i = 0;
            foreach (TypedValue actualValue in strategy.ActualValues(theActualRow)) {
                if (actualValue.Type != typeof(void) || expectedCells.ValueAt(i).Text.Length > 0) {
                    if (!strategy.CellMatches(actualValue, expectedCells.Branches[i], i)) return false;
                }
                //expectedCell = expectedCell.More;
                i++;
            }
            return true;
        }

        class Actuals {
            readonly List<ActualItem> myActuals;
            readonly ListMatchStrategy myStrategy;

            public delegate bool Matches(Parse theExpectedCells, object theActual);
            public Actuals(IEnumerable<object> theActualValues, ListMatchStrategy theStrategy) {
                myActuals = new List<ActualItem>(theActualValues.Select(o => new ActualItem(o)));
                myStrategy = theStrategy;
                UnmatchedCount = myActuals.Count;
            }

            public int UnmatchedCount { get; private set; }

            public object Match(int theIndex) {
                return myActuals.Where(item => item.MatchRow == theIndex).Select(item => item.Value).FirstOrDefault();
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
                    UnmatchedCount--;
                } else {
                    myActuals.Insert(lastMatched + 1, new ActualItem(null, -1));
                }
                return result;
            }

            public void ShowSurplus(CellProcessor processor, Parse theLastRow) {
                Parse lastRow = theLastRow;
                for (int i = 0; i < myActuals.Count;) {
                    ActualItem surplus = myActuals[i];
                    if (surplus.MatchRow != null) {
                        i++;
                        continue;
                    }
                    Parse surplusRow = MakeSurplusRow(processor, surplus.Value);
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

            Parse MakeSurplusRow(CellProcessor processor, object theSurplusRow) {
                Parse cells = null;
                foreach (TypedValue actualValue in myStrategy.ActualValues(theSurplusRow)) {
                    var cell = (Parse) processor.Compose(actualValue.IsVoid ? new TypedValue(string.Empty) : actualValue);
                    if (cells == null) {
                        cell.SetAttribute(CellAttribute.Label, "surplus");
                        cells = cell;
                    }
                    else
                        cells.Last.More = cell;
                }
                var row = new Parse("tr", null, cells, null);
                processor.TestStatus.MarkWrong(row);
                return row;
            }

            class ActualItem {
                public readonly object Value;
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