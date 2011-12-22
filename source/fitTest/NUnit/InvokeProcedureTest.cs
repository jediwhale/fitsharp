// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Operators;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class InvokeProcedureTest {
        const string noParameterProcedureHtml = "<table><tr><td>define</td><td>procedure</td></tr><tr><td>settext</td></tr></table>";
        const string errorProcedureHtml = "<table><tr><td>define</td><td>procedure</td></tr><tr><td>garbage</td></tr></table>";
        const string oneParameterProcedureHtml =
            "<table><tr><td>define</td><td>procedure</td><td>parm</td></tr><tr><td>settext</td><td>parm</td></tr></table>";
        const string twoParameterProcedureHtml =
            "<table><tr><td>define</td><td>procedure</td><td>parm1</td><td></td><td>parm2</td></tr><tr><td>settext</td><td>parm1</td><td></td><td>parm2</td></tr></table>";

        InvokeProcedure invokeProcedure;
        CellProcessor cellProcessor;

        [SetUp] public void SetUp() {
            invokeProcedure = new InvokeProcedure();
            cellProcessor = new Service.Service();
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
            cellProcessor.Get<Procedures>().Save("procedure", Parse.ParseFrom(noParameterProcedureHtml));
            var sample = new Sample();
            invokeProcedure.Invoke(new TypedValue(sample), new MemberName("procedure"), new CellTree());
            Assert.AreEqual("hi", sample.Text);
        }

        [Test] public void ProcedureExecutionIsLogged() {
            cellProcessor.Get<Procedures>().Save("procedure", Parse.ParseFrom(noParameterProcedureHtml));
            var sample = new Sample();
            invokeProcedure.Invoke(new TypedValue(sample), new MemberName("procedure"), new CellTree());
            Assert.AreEqual("<table><tr><td>settext</td></tr></table>", cellProcessor.TestStatus.LastAction);
        }

        [Test] public void ProcedureIsExecutedOnACopyOfBody() {
            cellProcessor.Get<Procedures>().Save("procedure", Parse.ParseFrom(errorProcedureHtml));
            var sample = new Sample();
            invokeProcedure.Invoke(new TypedValue(sample), new MemberName("procedure"), new CellTree());
            Assert.AreEqual(errorProcedureHtml, cellProcessor.Get<Procedures>().GetValue("procedure").ToString());
        }

        [Test] public void ParameterValueIsSubstituted() {
            cellProcessor.Get<Procedures>().Save("procedure", Parse.ParseFrom(oneParameterProcedureHtml));
            var sample = new Sample();
            invokeProcedure.Invoke(new TypedValue(sample), new MemberName("procedure"), new Parse("tr", "", new Parse("td", "actual", null, null), null));
            Assert.AreEqual("actual", sample.Text);
        }

        [Test] public void TwoParameterValuesAreSubstituted() {
            cellProcessor.Get<Procedures>().Save("procedure", Parse.ParseFrom(twoParameterProcedureHtml));
            var sample = new Sample();
            invokeProcedure.Invoke(new TypedValue(sample), new MemberName("procedure"), new Parse("tr", "",
                new Parse("td", "actual1", null, new Parse("td", "actual2", null, null)),
                null));
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
