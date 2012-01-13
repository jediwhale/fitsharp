// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Linq;
using System.Collections.Generic;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Engine {

    public class ListMatcher {
        readonly CellProcessor processor;
        readonly ListMatchStrategy strategy;

        public ListMatcher(CellProcessor processor, ListMatchStrategy strategy) {
            this.strategy = strategy;
            this.processor = processor;
        }

        public bool IsEqual(IEnumerable<object> theActualValue, Tree<Cell> theExpectedValueCell) {
            var actuals = new Actuals(theActualValue, strategy);
            var expectedRow = 0;
            foreach (var currentRow in theExpectedValueCell.Branches[0].Branches.Skip(1)) {
                int match = actuals.FindMatch(ExactMatch, expectedRow, currentRow);
                if (match < 0 || (match != expectedRow && strategy.IsOrdered)) return false;
                expectedRow++;
            }
            return (actuals.UnmatchedCount == 0);
        }

        public bool MarkCell(IEnumerable<object> theActualValue, Tree<Cell> table, int rowsToSkip) {
            var actuals = new Actuals(theActualValue, strategy);
            if (table.Branches.Count == rowsToSkip + 1 && actuals.UnmatchedCount == 0) {
                processor.TestStatus.MarkRight(table.ValueAt(rowsToSkip));
            }
            var result = true;
            var expectedRow = 0;
            foreach (var currentRow in table.Branches.Skip(rowsToSkip + 1)) {
                try {
                    var match = actuals.FindMatch(RowMatches, expectedRow, currentRow);
                    if (match < 0) {
                        MarkAsIncorrect(currentRow, "missing");
                        result = false;
                    }
                    expectedRow++;
                }
                catch (Exception e) {
                    processor.TestStatus.MarkException(currentRow.ValueAt(0), e);
                    return false;
                }
            }
            if (actuals.UnmatchedCount > 0 && !strategy.SurplusAllowed) {
                actuals.ShowSurplus(processor, table);
                result = false;
            }

            for (var row = 0; row < expectedRow; row++) {
                var markRow = table.Branches[rowsToSkip + row + 1];
                if (strategy.IsOrdered && actuals.IsOutOfOrder(row)) {
                    MarkAsIncorrect(markRow, "out of order");
                    result = false;
                }
                else if (actuals.Match(row) != null) {
                    var actualValues = strategy.ActualValues(actuals.Match(row));
                    int i = 0;
                    foreach (var cell in markRow.Branches) {
                        if (actualValues[i].Type != typeof(void) || cell.Value.Text.Length > 0) {
                             processor.Check(actualValues[i], cell);
                        }
                        i++;
                    }
                }
            }

            if (!strategy.FinalCheck(processor.TestStatus)) return false;
            return result;
        }

        void MarkAsIncorrect(Tree<Cell> theRow, string theReason) {
            var firstCell = theRow.ValueAt(0);
            firstCell.SetAttribute(CellAttribute.Label, theReason);
            processor.TestStatus.MarkWrong(theRow.Value);
        }

        int ExactMatch(Tree<Cell> expectedCells, object actualRow) {
            return RowMatches(expectedCells, actualRow) == expectedCells.Branches.Count ? 1 : 0;
        }

        int RowMatches(Tree<Cell> expectedCells, object theActualRow) {
            if (!strategy.IsExpectedSize(expectedCells.Branches.Count, theActualRow)) return 0;
            var i = 0;
            var matches = 0;
            var voids = 0;
            var mismatches = 0;
            foreach (var actualValue in strategy.ActualValues(theActualRow)) {
                if (actualValue.Type == typeof(void) && expectedCells.ValueAt(i).Text.Length == 0) {
                    voids++;
                }
                else if (strategy.CellMatches(actualValue, expectedCells.Branches[i], i)) {
                    matches++;
                }
                else {
                    mismatches++;
                }
                i++;
            }
            return matches == 0 && mismatches > 0 ? 0 : matches + voids;
        }

        class Actuals {
            readonly List<ActualItem> myActuals;
            readonly ListMatchStrategy myStrategy;

            public delegate int Matches(Tree<Cell> theExpectedCells, object theActual);

            public Actuals(IEnumerable<object> theActualValues, ListMatchStrategy theStrategy) {
                myActuals = new List<ActualItem>(theActualValues.Select(o => new ActualItem(o)));
                myStrategy = theStrategy;
                UnmatchedCount = myActuals.Count;
            }

            public int UnmatchedCount { get; private set; }

            public object Match(int theIndex) {
                return myActuals.Where(item => item.MatchRow == theIndex).Select(item => item.Value).FirstOrDefault();
            }

            public int FindMatch(Matches theMatcher, int theExpectedRow, Tree<Cell> theExpectedCells) {
                if (myActuals.Count == 0) return -1;

                var bestMatchScore = 0;
                var bestMatchRow = -1;

                for (var row = 0; row < myActuals.Count; row++) {
                    if (myActuals[row].MatchRow.HasValue) continue;
                    var matchScore = theMatcher(theExpectedCells, myActuals[row].Value);
                    if (matchScore <= bestMatchScore) continue;

                    bestMatchScore = matchScore;
                    bestMatchRow = row;
                }

                if (bestMatchScore <= 0) {
                    InsertPlaceholderForMissingRow();
                    return -1;
                }

                myActuals[bestMatchRow].MatchRow = theExpectedRow;
                UnmatchedCount--;
                return bestMatchRow;
            }

            void InsertPlaceholderForMissingRow() {
                var lastMatched = -1;
                for (var row = 0; row < myActuals.Count; row++) {
                    if (myActuals[row].MatchRow != null) lastMatched = row;
                }
                myActuals.Insert(lastMatched + 1, ActualItem.Missing);
            }

            public void ShowSurplus(CellProcessor processor, Tree<Cell> table) {
                for (var i = 0; i < myActuals.Count;) {
                    var surplus = myActuals[i];
                    if (surplus.MatchRow.HasValue) {
                        i++;
                        continue;
                    }
                    var surplusRow = MakeSurplusRow(processor, surplus.Value);
                    table.Add(surplusRow);
                    myActuals.RemoveAt(i);
                }
            }

            public bool IsOutOfOrder(int theExpectedRow) {
                return
                    (theExpectedRow < myActuals.Count && myActuals[theExpectedRow].MatchRow != theExpectedRow &&
                     myActuals[theExpectedRow].MatchRow != -1);
            }

            Tree<Cell> MakeSurplusRow(CellProcessor processor, object theSurplusRow) {
                var cells = new List<Tree<Cell>>();
                foreach (var actualValue in myStrategy.ActualValues(theSurplusRow)) {
                    var cell = processor.Compose(actualValue.IsVoid ? new TypedValue(string.Empty) : actualValue);
                    if (cells.Count == 0) {
                        cell.Value.SetAttribute(CellAttribute.Label, "surplus");
                    }
                    cells.Add(cell);
                }
                var row = processor.MakeCell(string.Empty, "tr", cells);
                processor.TestStatus.MarkWrong(row.Value);
                return row;
            }

            class ActualItem {
                public static readonly ActualItem Missing = new ActualItem(null, -1);

                public readonly object Value;
                public int? MatchRow;

                public ActualItem(object theValue) {
                    Value = theValue;
                    MatchRow = null;
                }

                ActualItem(object theValue, int theMatchRow) {
                    Value = theValue;
                    MatchRow = theMatchRow;
                }
            }
        }

    }
}