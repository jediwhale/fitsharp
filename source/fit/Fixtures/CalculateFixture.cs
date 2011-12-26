// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fit;
using fit.Model;
using fitlibrary.exception;
using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Exception;

namespace fitlibrary {

    public class CalculateFixture: DoFixtureBase {

        private CellRange methodSuffixCells;
        private Parse headerCells;
        private int expectedCount;
        private int myParameterCount;
        private bool IHaveNoteColumns;
        private ValueArray myValues;

        public CalculateFixture(): this(null) {}

        public CalculateFixture(object theSystemUnderTest): base(theSystemUnderTest) {}

        [Obsolete]
        public string ExceptionString { get; set; }

        public string RepeatString { get; set; }

        public override void DoRows(Parse theRows) {
            if (theRows == null) throw new TableStructureException("No header row.");
            ExamineTableStructure(theRows.Parts);
            if (expectedCount == 0) return;
            myValues = new ValueArray(RepeatString);
            base.DoRows(theRows.More);
        }

        public override void DoRow(Parse theRow) {
            try {
                CheckRowSize(theRow.Parts);

                for (int j = 0; j < expectedCount; j++) {
                    var memberCells = new List<Parse> { headerCells.At(j) };
                    foreach (Parse cell in methodSuffixCells.Cells) memberCells.Add(cell);

                    Parse expectedCell = theRow.Parts.At(myParameterCount + j + 1);

                    try {
                        Processor.Check(GetTargetObject(), new CellRange(memberCells),
                                            myValues.GetCells(new CellRange(theRow.Parts, myParameterCount).Cells),
                                            expectedCell);
                    }
                    catch (MemberMissingException e) {
                        TestStatus.MarkException(headerCells.At(j), e);
                        TestStatus.MarkIgnore(expectedCell);
                    }
                    catch (IgnoredException) {
                        TestStatus.MarkIgnore(expectedCell);
                    }
                    catch (Exception e) {
                        TestStatus.MarkException(expectedCell, e);
                    }
                }
            }
            catch (Exception e) {
                TestStatus.MarkException(theRow.Parts, e);
            }
        }

        private void CheckRowSize(Parse theCells) {
            int expectedSize = myParameterCount + expectedCount + 1;
            if (theCells.Size < expectedSize ||
                (!IHaveNoteColumns && theCells.Size != expectedSize)) {
                throw new RowWidthException(expectedSize);
            }
        }

        private static int CountParameterCells(Parse theHeaderCells) {
            Parse headerCell;
            int leadingCount = 0;
            for (headerCell = theHeaderCells; headerCell != null && headerCell.Text.Length > 0; headerCell = headerCell.More) {
                leadingCount++;
            }
            if (headerCell == null) {
                throw new TableStructureException("No calculated columns.");
            }
            for (; headerCell != null && headerCell.Text.Length == 0; headerCell = headerCell.More) {
                leadingCount++;
            }
            if (headerCell == null) throw new TableStructureException("No expected columns.");
            return leadingCount - 1;
        }

        private void ExamineTableStructure(Parse theHeaderCells) {
            expectedCount = 0;
            IHaveNoteColumns = false;
            try {
                myParameterCount = CountParameterCells(theHeaderCells);
                methodSuffixCells = new CellRange(theHeaderCells, myParameterCount);
                headerCells = theHeaderCells.At(myParameterCount + 1);
                foreach (Parse headerCell in new CellRange(theHeaderCells.At(myParameterCount + 1)).Cells) {
                    if (headerCell.Text.Length == 0) {
                        IHaveNoteColumns = true;
                        break;
                    }
                    expectedCount++;
                }
            }
            catch (Exception e) {
                TestStatus.MarkException(theHeaderCells, e);
            }
        }
    }
}
