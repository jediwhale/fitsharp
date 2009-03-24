// FitNesse.NET
// Copyright © 2006-2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit;
using System;
using System.Collections;
using System.Collections.Generic;
using fit.Engine;
using fit.exception;
using fit.Fixtures;
using fitlibrary.exception;
using fitSharp.Fit.Model;
using fitSharp.Machine.Application;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitlibrary {

	public abstract class FlowFixtureBase: Fixture {
	    protected FlowFixtureBase() {
            myNamedFixtures = new Hashtable();
        }

	    protected FlowFixtureBase(object theSystemUnderTest): this() {
            mySystemUnderTest = theSystemUnderTest;
        }

        public override  bool IsInFlow(int tableCount) { return tableCount == 1; }

        public virtual void DoFlowTable(Parse table) {
            if (TestStatus.IsAbandoned) return;
            ProcessFlowRows(table.Parts);
        }

        public void DoSetUp(Parse table) {
            ExecuteOptionalMethod("setup", table.Parts.Parts);
        }

        public void DoTearDown(Parse table) {
            ExecuteOptionalMethod("teardown", table.Parts.Parts);
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
                string specialActionName = Context.Configuration.GetItem<Service>().ParseTree<MemberName>(new CellRange(theCurrentRow.Parts, 1)).ToString();
                TypedValue result = Context.Configuration.GetItem<Service>().TryInvoke(new TypedValue(new Keywords(this)),
                                                                                  specialActionName, theCurrentRow.Parts);
                if (!result.IsValid) {
                    result = Context.Configuration.GetItem<Service>().TryInvoke(new TypedValue(this),
                                                                                 specialActionName, theCurrentRow.Parts);
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
                        Ignore(theCurrentRow);
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
	            Exception((Parse)e.Subject, e);
	        }
            catch (Exception e) {
                Exception(theCurrentRow.Parts, e);
            }
        }

        private void ExecuteOptionalMethod(string theMethodName, Parse theCell) {
            try {
                Context.Configuration.GetItem<Service>().TryInvoke(new TypedValue(this), theMethodName, new TreeLeaf<Cell>(null)); //todo: non-intuitive! use method name?
            }
            catch (Exception e) {
                Exception(theCell, e);
            }
        }

        public object NamedFixture(string theName) {
            return myNamedFixtures[theName];
        }

        protected abstract IEnumerable<Parse> MethodCells(CellRange theCells);
        protected abstract IEnumerable<Parse> ParameterCells(CellRange theCells);

        private static void AddCell(Parse theCells, object theNewValue) {
            theCells.Last.More = CellFactoryRepository.Instance.Make(theNewValue);
        }

        private void ColorMethodName(Parse theCells, bool thisIsRight) {
            foreach (Parse nameCell in MethodCells(new CellRange(theCells))) {
                ColorCell(nameCell, thisIsRight);
            }
        }

        private void ColorCell(Parse theCell, bool thisIsRight) {
            if (thisIsRight)
                Right(theCell);
            else
                Wrong(theCell);
        }

        public object ExecuteEmbeddedMethod(Parse theCells, int theExcludedCellCount) {
            try {
                CellRange cells = GetMethodCellRange(theCells, theExcludedCellCount);
                return
                    CellOperation.Invoke(this, new CellRange(MethodCells(cells)), new CellRange(ParameterCells(cells))).
                        Value;
            }
            catch (ParseException<Cell> e) {
                Exception((Parse)e.Subject, e.InnerException);
                throw new IgnoredException();
            }

        }

        private static CellRange GetMethodCellRange(Parse cells, int excludedCellCount) {
            Parse restOfTheRow = cells.More;
            if (restOfTheRow == null) {
                throw new FitFailureException("Missing cells for embedded method");
            }
            return new CellRange(restOfTheRow, restOfTheRow.Size - excludedCellCount);
        }

        private void ProcessRestOfTable(Fixture theFixture, Parse theRestOfTheRows) {
            Parse restOfTable = new Parse("table", "", theRestOfTheRows, null);
            theFixture.Prepare(this, restOfTable);
            try {
                StoryTest.DoTable(restOfTable, theFixture, false);
            }
            catch (Exception e) {
                Exception(theRestOfTheRows.Parts, e);
            }
        }

        public override object GetTargetObject() {
            return this;
		}

	    private static readonly IdentifierName ourWithIdentifier = new IdentifierName("with");

	    protected bool IHaveFinishedTable;
        protected Hashtable myNamedFixtures;

        private class Keywords {
            private readonly FlowFixtureBase fixture;

            public Keywords(FlowFixtureBase fixture) {
                this.fixture = fixture;
            }
            
	        public void AbandonStoryTest(Parse theCells) {
                fixture.Ignore(theCells);
                fixture.TestStatus.IsAbandoned = true;
                throw new AbandonStoryTestException();
            }

            public Fixture Calculate(Parse theCells) {
                return new CalculateFixture(fixture.SystemUnderTest ?? fixture);
            }

            public void Check(Parse theCells) {
                try {
                    CellRange cells = GetMethodCellRange(theCells, 1);
                    try {
                        fixture.CellOperation.Check(fixture,
                            new CellRange(fixture.MethodCells(cells)), 
                            new CellRange(fixture.ParameterCells(cells)),
                            theCells.Last);
                    }
                    catch (MemberMissingException e) {
                        fixture.Exception(theCells.More, e);
                    }
                    catch (Exception e) {
                        fixture.Exception(theCells.Last, e);
                    }
                }
                catch (IgnoredException) {}
            }
    	    
	        public void Set(Parse theCells) {
	            fixture.ExecuteEmbeddedMethod(theCells, 0);
	        }

            public CommentFixture Comment(Parse cells) {
                return new CommentFixture();
            }

            public Fixture Ignored(Parse cells) {
                return new Fixture();
            }

            public void Ensure(Parse theCells) {
                try {
                    fixture.ColorCell(theCells, (bool)fixture.ExecuteEmbeddedMethod(theCells, 0));
                }
                catch (IgnoredException) {}
                catch (Exception e) {
                    fixture.Exception(theCells, e);
                }
            }

            public void Not(Parse theCells) {
                try {
                    fixture.ColorCell(theCells, !(bool)fixture.ExecuteEmbeddedMethod(theCells, 0));
                }
                catch (IgnoredException) {}
                catch (Exception) {
                    fixture.Right(theCells);
                }
            }

	        public void Name(Parse theCells) {
	            Parse restOfTheCells = theCells.More;
	            if (restOfTheCells == null || restOfTheCells.More == null)
	                throw new TableStructureException("missing cells for name.");
	            fixture.myNamedFixtures.Add(
	                restOfTheCells.Text,
	                ourWithIdentifier.Equals(restOfTheCells.More.Text)
	                    ? new WithPhrase(restOfTheCells.More).Evaluate(fixture)
	                    : fixture.ExecuteEmbeddedMethod(restOfTheCells, 0));

	            fixture.Right(restOfTheCells);
	        }

            public void Note(Parse theCells) {}

            public void Reject(Parse theCells) {
                Not(theCells);
            }

            public void Show(Parse theCells) {
                try {
                    AddCell(theCells, fixture.ExecuteEmbeddedMethod(theCells, 0));
                }
                catch (IgnoredException) {}
            }

            public void Start(Parse theCells) {
                try {
                    fixture.SetSystemUnderTest(new WithPhrase(theCells).EvaluateNew(fixture));
                }
                catch (Exception e) {
                    fixture.Exception(theCells, e);
                }
            }

            public void With(Parse theCells) {
                fixture.SetSystemUnderTest(new WithPhrase(theCells).Evaluate(fixture));
            }
        }
    }
}
