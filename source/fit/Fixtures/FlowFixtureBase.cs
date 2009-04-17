// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit;
using System;
using System.Collections;
using System.Collections.Generic;
using fit.exception;
using fit.Fixtures;
using fit.Model;
using fitlibrary.exception;
using fitSharp.Fit.Exception;
using fitSharp.Fit.Model;
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
                string specialActionName = Processor.ParseTree<MemberName>(new CellRange(theCurrentRow.Parts, 1)).ToString();
                TypedValue result = Processor.TryInvoke(new TypedValue(new Keywords(this, TestStatus)),
                                                                                  specialActionName, theCurrentRow.Parts);
                if (!result.IsValid) {
                    result = Processor.TryInvoke(new TypedValue(this),
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
                Processor.TryInvoke(new TypedValue(this), theMethodName, new TreeLeaf<Cell>(null)); //todo: non-intuitive! use method name?
            }
            catch (Exception e) {
                TestStatus.MarkException(theCell, e);
            }
        }

        public object NamedFixture(string theName) {
            return myNamedFixtures[theName];
        }

        protected abstract IEnumerable<Parse> MethodCells(CellRange theCells);
        protected abstract IEnumerable<Parse> ParameterCells(CellRange theCells);

        private void AddCell(Parse theCells, object theNewValue) {
            theCells.Last.More = (Parse)Processor.Compose(theNewValue);
        }

        private void ColorMethodName(Parse theCells, bool thisIsRight) {
            foreach (Parse nameCell in MethodCells(new CellRange(theCells))) {
                TestStatus.ColorCell(nameCell, thisIsRight);
            }
        }

        public object ExecuteEmbeddedMethod(Parse theCells, int theExcludedCellCount) {
            try {
                CellRange cells = GetMethodCellRange(theCells, theExcludedCellCount);
                return
                    CellOperation.Invoke(this, new CellRange(MethodCells(cells)), new CellRange(ParameterCells(cells))).
                        Value;
            }
            catch (ParseException<Cell> e) {
                TestStatus.MarkException(e.Subject, e.InnerException);
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
                TestStatus.MarkException(theRestOfTheRows.Parts, e);
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
            private readonly TestStatus testStatus;

            public Keywords(FlowFixtureBase fixture, TestStatus testStatus) {
                this.fixture = fixture;
                this.testStatus = testStatus;
            }
            
	        public void AbandonStoryTest(Parse theCells) {
                testStatus.MarkIgnore(theCells);
                testStatus.IsAbandoned = true;
                throw new AbandonStoryTestException();
            }

            public Fixture Calculate(Parse theCells) {
                return new CalculateFixture(fixture.SystemUnderTest ?? fixture);
            }

            public void Check(Parse theCells) {
                try {
                    CellRange cells = GetMethodCellRange(theCells, 1);
                    try {
                        fixture.CellOperation.Check(testStatus, fixture.GetTargetObject(),
                            new CellRange(fixture.MethodCells(cells)), 
                            new CellRange(fixture.ParameterCells(cells)),
                            theCells.Last);
                    }
                    catch (MemberMissingException e) {
                        testStatus.MarkException(theCells.More, e);
                    }
                    catch (Exception e) {
                        testStatus.MarkException(theCells.Last, e);
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
                    testStatus.ColorCell(theCells, (bool)fixture.ExecuteEmbeddedMethod(theCells, 0));
                }
                catch (IgnoredException) {}
                catch (Exception e) {
                    testStatus.MarkException(theCells, e);
                }
            }

            public void Not(Parse theCells) {
                try {
                    testStatus.ColorCell(theCells, !(bool)fixture.ExecuteEmbeddedMethod(theCells, 0));
                }
                catch (IgnoredException) {}
                catch (Exception) {
                    testStatus.MarkRight(theCells);
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

	            testStatus.MarkRight(restOfTheCells);
	        }

            public void Note(Parse theCells) {}

            public void Reject(Parse theCells) {
                Not(theCells);
            }

            public void Show(Parse theCells) {
                try {
                    fixture.AddCell(theCells, fixture.ExecuteEmbeddedMethod(theCells, 0));
                }
                catch (IgnoredException) {}
            }

            public void Start(Parse theCells) {
                try {
                    fixture.SetSystemUnderTest(new WithPhrase(theCells).EvaluateNew(fixture));
                }
                catch (Exception e) {
                    testStatus.MarkException(theCells, e);
                }
            }

            public void With(Parse theCells) {
                fixture.SetSystemUnderTest(new WithPhrase(theCells).Evaluate(fixture));
            }
        }
    }
}
