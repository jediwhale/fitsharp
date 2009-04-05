// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit;
using fit.Model;
using fitlibrary.exception;
using fitSharp.Fit.Model;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitlibrary {

	public class ConstraintFixture: DoFixtureBase {

        private readonly bool expectedCondition;
	    private CellRange memberNameCells;
	    private int rowWidth;
        private ValueArray valueCells;

        public ConstraintFixture(): this(true, null) {}
        public ConstraintFixture(bool expectedCondition): this(expectedCondition, null) {}
        public ConstraintFixture(object systemUnderTest): this(true, systemUnderTest) {}
        public ConstraintFixture(bool expectedCondition, object systemUnderTest): base(systemUnderTest) {
            this.expectedCondition = expectedCondition;
        }

	    public string RepeatString { get; set; }

	    public override void DoRows(Parse rows) {
            memberNameCells = new CellRange(rows.Parts);
	        rowWidth = rows.Parts.Size;
            valueCells = new ValueArray(RepeatString);
            base.DoRows(rows.More);
        }

	    public override void DoRow(Parse row) {
	        if (row.Parts.Size != rowWidth) {
	            Exception(row.Parts, new RowWidthException(rowWidth));
	        }
	        else {
	            try {
	                TypedValue result = CellOperation.Invoke(this, memberNameCells,
	                                                         valueCells.GetCells(new CellRange(row.Parts).Cells));
	                if (result.Type != typeof (bool)) {
	                    throw new InvalidMethodException(string.Format("Method does not return boolean."));
	                }
	                if ((bool) result.Value == expectedCondition) {
	                    Right(row);
	                }
	                else {
	                    Wrong(row);
	                }
	            }
	            catch (ParseException<Cell> e) {
	                Exception((Parse)e.Subject, e.InnerException);
	            }
	        }
	    }
	}
}
