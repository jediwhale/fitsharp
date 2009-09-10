// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Operators;
using fitlibrary;
using fitlibrary.exception;
using fitSharp.Fit.Model;

namespace fit.Test.Acceptance {
    public class SpecifyFixture: Fixture {
        private Parse resultTables;

        public override void DoTable(Parse theTable) {
            Parse embeddedTables = GetEmbeddedTables(theTable);
            Parse expectedCell = GetExpectedCell(theTable);

            var storyTest = new StoryTest(embeddedTables, SpecifyWriter);
            storyTest.Execute();

            SetEmbeddedTables(theTable, resultTables);

            if (expectedCell != null) {
                var actual = new FixtureTable(resultTables);
                var expected = new FixtureTable(expectedCell.Parts);
                string differences = actual.Differences(expected);
                if (differences.Length == 0) {
                    Right(expectedCell);
                }
                else {
                    Wrong(expectedCell);
                    expectedCell.More = ParseNode.MakeCells(Escape(differences));
                    expectedCell.More.SetAttribute(CellAttributes.StatusKey, CellAttributes.WrongStatus);
                }
            }
        }

        private static Parse GetEmbeddedTables(Parse theTable) {
            Parse secondRow = theTable.Parts.More;
            if (secondRow == null)
                throw new FitFailureException("No second row.");
            Parse embeddedTable = secondRow.Parts.Parts;
            if (!ParseNode.IsTable(embeddedTable))
                throw new FitFailureException("No embedded table.");
            return embeddedTable;
        }

        private static void SetEmbeddedTables(Parse theParentTable, Parse theTables) {
            Parse secondRow = theParentTable.Parts.More;
            secondRow.Parts.Parts = theTables;
        }

        private static Parse GetExpectedCell(Parse theTable) {
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

        private void SpecifyWriter(Parse theTables, TestStatus status) {
            for (Parse table = theTables; table != null; table = table.More) {
                Parse newTable = table.Copy();
                if (resultTables == null) {
                    resultTables = newTable;
                }
                else {
                    resultTables.Last.More = newTable;
                }
            }
        }
    }
}