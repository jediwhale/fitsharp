// Copyright © 2010 Syterra Software Inc.
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
        const string simpleProcedureHtml = "<table><tr><td>define</td><td>procedure</td></tr><tr><td>verb</td></tr></table>";
        const string parameterProcedureHtml =
            "<table><tr><td>define</td><td>procedure</td><td>parm</td></tr><tr><td>verb</td><td>parm</td></tr></table>";
        const string twoParameterProcedureHtml =
            "<table><tr><td>define</td><td>procedure</td><td>parm1</td><td></td><td>parm2</td></tr><tr><td>verb</td><td>parm1</td><td>parm2</td></tr></table>";

        Mock<CellProcessor> processor;
        RuntimeProcedure runtime;
        Procedure procedure;
        TypedValue result;
        TypedValue target;
        TypedValue fixture;
        TestStatus testStatus;

        [Test] public void CreateIsntHandled() {
            SetupSUT(simpleProcedureHtml);
            Assert.IsFalse(runtime.CanCreate("anything", new CellTree()));
        }

        [Test] public void InvokeForMembersIsntHandled() {
            SetupSUT(simpleProcedureHtml);
            Assert.IsFalse(runtime.CanInvoke(target, "member", new CellTree()));
        }

        [Test] public void InvokeForProceduresIsHandled() {
            SetupSUT(simpleProcedureHtml);
            Assert.IsTrue(runtime.CanInvoke(target, "procedure", new CellTree()));
        }

        [Test] public void ProcedureIsExecuted() {
            SetupSUT(simpleProcedureHtml);
            Assert.AreEqual(result, runtime.Invoke(target, "procedure", new CellTree()));
        }

        [Test] public void ProcedureExecutionIsLogged() {
            SetupSUT(simpleProcedureHtml);
            runtime.Invoke(target, "procedure", new CellTree());
            Assert.AreEqual("procedure log", testStatus.LastAction);
        }

        [Test] public void ProcedureIsExecutedOnACopyOfBody() {
            SetupSUT(simpleProcedureHtml);
            runtime.Invoke(target, "procedure", new CellTree());
            Assert.AreEqual(string.Empty, procedure.Instance.Branches[1].Branches[0].Value.GetAttribute(CellAttribute.Label));
        }

        [Test] public void ParameterValueIsSubstituted() {
            SetupSUT(parameterProcedureHtml);
            Assert.AreEqual(result, runtime.Invoke(target, "procedure", new Parse("tr", "", new Parse("td", "actual", null, null), null)));
        }

        [Test] public void TwoParameterValuesAreSubstituted() {
            SetupSUT(twoParameterProcedureHtml);
            Assert.AreEqual(result, runtime.Invoke(target, "procedure", new Parse("tr", "",
                new Parse("td", "actual1", null, new Parse("td", "actual2", null, null)),
                null)));
        }

        void SetupSUT(string html) {
            procedure = new Procedure("procedure", new HtmlParser().Parse(html));

            result = new TypedValue("result");
            target = new TypedValue("target");
            fixture = new TypedValue("fixture");

            testStatus = new TestStatus();

            processor = new Mock<CellProcessor>();
            runtime = new RuntimeProcedure {Processor = processor.Object};

            processor.Setup(p => p.TestStatus).Returns(testStatus);
            processor.Setup(p => p.Contains(It.Is<Procedure>(v => v.Id == "member"))).Returns(false);
            processor.Setup(p => p.Contains(It.Is<Procedure>(v => v.Id == "procedure"))).Returns(true);
            processor.Setup(p => p.Load(It.Is<Procedure>(v => v.Id == "procedure"))).Returns(procedure);

            processor.Setup(p => p.Parse(typeof (Interpreter), target, It.Is<Tree<Cell>>(c => IsDoFixture(c))))
                .Returns(fixture);

            processor.Setup(p => p.Execute(fixture, It.Is<Tree<Cell>>(t => IsTablesWithVerb(t))))
                .Returns((TypedValue f, Tree<Cell> t) => {
                    t.Branches[0].Branches[0].Branches[0].Value.SetAttribute(CellAttribute.Label, "stuff");
                    testStatus.PopReturn();
                    testStatus.PushReturn(result);
                    return TypedValue.Void;
                });

            processor.Setup(p => p.Parse(typeof (StoryTestString), It.IsAny<TypedValue>(),
                                         It.Is<Tree<Cell>>(t => IsTableWithVerb(t))))
                .Returns(new TypedValue("procedure log"));
        }

        static bool IsTableWithVerb(Tree<Cell> t) {
            return t.Branches[0].Branches.Count == 1 && t.Branches[0].Branches[0].Value.Text == "verb";
        }

        static bool IsTablesWithVerb(Tree<Cell> t) {
            if (t.Branches[0].Branches[0].Branches.Count == 1
                && t.Branches[0].Branches[0].Branches[0].Value.Text == "verb") return true;
            if (t.Branches[0].Branches[0].Branches.Count == 2
                && t.Branches[0].Branches[0].Branches[0].Value.Text == "verb"
                && t.Branches[0].Branches[0].Branches[1].Value.Text == "actual") return true;
            if (t.Branches[0].Branches[0].Branches.Count == 3
                && t.Branches[0].Branches[0].Branches[0].Value.Text == "verb"
                && t.Branches[0].Branches[0].Branches[1].Value.Text == "actual1"
                && t.Branches[0].Branches[0].Branches[2].Value.Text == "actual2") return true;
            return false;
        }

        static bool IsDoFixture(Tree<Cell> c) {
            return c.Branches[0].Value.Text == "dofixture";
        }
    }
}
