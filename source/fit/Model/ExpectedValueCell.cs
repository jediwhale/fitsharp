// FitNesse.NET
// Copyright © 2006-2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit;
using fit.Engine;
using fitSharp.Machine.Model;

namespace fitlibrary {

	public class ExpectedValueCell {

        public ExpectedValueCell(Parse theCell) {
            myCell = theCell;
        }

        public ExpectedValueCell(Parse theCell, string theExceptionKeyword): this(theCell) {
            myExceptionKeyword = theExceptionKeyword;
        }

        public void CompareTo(Fixture theFixture, object theActualValue) {
            CompareTo(theFixture, theActualValue, null);
        }

        public void CompareTo(Fixture theFixture, object theActualValue, Exception theActualException) {
            if (myExceptionKeyword != null && myCell.Text == myExceptionKeyword) {
                if (theActualException == null) {
                    theFixture.Wrong(myCell);
                }
                else {
                    theFixture.Right(myCell);
                }
            }
            else {
                if (theActualException != null) {
                    theFixture.Exception(myCell, theActualException);
                }
                else {
                    try {
                        MarkCell(theFixture, new TypedValue(theActualValue));
                    }
                    catch (Exception e) {
                        theFixture.Exception(myCell, e);
                    }
                }
            }
        }

        //todo: find a cellop reference?
        public bool IsEqual(TypedValue theActualValue) {
            return new CellOperation().Compare(theActualValue, myCell);
        }

        public void MarkCell(Fixture theFixture, TypedValue theActualValue) {
            theFixture.CellOperation.Check(theFixture, theActualValue, myCell);
        }

        private Parse myCell;
        private string myExceptionKeyword;
	}
}
