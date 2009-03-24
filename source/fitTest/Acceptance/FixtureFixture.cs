// Copyright (c) 2003 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

using fitlibrary.exception;
using System;

namespace fit.Test.Acceptance {
    public class FixtureFixture: Fixture {

        protected const string REPORT = "report";
        protected const string INSERT_ROW = "I";

        protected static Counts ourEmbeddedCounts;
        protected static StoryTest ourEmbeddedStoryTest;

        protected Parse myEmbeddedRow;
        protected Parse myEmbeddedTable;

        public FixtureFixture() {
            ourEmbeddedCounts = new Counts();
            ourEmbeddedStoryTest = new StoryTest();
            ourEmbeddedStoryTest.TestStatus.Summary["run date"] = DateTime.Now;
            ourEmbeddedStoryTest.TestStatus.Summary["run elapsed time"] = new RunTime();
        }

        public override void DoTable(Parse theTable) {
            //FixtureFactory original = FixtureFactory;
            ProcessFirstTable(theTable);
            //FixtureFactory = original;
        }

        protected void ProcessFirstTable(Parse theTable) {
            try {
                base.DoTable(theTable);
            } catch (FitFailureException ex) {
                Failure(theTable.At(0, 0, 0), ex.Message);
            } catch (IgnoredException) {
            }
        }

        public override void DoRows(Parse theRows) {
            ProcessEmbeddedRows(theRows, theRows.More, GetFirstEmbeddedRow(theRows.Parts));
        }

        protected void ProcessEmbeddedRows(Parse theRows, Parse theBodyRows, Parse theNewHeaderRow) {
            Parse embeddedRows = ExtractEmbeddedRows(theBodyRows);
            if (theNewHeaderRow != null) {
                theNewHeaderRow.More = embeddedRows;
                embeddedRows = theNewHeaderRow;
            }
            myEmbeddedTable = new Parse("table", "", embeddedRows, null);
            DoEmbeddedTable(myEmbeddedTable);
            if (SplicedInAddedRows(theRows, myEmbeddedTable.Parts))
                CheckRowMarkings(theBodyRows);
            else
                ShowActualRowsAtBottom(theRows, myEmbeddedTable.Parts);
        }

        private Parse ExtractEmbeddedRows(Parse theRows) {
            Parse result = null;
            for (Parse row = theRows; row != null; row = row.More) {
                Parse firstCell = row.Parts;
                if (firstCell.Text == REPORT || firstCell.More == null || firstCell.Text.StartsWith(INSERT_ROW))
                    continue;
                Parse newRow = new Parse("<tr>", "", firstCell.More, null);
                if (result == null)
                    result = newRow;
                else
                    result.Last.More = newRow;
            }
            return result;
        }

        protected void ShowActualRowsAtBottom(Parse theGivenRows, Parse theEmbeddedRows) {
            Parse rows = theGivenRows;
            Parse embeddedRows = theEmbeddedRows;
            while (rows.More != null)
                rows = rows.More;
            rows.More = new Parse("tr",null,new Parse("td","Actual Rows:",null,null),null);
            rows = rows.More;
            Wrong(rows.Parts);
            while (embeddedRows != null) {
                rows.More = new Parse("tr",null,new Parse("td","",null,embeddedRows.Parts),null);
                rows = rows.More;
                embeddedRows = embeddedRows.More;
            }
        }

        protected bool SplicedInAddedRows(Parse theOuterRows, Parse theEmbeddedRows) {
            Parse outerRows = theOuterRows;
            Parse embeddedRows = theEmbeddedRows;
            while (outerRows != null && embeddedRows != null) {
                if (outerRows.Parts.Text.StartsWith(INSERT_ROW)) {
                    if (outerRows.Parts.More != null && !ValidRowValues(outerRows.Parts.More, embeddedRows.Parts))
                        return false;
                    outerRows.Parts.More = embeddedRows.Parts;
                    embeddedRows = embeddedRows.More;
                }
                else if (outerRows.Parts.Text == REPORT) {}
                    // Do nothing; it will be picked up on the next pass
                else if (outerRows.Parts.More == null)
                    return false;
                else if (outerRows.Parts.More == embeddedRows.Parts) // They match, so OK
                    embeddedRows = embeddedRows.More;
                else // Don't match, so wrong
                    return false;
                outerRows = outerRows.More;
            }
            while (outerRows != null && outerRows.Parts.Text == REPORT)
                outerRows = outerRows.More;
            if (outerRows != null || embeddedRows != null)
                return false;
            return true;
        }

        private bool ValidRowValues(Parse theExpected, Parse theActual) {
            Parse expected = theExpected;
            Parse actual = theActual;
            while (expected != null && actual != null) {
                if (expected.Text.Length != 0) {
                    if (expected.Text != actual.Text.Trim()) // there's a leading space in actual surplus line - not sure why...
                        return false;
                }
                expected = expected.More;
                actual = actual.More;
            }
            return expected == null && actual == null;
        }

        private Parse GetFirstEmbeddedRow(Parse theCells) {
            if (theCells == null)
                throw new FitFailureException("Embedded fixture is missing");
            if (theCells.Text == "fixture" && theCells.More != null)
                return new Parse("<tr>", "", theCells.More, null);
            else {
                Failure(theCells, "Embedded fixture is missing");
                throw new IgnoredException();
            }
        }

        protected void DoEmbeddedTable(Parse theTable) {
            Parse heading = theTable.At(0, 0, 0);
            if (heading != null) {
                try {
                    //Fixture fixture = FixtureFactory.LoadFixture(theTable);
                    Fixture fixture = LoadFixture(heading.Text);
                    //(Fixture) (Class.forName(heading.text()).newInstance());
                    RunFixture(theTable, fixture);
                    //			} catch (ClassNotFoundException ex) {
                    //				failure(heading, ": Unknown Class");
                } catch (Exception e) {
                    Exception(heading, e);
                }
            }
        }

        private void RunFixture(Parse theTables, Fixture theFixture) {
            theFixture.TestStatus = ourEmbeddedStoryTest.TestStatus;
            theFixture.DoTable(theTables);
        }

        public void Failure(Parse theCell, string theMessage) {
            Wrong(theCell);
            theCell.AddToBody(Label(theMessage));
        }

        protected void CheckRowMarkings(Parse theRows) {
            Parse rows = theRows;
            Parse previousRow = null;
            while (rows != null) {
                Parse cells = rows.Parts;
                if (cells.Text == INSERT_ROW) {}
                else if (cells.Text == REPORT && previousRow != null) {
                    if (ValidReportValuesMarked(cells.More, previousRow.Parts.More))
                        Right(cells);
                    else
                        Wrong(cells);
                } else {
                    string result = CellMarkings(cells.More);
                    if (MarkingsEqual(Unexclaim(cells.Text), result))
                        Right(cells);
                    else
                        Wrong(cells, result);
                }
                previousRow = rows;
                rows = rows.More;
            }
        }

        private string Unexclaim(string theText) {
            if (theText.StartsWith(INSERT_ROW))
                return theText.Substring(1);
            return theText;
        }

        private bool ValidReportValuesMarked(Parse theExpected, Parse theActual) {
            Parse expected = theExpected;
            Parse actual = theActual;
            bool result = true;
            while (expected != null && actual != null) {
                if (expected.Text.Length != 0) {
                    if (expected.Text == actual.Text.Trim()) // actual may have leading space if marked, not sure why
                        Right(expected);
                    else {
                        Wrong(expected,actual.Text);
                        result = false;
                    }
                }
                expected = expected.More;
                actual = actual.More;
            }
            return result;
        }

        protected string CellMarkings(Parse theCells) {
            Parse cells = theCells;
            string result = "";
            while (cells != null) {
                result += Coloring.GetColor(cells);
                cells = cells.More;
            }
            return result;
        }

        private bool MarkingsEqual(string theExpected, string theActual) {
            if (theExpected == theActual)
                return true;
            string trimmedActual = theActual;
            while (trimmedActual.EndsWith("-")) {
                trimmedActual =
                    trimmedActual.Substring(0, trimmedActual.Length - 1);
                if (theExpected == trimmedActual)
                    return true;
            }
            return false;
        }
    }
}