// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Model;
using Moq;
using NUnit.Framework;
using TestStatus=fitSharp.Fit.Model.TestStatus;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class ExecuteDefaultTest {
        Mock<CellProcessor> processor;
        ExecuteDefault execute;
        TypedValue target;
        TypedValue result;
        TestStatus testStatus;
        CellTreeLeaf targetCell;
        string memberName;

        [Test] public void MethodIsInvoked() {
            SetUpSUT("member");
            Assert.AreEqual(result, ExecuteWithNoTargetCell());
        }

        [Test] public void LastStatusIsClearedWhenMethodIsInvoked() {
            SetUpSUT("member");
            testStatus.LastAction = "blah blah";
            ExecuteWithNoTargetCell();
            Assert.IsNull(testStatus.LastAction);
        }

        [Test] public void LastActionIsSetAsInvokeCellAttribute() {
            SetUpSUT("procedure");
            Execute(targetCell);
            Assert.AreEqual("blah blah", targetCell.Value.GetAttribute(CellAttribute.Folded));
        }

        [Test] public void CellIsMarkedWithInvokeStatus() {
            SetUpSUT("procedure");
            testStatus.Counts.AddCount(TestStatus.Wrong);
            Execute(targetCell);
            Assert.AreEqual(TestStatus.Right, targetCell.Value.GetAttribute(CellAttribute.Status));
        }

        [Test] public void CellIsNotMarkedIfAlreadyMarked() {
            SetUpSUT("procedure");
            targetCell.Value.SetAttribute(CellAttribute.Status, TestStatus.Wrong);
            Execute(targetCell);
            Assert.AreEqual(TestStatus.Wrong, targetCell.Value.GetAttribute(CellAttribute.Status));
        }

        [Test] public void LastActionIsSetAsInputCellAttribute() {
            SetUpSUT("procedure");
            new InvokeOperation(processor.Object, target, new CellTreeLeaf("procedure"), targetCell, targetCell).Do();
            Assert.AreEqual("blah blah", targetCell.Value.GetAttribute(CellAttribute.Folded));
        }

        [Test] public void LastActionIsSetAsExpectedCellAttribute() {
            SetUpSUT("procedure");
            var parameters = new ExecuteParameters(
                ExecuteParameters.Make(new CellTreeLeaf("procedure"), new CellTree(), targetCell));
            execute.Execute(new ExecuteContext(ExecuteCommand.Check, target.Value), parameters);
            Assert.AreEqual("blah blah", targetCell.Value.GetAttribute(CellAttribute.Folded));
        }

        TypedValue ExecuteWithNoTargetCell() {
            return Execute(null);
        }

        TypedValue Execute(Tree<Cell> targetCell) {
            var parameters = new ExecuteParameters(
                ExecuteParameters.Make(new CellTreeLeaf(memberName), new CellTree(), targetCell));
            return execute.Execute(new ExecuteContext(ExecuteCommand.Invoke, target), parameters);
        }

        void SetUpSUT(string memberName) {
            this.memberName = memberName;
            testStatus = new TestStatus();
            processor = new Mock<CellProcessor>();
            execute = new ExecuteDefault {Processor = processor.Object};

            target = new TypedValue("target");
            result = new TypedValue("result");

            targetCell = new CellTreeLeaf("stuff");

            processor
                .Setup(p => p.Parse(typeof (MemberName), It.IsAny<TypedValue>(), It.Is<CellTreeLeaf>(c => c.Text == memberName)))
                .Returns(new TypedValue(new MemberName(memberName)));
            processor
                .Setup(p => p.Invoke(target, "member", It.Is<Tree<Cell>>(c => c.Branches.Count == 0)))
                .Returns(result);
            processor
                .Setup(p => p.Invoke(It.Is<TypedValue>(v => v.ValueString == "target"), "procedure", It.IsAny<Tree<Cell>>()))
                .Returns((TypedValue t, string s, Tree<Cell> c) => {
                    testStatus.Counts.AddCount(TestStatus.Right);
                    testStatus.LastAction = "blah blah";
                    return result;
                });
            processor.Setup(p => p.Compare(It.IsAny<TypedValue>(), It.IsAny<Tree<Cell>>())).Returns(true);
            processor.Setup(p => p.TestStatus).Returns(testStatus);
        }
    }
}
