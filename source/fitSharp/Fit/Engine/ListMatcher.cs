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
        public ListMatcher(CellProcessor processor, ListMatchStrategy strategy) {
            this.strategy = strategy;
            this.processor = processor;
        }

        public bool IsEqual(IEnumerable<object> actualValues, Tree<Cell> expectedValueCell) {
            var actuals = new Actuals(actualValues);
            var expectedRowNumber = 0;
            foreach (var expectedRow in expectedValueCell.Branches[0].Branches.Skip(1)) {
                var matchRowNumber = actuals.FindMatch(ExactMatch, expectedRowNumber, expectedRow);
                if (matchRowNumber == Actuals.Unmatched || (matchRowNumber != expectedRowNumber && strategy.IsOrdered)) return false;
                expectedRowNumber++;
            }
            return (actuals.UnmatchedCount == 0);
        }

        public bool MarkCell(IEnumerable<object> actualValues, Tree<Cell> table, int rowsToSkip) {
            var actuals = new Actuals(actualValues);
            if (table.Branches.Count == rowsToSkip + 1 && actuals.UnmatchedCount == 0) {
                processor.TestStatus.MarkRight(table.ValueAt(rowsToSkip));
            }

            if (!FindMatches(table.Branches.Skip(rowsToSkip + 1), actuals)) return false;

            var rowCountBeforeSurplus = table.Branches.Count - rowsToSkip - 1;
            var result = true;

            if (actuals.UnmatchedCount > 0 && !strategy.SurplusAllowed) {
                actuals.ForSurplusValues(surplus => AddSurplusRow(surplus, table));
                result = false;
            }

            if (!MarkRows(table.Branches.Skip(rowsToSkip + 1).Take(rowCountBeforeSurplus), actuals)) result = false;

            return strategy.FinalCheck(processor.TestStatus) && result;
        }


        bool FindMatches(IEnumerable<Tree<Cell>> expectedRows, Actuals actuals) {
            var expectedRow = 0;
            foreach (var currentRow in expectedRows) {
                try {
                    actuals.FindMatch(BestMatch, expectedRow, currentRow);
                    expectedRow++;
                }
                catch (Exception e) {
                    processor.TestStatus.MarkException(currentRow.ValueAt(0), e);
                    return false;
                }
            }
            return true;
        }

        void AddSurplusRow(object surplusValue, Tree<Cell> table) {
            var cells = new List<Tree<Cell>>();
            foreach (var actualValue in strategy.ActualValues(surplusValue)) {
                var cell = processor.Compose(actualValue.IsVoid ? new TypedValue(string.Empty) : actualValue);
                if (cells.Count == 0) {
                    cell.Value.SetAttribute(CellAttribute.Label, "surplus");
                }
                cells.Add(cell);
            }
            var row = processor.MakeCell(string.Empty, "tr", cells);
            processor.TestStatus.MarkWrong(row.Value);
            table.Add(row);
        }

        bool MarkRows(IEnumerable<Tree<Cell>> expectedRows, Actuals actuals) {
            var markResult = true;
            var row = 0;
            foreach (var markRow in expectedRows) {
                if (strategy.IsOrdered && actuals.IsOutOfOrder(row)) {
                    MarkAsIncorrect(markRow, "out of order");
                    markResult = false;
                }
                else if (actuals.MatchValue(row) == null) {
                    MarkAsIncorrect(markRow, "missing");
                    markResult = false;
                }
                else {
                    var matchValues = strategy.ActualValues(actuals.MatchValue(row));
                    var cellNumber = 0;
                    foreach (var cell in markRow.Branches) {
                        if (matchValues[cellNumber].Type != typeof(void) || cell.Value.Text.Length > 0) {
                            processor.Check(matchValues[cellNumber], cell);
                        }
                        cellNumber++;
                    }
                }
                row++;
            }
            return markResult;
        }
        
        void MarkAsIncorrect(Tree<Cell> row, string reason) {
            row.ValueAt(0).SetAttribute(CellAttribute.Label, reason);
            processor.TestStatus.MarkWrong(row.Value);
        }

        int ExactMatch(Tree<Cell> expectedCells, object actualRow) {
            return BestMatch(expectedCells, actualRow) == expectedCells.Branches.Count ? 1 : 0;
        }

        int BestMatch(Tree<Cell> expectedCells, object theActualRow) {
            if (!strategy.IsExpectedSize(expectedCells.Branches.Count, theActualRow)) return 0;
            var cellNumber = 0;
            var matches = 0;
            var voids = 0;
            var mismatches = 0;
            foreach (var actualValue in strategy.ActualValues(theActualRow)) {
                if (actualValue.Type == typeof(void) && expectedCells.ValueAt(cellNumber).Text.Length == 0) {
                    voids++;
                }
                else if (strategy.CellMatches(actualValue, expectedCells.Branches[cellNumber], cellNumber)) {
                    matches++;
                }
                else {
                    mismatches++;
                }
                cellNumber++;
            }
            return matches == 0 && mismatches > 0 ? 0 : matches + voids;
        }

        readonly CellProcessor processor;
        readonly ListMatchStrategy strategy;

        class Actuals {
            public const int Unmatched = -1;

            public Actuals(IEnumerable<object> actualValues) {
                actualItems = new List<ActualItem>(actualValues.Select(o => new ActualItem(o)));
                UnmatchedCount = actualItems.Count;
            }

            public int UnmatchedCount { get; private set; }

            public object MatchValue(int rowNumber) {
                return actualItems.Where(item => item.MatchRow == rowNumber).Select(item => item.Value).FirstOrDefault();
            }

            public int FindMatch(Func<Tree<Cell>, object, int> matcher, int expectedRowNumber, Tree<Cell> expectedRow) {
                if (actualItems.Count == 0) return Unmatched;

                var bestMatchScore = 0;
                var bestMatchRow = Unmatched;

                for (var row = 0; row < actualItems.Count; row++) {
                    if (actualItems[row].MatchRow.HasValue) continue;
                    var matchScore = matcher(expectedRow, actualItems[row].Value);
                    if (matchScore <= bestMatchScore) continue;

                    bestMatchScore = matchScore;
                    bestMatchRow = row;
                }

                if (bestMatchScore <= 0) {
                    InsertPlaceholderForMissingRow();
                    return Unmatched;
                }

                actualItems[bestMatchRow].MatchRow = expectedRowNumber;
                UnmatchedCount--;
                return bestMatchRow;
            }

            void InsertPlaceholderForMissingRow() {
                var insertLocation = 0;
                for (var row = 0; row < actualItems.Count; row++) {
                    if (actualItems[row].MatchRow != null) insertLocation = row + 1;
                }
                actualItems.Insert(insertLocation, ActualItem.Missing);
            }

            public void ForSurplusValues(Action<object> handleSurplus) {
                for (var i = 0; i < actualItems.Count;) {
                    var surplus = actualItems[i];
                    if (surplus.MatchRow.HasValue) {
                        i++;
                    }
                    else {
                        handleSurplus(surplus.Value);
                        actualItems.RemoveAt(i);
                    }
                }
            }

            public bool IsOutOfOrder(int theExpectedRow) {
                return
                    (theExpectedRow < actualItems.Count && actualItems[theExpectedRow].MatchRow != theExpectedRow &&
                     actualItems[theExpectedRow].MatchRow != -1);
            }

            readonly List<ActualItem> actualItems;

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
