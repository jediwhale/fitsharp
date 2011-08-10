// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Operators;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;
using Moq;
using NUnit.Framework;
using TestStatus=fitSharp.Fit.Model.TestStatus;

namespace fit.Test.NUnit {
    [TestFixture] public class InvokeProcedureTest {
        const string noParameterProcedureHtml = "<table><tr><td>define</td><td>procedure</td></tr><tr><td>settext</td></tr></table>";
        const string simpleProcedureHtml = "<table><tr><td>define</td><td>procedure</td></tr><tr><td>verb</td></tr></table>";
        const string oneParameterProcedureHtml =
            "<table><tr><td>define</td><td>procedure</td><td>parm</td></tr><tr><td>settext</td><td>parm</td></tr></table>";
        const string twoParameterProcedureHtml =
            "<table><tr><td>define</td><td>procedure</td><td>parm1</td><td></td><td>parm2</td></tr><tr><td>settext</td><td>parm1</td><td></td><td>parm2</td></tr></table>";

        Mock<CellProcessor> processor;
        InvokeProcedure invoke;
        Procedure procedure;
        TypedValue result;
        TypedValue target;
        Mock<FlowInterpreter> fixture;
        Mock<InterpretTableFlow> flow;
        TestStatus testStatus;

        InvokeProcedure invokeProcedure;
        CellProcessor cellProcessor;

        [SetUp] public void SetUp() {
            invokeProcedure = new InvokeProcedure();
            cellProcessor = new Service.Service();
            invokeProcedure.Processor = cellProcessor;
        }

        [Test] public void InvokeForMembersIsntHandled() {
            Assert.IsFalse(invokeProcedure.CanInvoke(TypedValue.Void, "member", new CellTree()));
        }

        [Test] public void InvokeForProceduresIsHandled() {
            cellProcessor.Store(new Procedure("procedure"));
            Assert.IsTrue(invokeProcedure.CanInvoke(TypedValue.Void, "procedure", new CellTree()));
        }

        [Test] public void ProcedureIsExecuted() {
            cellProcessor.Store(new Procedure("procedure", Parse.ParseFrom(noParameterProcedureHtml)));
            var sample = new Sample();
            invokeProcedure.Invoke(new TypedValue(sample), "procedure", new CellTree());
            Assert.AreEqual("hi", sample.Text);
        }

        [Test] public void ProcedureExecutionIsLogged() {
            SetupSUT(simpleProcedureHtml);
            invoke.Invoke(target, "procedure", new CellTree());
            Assert.AreEqual("procedure log", testStatus.LastAction);
        }

        [Test] public void ProcedureIsExecutedOnACopyOfBody() {
            SetupSUT(simpleProcedureHtml);
            invoke.Invoke(target, "procedure", new CellTree());
            Assert.AreEqual(string.Empty, procedure.Instance.Branches[1].Branches[0].Value.GetAttribute(CellAttribute.Label));
        }

        [Test] public void ParameterValueIsSubstituted() {
            cellProcessor.Store(new Procedure("procedure", Parse.ParseFrom(oneParameterProcedureHtml)));
            var sample = new Sample();
            invokeProcedure.Invoke(new TypedValue(sample), "procedure", new Parse("tr", "", new Parse("td", "actual", null, null), null));
            Assert.AreEqual("actual", sample.Text);
        }

        [Test] public void TwoParameterValuesAreSubstituted() {
            cellProcessor.Store(new Procedure("procedure", Parse.ParseFrom(twoParameterProcedureHtml)));
            var sample = new Sample();
            invokeProcedure.Invoke(new TypedValue(sample), "procedure", new Parse("tr", "",
                new Parse("td", "actual1", null, new Parse("td", "actual2", null, null)),
                null));
            Assert.AreEqual("actual1actual2", sample.Text);
        }

        void SetupSUT(string html) {
            procedure = new Procedure("procedure", Parse.ParseFrom(html));

            result = new TypedValue("result");
            target = new TypedValue("target");

            fixture = new Mock<FlowInterpreter>();

            flow = new Mock<InterpretTableFlow>();
            flow.Setup(f => f.DoTableFlow(It.IsAny<CellProcessor>(), fixture.Object, It.Is<Tree<Cell>>(t => IsTablesWithVerb(t))))
                .Callback<CellProcessor, FlowInterpreter, Tree<Cell>>((p, f, t) => {
                    t.Branches[0].Branches[0].Value.SetAttribute(CellAttribute.Label, "stuff");
                    testStatus.PopReturn();
                    testStatus.PushReturn(result);
                });

            testStatus = new TestStatus();

            processor = new Mock<CellProcessor>();
            invoke = new InvokeProcedure {Processor = processor.Object};

            processor.Setup(p => p.TestStatus).Returns(testStatus);
            processor.Setup(p => p.Contains(It.Is<Procedure>(v => v.Id == "member"))).Returns(false);
            processor.Setup(p => p.Contains(It.Is<Procedure>(v => v.Id == "procedure"))).Returns(true);
            processor.Setup(p => p.Load(It.Is<Procedure>(v => v.Id == "procedure"))).Returns(procedure);

            processor.Setup(p => p.Parse(typeof (Interpreter), target, It.Is<Tree<Cell>>(c => IsDoFixture(c))))
                .Returns(new TypedValue(fixture.Object));

            processor.Setup(p => p.Parse(typeof (StoryTestString), It.IsAny<TypedValue>(),
                                         It.Is<Tree<Cell>>(t => IsTableWithVerb(t))))
                .Returns(new TypedValue("procedure log"));
        }

        static bool IsTableWithVerb(Tree<Cell> t) {
            return t.Branches[0].Branches[0].Branches.Count == 1 && t.Branches[0].Branches[0].Branches[0].Value.Text == "verb";
        }

        static bool IsTablesWithVerb(Tree<Cell> t) {
            if (t.Branches[0].Branches.Count == 1
                && t.Branches[0].Branches[0].Value.Text == "verb") return true;
            if (t.Branches[0].Branches.Count == 2
                && t.Branches[0].Branches[0].Value.Text == "verb"
                && t.Branches[0].Branches[1].Value.Text == "actual") return true;
            if (t.Branches[0].Branches.Count == 3
                && t.Branches[0].Branches[0].Value.Text == "verb"
                && t.Branches[0].Branches[1].Value.Text == "actual1"
                && t.Branches[0].Branches[2].Value.Text == "actual2") return true;
            return false;
        }

        static bool IsDoFixture(Tree<Cell> c) {
            return c.Branches[0].Value.Text == "fitlibrary.DoFixture";
        }

        class Sample {
            public string Text;
            public void SetText() { Text = "hi"; }
            public void SetText(string text) { Text = text; }
            public void SetText(string text1, string text2) { Text = text1 + text2; }
        }
    }
}
