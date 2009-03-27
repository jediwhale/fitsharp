// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitlibrary.exception;

namespace fit.Test.Acceptance {
    public class SpecifyFixture: Fixture {

        public override void DoTable(Parse theTable) {
            Parse embeddedTables = GetEmbeddedTables(theTable);
            Parse expectedCell = GetExpectedCell(theTable);

            SpecifyListener listener = new SpecifyListener();
            StoryTest storyTest = new StoryTest(embeddedTables, listener);
            storyTest.Execute();

            SetEmbeddedTables(theTable, listener.Tables);

            if (expectedCell != null) {
                FixtureTable actual = new FixtureTable(listener.Tables);
                FixtureTable expected = new FixtureTable(expectedCell.Parts);
                string differences = actual.Differences(expected);
                if (differences.Length == 0) {
                    Right(expectedCell);
                }
                else {
                    Wrong(expectedCell);
                    expectedCell.More = ParseNode.Fail(ParseNode.MakeCells(Escape(differences)));
                }
            }
        }

        private Parse GetEmbeddedTables(Parse theTable) {
            Parse secondRow = theTable.Parts.More;
            if (secondRow == null)
                throw new FitFailureException("No second row.");
            Parse embeddedTable = secondRow.Parts.Parts;
            if (!ParseNode.IsTable(embeddedTable))
                throw new FitFailureException("No embedded table.");
            return embeddedTable;
        }

        private void SetEmbeddedTables(Parse theParentTable, Parse theTables) {
            Parse secondRow = theParentTable.Parts.More;
            secondRow.Parts.Parts = theTables;
        }

        private Parse GetExpectedCell(Parse theTable) {
            Parse secondRow = theTable.Parts.More;
            Parse expectedCell = secondRow.Parts.More;
            if (expectedCell == null && secondRow.More != null) {
                expectedCell = secondRow.More.Parts;
            }
            if (expectedCell != null) {
                if (!ParseNode.IsTable(expectedCell.Parts))
                    throw new FitFailureException("No embedded table for expected results.");
            }
            return expectedCell;
        }

        private class SpecifyListener: FixtureListener {

            public void TablesFinished(Parse theTables, Counts counts) {}

            public void TableFinished(Parse finishedTable) {
                Parse newTable = ParseNode.Clone(finishedTable);
                if (myTables == null) {
                    myTables = newTable;
                }
                else {
                    myTables.Last.More = newTable;
                }
            }

            public Parse Tables {get {return myTables;}}

            private Parse myTables;

        }

    }
}