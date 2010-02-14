// Copyright © 2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.exception;
using fit.Model;
using fitlibrary;
using fitlibrary.exception;
using fitSharp.Fit.Exception;
using fitSharp.Fit.Model;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Fixtures {
    public class FlowKeywords {
        private static readonly IdentifierName ourWithIdentifier = new IdentifierName("with");
        private readonly FlowFixtureBase fixture;

        public FlowKeywords(FlowFixtureBase fixture) {
            this.fixture = fixture;
        }

        public void AbandonStoryTest(Parse theCells) {
            fixture.TestStatus.MarkIgnore(theCells);
            fixture.TestStatus.IsAbandoned = true;
            throw new AbandonStoryTestException();
        }

        public Fixture Calculate(Parse theCells) {
            return new CalculateFixture(fixture.SystemUnderTest ?? fixture);
        }

        public void Check(Parse theCells) {
            try {
                CellRange methodCells = CellRange.GetMethodCellRange(theCells, 1);
                try {
                    fixture.DoCheckOperation(theCells.Last, methodCells);
                }
                catch (MemberMissingException e) {
                    fixture.TestStatus.MarkException(theCells.More, e);
                }
                catch (Exception e) {
                    fixture.TestStatus.MarkException(theCells.Last, e);
                }
            }
            catch (IgnoredException) {}
        }

        public void Return(Parse cells) {
            var result = fixture.ExecuteEmbeddedMethod(cells);
            fixture.Processor.TestStatus.SetReturn(new TypedValue(result));
        }

        public void Set(Parse theCells) {
            fixture.ExecuteEmbeddedMethod(theCells);
        }

        public CommentFixture Comment(Parse cells) {
            return new CommentFixture();
        }

        public Fixture Ignored(Parse cells) {
            return new Fixture();
        }

        public void Ensure(Parse theCells) {
            try {
                fixture.TestStatus.ColorCell(theCells, (bool) fixture.ExecuteEmbeddedMethod(theCells));
            }
            catch (IgnoredException) {}
            catch (Exception e) {
                fixture.TestStatus.MarkException(theCells, e);
            }
        }

        public void Not(Parse theCells) {
            try {
                fixture.TestStatus.ColorCell(theCells, !(bool) fixture.ExecuteEmbeddedMethod(theCells));
            }
            catch (IgnoredException) {}
            catch (Exception) {
                fixture.TestStatus.MarkRight(theCells);
            }
        }

        public void Name(Parse theCells) {
            Parse restOfTheCells = theCells.More;
            if (restOfTheCells == null || restOfTheCells.More == null)
                throw new TableStructureException("missing cells for name.");

            object namedValue = ourWithIdentifier.Equals(restOfTheCells.More.Text)
                                    ? new WithPhrase(restOfTheCells.More).Evaluate(fixture)
                                    : fixture.ExecuteEmbeddedMethod(restOfTheCells);
            fixture.Processor.Store(new Symbol(restOfTheCells.Text, namedValue));

            fixture.TestStatus.MarkRight(restOfTheCells);
        }

        public void Note(Parse theCells) {}

        public void Reject(Parse theCells) {
            Not(theCells);
        }

        public void Show(Parse theCells) {
            try {
                AddCell(theCells, fixture.ExecuteEmbeddedMethod(theCells));
            }
            catch (IgnoredException) {}
        }

        public void Start(Parse theCells) {
            try {
                fixture.SetSystemUnderTest(new WithPhrase(theCells).EvaluateNew(fixture));
            }
            catch (Exception e) {
                fixture.TestStatus.MarkException(theCells, e);
            }
        }

        public void With(Parse theCells) {
            fixture.SetSystemUnderTest(new WithPhrase(theCells).Evaluate(fixture));
        }

        void AddCell(Parse theCells, object theNewValue) {
            theCells.Last.More = (Parse)fixture.Processor.Compose(theNewValue);
        }

    }
}
