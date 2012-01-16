// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit;
using System;
using fit.Model;
using fitlibrary.exception;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Exception;

namespace fitlibrary {

	public class SetUpFixture: Fixture {
	    private Parse headerCells;

        public override void DoTable(Parse theTable) {
            SetUp();
            base.DoTable(theTable);
            TearDown();
        }

        public override void DoRows(Parse theRows) {
            if (theRows == null) throw new FitFailureException("No header row.");
            headerCells = theRows.Parts;
            base.DoRows(theRows.More);
        }

        public override void DoRow(Parse theRow) {
            try {
                if (theRow.Parts.Size != headerCells.Size)
                    throw new FitFailureException(String.Format("Row should be {0} cells wide.", headerCells.Size));
                Processor.ExecuteWithThrow(this, new CellRange(headerCells), new CellRange(theRow.Parts), theRow.Parts);
            }
            catch (MemberMissingException e) {
                TestStatus.MarkException(headerCells, e);
                throw new IgnoredException();
            }
            catch (Exception e) {
                TestStatus.MarkException(theRow.Parts, e);
            }
        }

        protected virtual void SetUp() {}
        protected virtual void TearDown() {}
	}
}
