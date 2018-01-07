// Copyright © 2018 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class InvokeProcedureTest {
        const string noParameterProcedureHtml = "<br><table border=1><tr><td>define</td><td>procedure</td></tr><tr><td>settext</td></tr></table>";
        const string errorProcedureHtml = "<table><tr><td>define</td><td>procedure</td></tr><tr><td>garbage</td></tr></table>";

        InvokeProcedure invokeProcedure;
        CellProcessor cellProcessor;

        [SetUp] public void SetUp() {
            invokeProcedure = new InvokeProcedure();
            cellProcessor = Builder.CellProcessor();
            invokeProcedure.Processor = cellProcessor;
        }

        [Test] public void InvokeForMembersIsntHandled() {
            Assert.IsFalse(invokeProcedure.CanInvoke(TypedValue.Void, new MemberName("member"), new CellTree()));
        }

        [Test] public void InvokeForProceduresIsHandled() {
            cellProcessor.Get<Procedures>().Save("procedure", new CellTree());
            Assert.IsTrue(invokeProcedure.CanInvoke(TypedValue.Void, new MemberName("procedure"), new CellTree()));
        }

        [Test] public void ProcedureIsExecuted() {
            cellProcessor.Get<Procedures>().Save("procedure", new CellTree(new CellTree("define", "procedure"), new CellTree("settext")));
            var sample = new Sample();
            invokeProcedure.Invoke(new TypedValue(sample), new MemberName("procedure"), new CellTree());
            Assert.AreEqual("hi", sample.Text);
        }

        [Test] public void ProcedureInNestedTableIsExecuted() {
            cellProcessor.Get<Procedures>().Save("procedure", new CellTree(new CellTree("define", "procedure"), new CellTree(new CellTree(new CellTree(new CellTree("settext"))))));
            var sample = new Sample();
            invokeProcedure.Invoke(new TypedValue(sample), new MemberName("procedure"), new CellTree());
            Assert.AreEqual("hi", sample.Text);
        }

        [Test] public void ProcedureExecutionIsLogged() {
            cellProcessor.Get<Procedures>().Save("procedure", Builder.ParseHtmlTable(noParameterProcedureHtml));
            var sample = new Sample();
            invokeProcedure.Invoke(new TypedValue(sample), new MemberName("procedure"), new CellTree());
            Assert.AreEqual("<table border=1><tr><td><span class=\"fit_member\">settext</span></td></tr></table>", cellProcessor.TestStatus.LastAction);
        }

        [Test] public void ProcedureIsExecutedOnACopyOfBody() {
            cellProcessor.Get<Procedures>().Save("procedure", Builder.ParseHtmlTable(errorProcedureHtml));
            var sample = new Sample();
            invokeProcedure.Invoke(new TypedValue(sample), new MemberName("procedure"), new CellTree());
            Assert.AreEqual(errorProcedureHtml, ((Tree<Cell>)cellProcessor.Get<Procedures>().GetValue("procedure")).WriteTree());
        }

        [Test] public void ParameterValueIsSubstituted() {
            cellProcessor.Get<Procedures>().Save("procedure", new CellTree(new CellTree("define", "procedure", "parm"), new CellTree("settext", "parm")));
            var sample = new Sample();
            invokeProcedure.Invoke(new TypedValue(sample), new MemberName("procedure"), new CellTree("actual"));
            Assert.AreEqual("actual", sample.Text);
        }

        [Test] public void TwoParameterValuesAreSubstituted() {
            cellProcessor.Get<Procedures>()
                .Save("procedure",
                    new CellTree(new CellTree("define", "procedure", "parm1", "", "parm2"), new CellTree("settext", "parm1", "", "parm2")));
            var sample = new Sample();
            invokeProcedure.Invoke(new TypedValue(sample), new MemberName("procedure"), new CellTree("actual1", "actual2"));
            Assert.AreEqual("actual1actual2", sample.Text);
        }

        class Sample {
            public string Text;
            public void SetText() { Text = "hi"; }
            public void SetText(string text) { Text = text; }
            public void SetText(string text1, string text2) { Text = text1 + text2; }
        }
    }
}
