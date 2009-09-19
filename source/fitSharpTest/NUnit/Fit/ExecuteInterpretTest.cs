// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using Moq;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ExecuteInterpretTest {

        private Mock<CellProcessor> processor;
        private ExecuteInterpret interpreter;
        private CellTree story;

        [Test] public void TableIsInterpreted() {
            SetUpSingleTableStory();
            var fixture = new Mock<Interpreter>();
            SetUpParse(story.Branches[0], fixture.Object);

            TestExecute(story);

            fixture.Verify(f => f.Interpret(story.Branches[0]));
        }

        [Test] public void MultipleTablesAreInterpreted() {
            SetUpTwoTableStory();
            var fixture1 = new Mock<Interpreter>();
            var fixture2 = new Mock<Interpreter>();
            SetUpParse(story.Branches[0], fixture1.Object);
            SetUpParse(story.Branches[1], fixture2.Object);

            TestExecute(story);

            fixture1.Verify(f => f.Interpret(story.Branches[0]));
            fixture2.Verify(f => f.Interpret(story.Branches[1]));
        }

        [Test] public void FlowTablesAreInterpreted() {
            SetUpTwoTableStory();
            var flowFixture = new Mock<FlowInterpreter>();
            SetUpParse(story.Branches[0], flowFixture.Object);

            TestExecute(story);

            flowFixture.Verify(f => f.Interpret(story.Branches[0]));
            flowFixture.Verify(f => f.InterpretFlow(story.Branches[1]));
        }

        [Test] public void FlowsWithExistingInterpreter() {
            SetUpTwoTableStory();
            var flowFixture = new Mock<FlowInterpreter>();

            TestExecute(story, new TypedValue(flowFixture.Object));

            flowFixture.Verify(f => f.InterpretFlow(story.Branches[0]));
            flowFixture.Verify(f => f.InterpretFlow(story.Branches[1]));
        }

        private void TestExecute(Tree<Cell> story, TypedValue target) {
            interpreter = new ExecuteInterpret {Processor = processor.Object};
            interpreter.Execute(target, story);
        }

        private void TestExecute(Tree<Cell> story) { TestExecute(story, TypedValue.Void); }

        private void SetUpParse(Tree<Cell> table, Interpreter fixture) {
            processor.Setup(p => p.Parse(typeof (Interpreter), TypedValue.Void, table)).Returns(new TypedValue(fixture));
        }

        private void SetUpSingleTableStory() {
            story = new CellTree(new CellTree(new CellTree("fixture one")));
            processor = new Mock<CellProcessor>();
        }

        private void SetUpTwoTableStory() {
            story = new CellTree(new CellTree(new CellTree("fixture one")), new CellTree(new CellTree("fixture two")));
            processor = new Mock<CellProcessor>();
        }
    }
}
