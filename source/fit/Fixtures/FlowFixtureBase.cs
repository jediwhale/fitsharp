// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit;
using System;
using System.Collections;
using System.Collections.Generic;
using fit.Fixtures;
using fit.Model;
using fitSharp.Fit.Exception;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitlibrary {

	public abstract class FlowFixtureBase: Fixture, FlowInterpreter {

	    protected bool IHaveFinishedTable;
        protected Hashtable myNamedFixtures;

	    protected FlowFixtureBase() {
            myNamedFixtures = new Hashtable();
        }

	    protected FlowFixtureBase(object theSystemUnderTest): this() {
            mySystemUnderTest = theSystemUnderTest;
        }

        public override bool IsInFlow(int tableCount) { return tableCount == 1; }
        
	    public virtual void InterpretFlow(Tree<Cell> table) {
	        DoFlowTable((Parse)table.Value);
	    }

	    public virtual void DoFlowTable(Parse table) {
            if (TestStatus.IsAbandoned) return;
            ProcessFlowRows(table.Parts);
	    }

	    public void DoSetUp(Tree<Cell> table) {
	        var tableCell = (Parse) table.Value;
            ExecuteOptionalMethod(":setup", tableCell.Parts.Parts);
	    }

	    public void DoTearDown(Tree<Cell> table) {
	        var tableCell = (Parse) table.Value;
            ExecuteOptionalMethod(":teardown", tableCell.Parts.Parts);
	    }
        
        protected void ProcessFlowRows(Parse theRows) {
            Parse currentRow = theRows;
            IHaveFinishedTable = false;
            while (currentRow != null && !IHaveFinishedTable) {
                Parse nextRow = currentRow.More;
                ProcessFlowRow(currentRow);
                currentRow = nextRow;
            }
        }

        protected void ProcessFlowRow(Parse theCurrentRow) {
            try {
                string specialActionName = ":" +
                    Processor.Parse(typeof (MemberName), TypedValue.Void, new CellRange(theCurrentRow.Parts, 1)).ValueString;
                TypedValue result = Processor.TryInvoke(new TypedValue(new FlowKeywords(this)),
                                                                                  specialActionName, theCurrentRow.Parts);
                if (!result.IsValid) {
                    result = Processor.TryInvoke(new TypedValue(this), specialActionName, theCurrentRow.Parts);
                }
                if (!result.IsValid) {
                     result = CellOperation.TryInvoke(this,
                        new CellRange(MethodCells(new CellRange(theCurrentRow.Parts))),
                        new CellRange(ParameterCells(new CellRange(theCurrentRow.Parts))));

                }
                if (!result.IsValid) {
                    Fixture newFixture = null;
                    if (theCurrentRow.Parts.Text.Length > 0) {
                        try {
                            newFixture = LoadFixture(theCurrentRow.Parts.Text);
                        }
                        catch (Exception) {}
                    }
                    if (newFixture != null) {
                        ProcessRestOfTable(newFixture, theCurrentRow);
                        IHaveFinishedTable = true;
                    }
                    else {
                        result.ThrowExceptionIfNotValid();
                    }
                }
                else {

                    if (TestStatus.IsAbandoned) {
                        TestStatus.MarkIgnore(theCurrentRow);
                        return;
                    }
                    object wrapResult = FixtureResult.Wrap(result.Value);
                    if (wrapResult is bool) {
                        ColorMethodName(theCurrentRow.Parts, (bool) wrapResult);
                    }
                    else if (wrapResult is Fixture) {
                        ProcessRestOfTable((Fixture) wrapResult, theCurrentRow);
                        IHaveFinishedTable = true;
                        return;
                    }
                }
            }
            catch (IgnoredException) {}
	        catch (ParseException<Cell> e) {
	            TestStatus.MarkException(e.Subject, e);
	        }
            catch (Exception e) {
                TestStatus.MarkException(theCurrentRow.Parts, e);
            }
        }

        private void ExecuteOptionalMethod(string theMethodName, Parse theCell) {
            try {
                Processor.TryInvoke(new TypedValue(this), theMethodName, theCell);
            }
            catch (Exception e) {
                TestStatus.MarkException(theCell, e);
            }
        }

        public object NamedFixture(string theName) {
            return myNamedFixtures[theName];
        }

        public void AddNamedFixture(string name, object fixture) { myNamedFixtures.Add(name, fixture); }

        protected abstract IEnumerable<Parse> MethodCells(CellRange theCells);
        protected abstract IEnumerable<Parse> ParameterCells(CellRange theCells);

        private void ColorMethodName(Parse theCells, bool thisIsRight) {
            foreach (Parse nameCell in MethodCells(new CellRange(theCells))) {
                TestStatus.ColorCell(nameCell, thisIsRight);
            }
        }

        public  void DoCheckOperation(Parse expectedValue, CellRange cells) {
            CellOperation.Check(TestStatus, GetTargetObject(),
                                        new CellRange(MethodCells(cells)),
                                        new CellRange(ParameterCells(cells)),
                                        expectedValue);
        }

        public object ExecuteEmbeddedMethod(Parse theCells) {
            try {
                CellRange cells = CellRange.GetMethodCellRange(theCells, 0);
                return
                    CellOperation.Invoke(this, new CellRange(MethodCells(cells)), new CellRange(ParameterCells(cells))).
                        Value;
            }
            catch (ParseException<Cell> e) {
                TestStatus.MarkException(e.Subject, e.InnerException);
                throw new IgnoredException();
            }
        }

        private void ProcessRestOfTable(Interpreter theFixture, Parse theRestOfTheRows) {
            var restOfTable = new Parse("table", "", theRestOfTheRows, null);
            theFixture.Prepare(this, restOfTable);
            try {
                ExecuteStoryTest.DoTable(restOfTable, theFixture, false);
            }
            catch (Exception e) {
                TestStatus.MarkException(theRestOfTheRows.Parts, e);
            }
        }

        public override object GetTargetObject() {
            return this;
        }
	}
}
