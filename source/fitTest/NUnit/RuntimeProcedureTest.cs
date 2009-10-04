// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Operators;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;
using Moq;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class RuntimeProcedureTest {
        private Mock<CellProcessor> processor;
        private RuntimeProcedure runtime;
        private Procedure procedure;
        private TypedValue result;
        private TypedValue target;
        private TypedValue fixture;
        private TestStatus testStatus;

        [Test] public void CreateIsntHandled() {
            SetupSUT();
            Assert.IsFalse(runtime.CanCreate("anything", new CellTree()));
        }

        [Test] public void InvokeForMembersIsntHandled() {
            SetupSUT();
            Assert.IsFalse(runtime.CanInvoke(target, "member", new CellTree()));
        }

        [Test] public void InvokeForProceduresIsHandled() {
            SetupSUT();
            Assert.IsTrue(runtime.CanInvoke(target, "procedure", new CellTree()));
        }

        [Test] public void ProcedureIsExecuted() {
            SetupSUT();
            Assert.AreEqual(result, runtime.Invoke(target, "procedure", new CellTree()));
        }

        [Test] public void ProcedureExecutionIsLogged() {
            SetupSUT();
            runtime.Invoke(target, "procedure", new CellTree());
            Assert.AreEqual("procedure log", testStatus.LastAction);
        }

        [Test] public void ProcedureIsExecutedOnACopyOfBody() {
            SetupSUT();
            runtime.Invoke(target, "procedure", new CellTree());
            Assert.AreEqual(string.Empty, procedure.Instance.Branches[1].Branches[0].Value.GetAttribute("some"));
        }

        private void SetupSUT() {
            procedure = new Procedure("procedure", HtmlParser.Instance.Parse("<table><tr><td>define</td><td>procedure</td></tr><tr><td>verb</td></tr></table>"));

            result = new TypedValue("result");
            target = new TypedValue("target");
            fixture = new TypedValue("fixture");

            testStatus = new TestStatus();

            processor = new Mock<CellProcessor>();
            processor.Setup(p => p.TestStatus).Returns(testStatus);
            processor.Setup(p => p.Contains(It.Is<Procedure>(v => v.Id == "member"))).Returns(false);
            processor.Setup(p => p.Contains(It.Is<Procedure>(v => v.Id == "procedure"))).Returns(true);
            processor.Setup(p => p.Load(It.Is<Procedure>(v => v.Id == "procedure"))).Returns(procedure);
            processor.Setup(p => p.Parse(typeof (Interpreter), target,
                                         It.Is<Tree<Cell>>(c => c.Branches[0].Branches[0].Value.Text == "dofixture")))
                .Returns(fixture);
            processor.Setup(p => p.Execute(fixture,
                                           It.Is<Tree<Cell>>(t => t.Branches[0].Branches[0].Branches[0].Value.Text == "verb")))
                .Returns((TypedValue f, Tree<Cell> t) => {
                    t.Branches[0].Branches[0].Branches[0].Value.SetAttribute("some", "stuff");
                    return result;
                });
            processor.Setup(p => p.Parse(typeof (StoryTestString), It.IsAny<TypedValue>(),
                                         It.Is<Tree<Cell>>(t => t.Branches[0].Branches[0].Value.Text == "verb")))
                .Returns(new TypedValue("procedure log"));
            runtime = new RuntimeProcedure {Processor = processor.Object};
        }
    }
}