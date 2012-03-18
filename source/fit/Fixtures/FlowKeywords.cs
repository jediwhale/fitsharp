// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using fit.exception;
using fit.Model;
using fitlibrary;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Exception;
using fitSharp.Fit.Fixtures;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Fixtures {
    public class FlowKeywords {
        public FlowKeywords(FlowInterpreter fixture, CellProcessor processor) {
            this.fixture = fixture;
            this.processor = processor;
        }

        public void AbandonStoryTest(Parse theCells) {
            processor.TestStatus.MarkIgnore(theCells);
            processor.TestStatus.IsAbandoned = true;
            throw new AbandonStoryTestException();
        }

        public Fixture Calculate(Parse theCells) {
            return new CalculateFixture(fixture.SystemUnderTest ?? fixture);
        }

        public void Check(Parse theCells) {
            try {
                CellRange methodCells = CellRange.GetMethodCellRange(theCells, 1);
                try {
                    DoCheckOperation(theCells.Last, methodCells);
                }
                catch (MemberMissingException e) {
                    processor.TestStatus.MarkException(theCells.More, e);
                }
                catch (Exception e) {
                    processor.TestStatus.MarkException(theCells.Last, e);
                }
            }
            catch (IgnoredException) {}
        }

        void DoCheckOperation(Tree<Cell> expectedValue, Tree<Cell> cells) {
            processor.Check(fixture,
                    fixture.MethodRowSelector.SelectMethodCells(cells),
                    fixture.MethodRowSelector.SelectParameterCells(cells),
                    expectedValue);
        }

        public List<object> CheckFieldsFor(Parse cells) {
            return new List<object> { ExecuteEmbeddedMethod(cells) };
        }

        public void Return(Parse cells) {
            var result = new MethodPhrase(new CellRange(cells)).Evaluate(fixture, processor);
            processor.CallStack.SetReturn(new TypedValue(result));
        }

        public void Set(Parse theCells) {
            ExecuteEmbeddedMethod(theCells);
        }

        public CommentFixture Comment(Parse cells) {
            return new CommentFixture();
        }

        public Fixture Ignored(Parse cells) {
            return new Fixture();
        }

        public void Ensure(Parse theCells) {
            try {
                processor.TestStatus.ColorCell(theCells, (bool) ExecuteEmbeddedMethod(theCells));
            }
            catch (IgnoredException) {}
            catch (Exception e) {
                processor.TestStatus.MarkException(theCells, e);
            }
        }

        public void Not(Parse theCells) {
            try {
                processor.TestStatus.ColorCell(theCells, !(bool) ExecuteEmbeddedMethod(theCells));
            }
            catch (IgnoredException) {}
            catch (Exception) {
                processor.TestStatus.MarkRight(theCells);
            }
        }

        public void Name(Parse theCells) {
            Parse restOfTheCells = theCells.More;
            if (restOfTheCells == null || restOfTheCells.More == null)
                throw new TableStructureException("missing cells for name.");

            object namedValue = ourWithIdentifier.Equals(restOfTheCells.More.Text)
                                    ? new MethodPhrase(new CellRange(restOfTheCells.More)).Evaluate(fixture, processor)
                                    : ExecuteEmbeddedMethod(restOfTheCells);
            processor.Get<Symbols>().Save(restOfTheCells.Text, namedValue);
            processor.TestStatus.MarkRight(restOfTheCells);
        }

        public void Note(Parse theCells) {}

        public void Reject(Parse theCells) {
            Not(theCells);
        }

        public void Show(Parse theCells) {
            try {
                AddCell(theCells, ExecuteEmbeddedMethod(theCells));
            }
            catch (IgnoredException) {}
        }

        public void ShowAs(Parse cells) {
            try {
                var attributes = processor.Parse<Cell, CellAttribute[]>(cells.More);
                var value = ExecuteEmbeddedMethod(cells.More);
                AddCell(cells, new ComposeShowAsOperator(attributes, value));
            }
            catch (IgnoredException) {}
        }

        public void Start(Parse theCells) {
            try {
                fixture.SetSystemUnderTest(new MethodPhrase(new CellRange(theCells)).EvaluateNew(processor));
            }
            catch (Exception e) {
                processor.TestStatus.MarkException(theCells, e);
            }
        }

        public void With(Parse theCells) {
            fixture.SetSystemUnderTest(new MethodPhrase(new CellRange(theCells)).Evaluate(fixture, processor));
        }

        void AddCell(Parse theCells, object theNewValue) {
            theCells.Last.More = (Parse)processor.Compose(theNewValue);
        }

        public object ExecuteEmbeddedMethod(Parse theCells) {
            return fixture.ExecuteFlowRowMethod(processor, new CellRange(theCells));
        }

        static readonly IdentifierName ourWithIdentifier = new IdentifierName("with");

        readonly FlowInterpreter fixture;
        readonly CellProcessor processor;
    }
}
