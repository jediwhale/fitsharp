// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit;
using System;
using fit.Model;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Exception;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitlibrary {

	public abstract class FlowFixtureBase: Fixture, FlowInterpreter {

	    public abstract MethodRowSelector MethodRowSelector { get; }

	    protected FlowFixtureBase() {}

	    protected FlowFixtureBase(object theSystemUnderTest): this() {
            mySystemUnderTest = theSystemUnderTest;
        }

        public virtual bool IsInFlow(int tableCount) { return tableCount == 1; }
        
	    public void DoSetUp(Tree<Cell> table) {
            ExecuteOptionalMethod(InvokeDirect.SetUpMethod, (Parse)table.Branches[0].Branches[0]);
	    }

	    public void DoTearDown(Tree<Cell> table) {
            ExecuteOptionalMethod(InvokeDirect.TearDownMethod, (Parse)table.Branches[0].Branches[0]);
	    }

	    protected void ProcessFlowRows(Parse table) {
            new InterpretFlow(1).DoTableFlow(Processor, this, table);
        }

        void ExecuteOptionalMethod(string theMethodName, Parse theCell) {
            try {
                Processor.Invoke(this, theMethodName, theCell); //todo: invokewiththrow?
            }
            catch (Exception e) { 
                TestStatus.MarkException(theCell, e);
            }
        }

        public object NamedFixture(string theName) {
            var symbol = new Symbol(theName);
            return Processor.Contains(symbol) ? Processor.Load(symbol).Instance : null;
        }

        public void AddNamedFixture(string name, object fixture) { Processor.Store(new Symbol(name, fixture)); }

        public void DoCheckOperation(Parse expectedValue, CellRange cells) {
            CellOperation.Check(GetTargetObject(),
                                        MethodRowSelector.SelectMethodCells(cells),
                                        MethodRowSelector.SelectParameterCells(cells),
                                        expectedValue);
        }

        public object ExecuteEmbeddedMethod(Parse theCells) {
            try {
                CellRange cells = CellRange.GetMethodCellRange(theCells, 0);
                return
                    CellOperation.Invoke(this,  MethodRowSelector.SelectMethodCells(cells), MethodRowSelector.SelectParameterCells(cells), theCells.More).
                        Value;
            }
            catch (ParseException<Cell> e) {
                TestStatus.MarkException(e.Subject, e.InnerException);
                throw new IgnoredException();
            }
        }

        public override object GetTargetObject() {
            return this;
        }
	}
}
