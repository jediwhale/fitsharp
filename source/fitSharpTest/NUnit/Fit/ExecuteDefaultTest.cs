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
    [TestFixture] public class ExecuteDefaultTest {
        private Mock<CellProcessor> processor;
        private ExecuteDefault execute;
        private TypedValue target;
        private TypedValue result;
        private TestStatus testStatus;

        [Test] public void MethodIsInvoked() {
            SetUpSUT();
            var parameters = new ExecuteParameters(
                ExecuteParameters.MakeMemberParameters(new StringCellLeaf("member"), new TreeList<Cell>()));
            Assert.AreEqual(result, execute.Execute(new ExecuteContext(ExecuteCommand.Invoke, target), parameters));
        }

        [Test] public void LastActionIsSetAsCellAttribute() {
            SetUpSUT();
            var targetCell = new StringCellLeaf("stuff");
            var parameters = new ExecuteParameters(
                ExecuteParameters.Make(new StringCellLeaf("member"), new TreeList<Cell>(), targetCell));
            testStatus.LastAction = "blah blah";
            execute.Execute(new ExecuteContext(ExecuteCommand.Invoke, target), parameters);
            Assert.AreEqual("blah blah", targetCell.Value.GetAttribute(CellAttributes.ExtensionKey));
        }

        [Test] public void CellIsMarkedWithInvokeStatus() {
            SetUpSUT();
            testStatus.Counts.AddCount(CellAttributes.WrongStatus);
            var targetCell = new StringCellLeaf("stuff");
            var parameters = new ExecuteParameters(
                ExecuteParameters.Make(new StringCellLeaf("member"), new TreeList<Cell>(), targetCell));
            testStatus.LastAction = "blah blah";
            execute.Execute(new ExecuteContext(ExecuteCommand.Invoke, target), parameters);
            Assert.AreEqual(CellAttributes.RightStatus, targetCell.Value.GetAttribute(CellAttributes.StatusKey));
        }

        private void SetUpSUT() {
            testStatus = new TestStatus();
            processor = new Mock<CellProcessor>();
            execute = new ExecuteDefault {Processor = processor.Object};

            target = new TypedValue("target");
            result = new TypedValue("result");

            processor
                .Setup(p => p.Parse(typeof (MemberName), It.IsAny<TypedValue>(), It.Is<StringCellLeaf>(c => c.Text == "member")))
                .Returns(new TypedValue(new MemberName("member")));
            processor
                .Setup(p => p.Invoke(target, "member", It.Is<Tree<Cell>>(c => c.Branches.Count == 0)))
                .Returns((TypedValue t, string s, Tree<Cell> c) => {
                    testStatus.Counts.AddCount(CellAttributes.RightStatus);
                    return result;
                });
            processor.Setup(p => p.TestStatus).Returns(testStatus);
        }
    }
}
