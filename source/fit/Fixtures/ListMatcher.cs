// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Collections.Generic;
using fit;
using fitSharp.Machine.Model;

namespace fitlibrary {

    public interface ListMatchStrategy {
        bool IsOrdered { get; }
        TypedValue[] ActualValues(object theActualRow);
        bool IsExpectedSize(Parse theExpectedCells, object theActualRow);
        bool FinalCheck(Fixture fixture);
        bool SurplusAllowed {get;}
    }

    public class ListMatcher {

        public ListMatcher(ListMatchStrategy theStrategy) {
            myStrategy = theStrategy;
        }

	    public bool IsEqual(object theActualValue, Parse theExpectedValueCell) {
            Actuals actuals = new Actuals((IList)theActualValue, myStrategy);
	        int expectedRow = 0;
            foreach (Parse currentRow in new CellRange(theExpectedValueCell.Parts.Parts.More).Cells) {
                int match = actuals.FindMatch(RowMatches, expectedRow, currentRow.Parts);
                if (match < 0 || (match != expectedRow && myStrategy.IsOrdered)) return false;
                expectedRow++;
            }
            return (actuals.UnmatchedCount == 0);
        }

        public bool MarkCell(Fixture theFixture, object theActualValue, Parse theExpectedValueCell) {
            return MarkCell(theFixture, theActualValue, theExpectedValueCell.Parts.Parts,  theExpectedValueCell.Parts.Parts.More);
        }

        public bool MarkCell(Fixture theFixture, object theActualValue, Parse theTableRows, Parse theRowsToCompare) {
            Actuals actuals = new Actuals((IList)theActualValue, myStrategy);
            if (theRowsToCompare == null && actuals.UnmatchedCount == 0) {
                theFixture.Right(theTableRows);
            }
            bool result = true;
            int expectedRow = 0;
            foreach (Parse currentRow in new CellRange(theRowsToCompare).Cells) {
                try {
                    int match = actuals.FindMatch(RowMatches, expectedRow, currentRow.Parts);
                    if (match < 0) {
                        MarkAsIncorrect(theFixture, currentRow, "missing");
                        result = false;
                    }
                    expectedRow++;
                }
                catch (Exception e) {
                    theFixture.Exception(currentRow.Parts, e);
                    return false;
                }
            }
            if (actuals.UnmatchedCount > 0 && !myStrategy.SurplusAllowed) {
                actuals.ShowSurplus(theFixture, theTableRows.Last);
                result = false;
            }

            Parse markRow = theRowsToCompare;
            for (int row = 0; row < expectedRow; row++) {
                if (myStrategy.IsOrdered && actuals.IsOutOfOrder(row)) {
                    MarkAsIncorrect(theFixture, markRow, "out of order");
                    result = false;
                }
                else if (actuals.Match(row) != null) {
                    TypedValue[] actualValues = myStrategy.ActualValues(actuals.Match(row));
                    int i = 0;
                    foreach (Parse cell in new CellRange(markRow.Parts).Cells) {
                        if (actualValues[i].Type != typeof(void) || cell.Text.Length > 0) {
                            ExpectedValueCell expected = new ExpectedValueCell(cell);
                            expected.MarkCell(theFixture, actualValues[i]);
                        }
                        i++;
                    }
                }
                markRow = markRow.More;
            }

            if (!myStrategy.FinalCheck(theFixture)) return false;
            return result;
	    }

        private void MarkAsIncorrect(Fixture theFixture, Parse theRow, string theReason) {
            Parse firstCell = theRow.Parts;
            firstCell.AddToBody(Fixture.Label(theReason));
            theFixture.Wrong(theRow);
        }

        private bool RowMatches(Parse theExpectedCells, object theActualRow) {
            if (!myStrategy.IsExpectedSize(theExpectedCells, theActualRow)) return false;
            Parse expectedCell = theExpectedCells;
            int column = 0;
            foreach (TypedValue actualValue in myStrategy.ActualValues(theActualRow)) {
                if (actualValue.Type != typeof(void) || expectedCell.Text.Length > 0) {
                    if (!(new ExpectedValueCell(expectedCell)).IsEqual(actualValue)) return false;
                }
                expectedCell = expectedCell.More;
                column++;
            }
            return true;
        }

        private ListMatchStrategy myStrategy;

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

            public void ShowSurplus(Fixture theFixture, Parse theLastRow) {
                Parse lastRow = theLastRow;
                for (int i = 0; i < myActuals.Count;) {
                    ActualItem surplus = myActuals[i];
                    if (surplus.MatchRow != null) {
                        i++;
                        continue;
                    }
                    Parse surplusRow = MakeSurplusRow(theFixture, surplus.Value);
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

            private Parse MakeSurplusRow(Fixture theFixture, object theSurplusRow) {
                Parse cells = null;
                foreach (TypedValue actualValue in myStrategy.ActualValues(theSurplusRow)) {
                    Parse cell = CellFactoryRepository.Instance.Make(actualValue.Value, CellFactoryRepository.Grey);
                    if (cells == null) {
                        cell.AddToBody(Fixture.Label("surplus"));
                        cells = cell;
                    }
                    else
                        cells.Last.More = cell;
                }
                Parse row = new Parse("tr", null, cells, null);
                theFixture.Wrong(row);
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
