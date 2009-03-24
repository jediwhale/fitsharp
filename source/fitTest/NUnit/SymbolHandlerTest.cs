// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Engine;
using fit.Operators;
using fit.Test.Acceptance;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class SymbolHandlerTest
    {
        [Test]
        public void TestRegisterAndGet()
        {
            Assert.IsTrue(IsMatch("<<xyz"));
            Assert.IsFalse(TestUtils.IsMatch(new ExecuteSymbolSave(), ExecuteParameters.MakeCheck(TestUtils.CreateCell("x<<yz"))));
            Assert.IsFalse(IsMatch("x<<yz"));
            Assert.IsTrue(TestUtils.IsMatch(new ExecuteSymbolSave(), ExecuteParameters.MakeCheck(TestUtils.CreateCell(">>xyz"))));
            Assert.IsFalse(TestUtils.IsMatch(new ExecuteSymbolSave(), ExecuteParameters.MakeInput(TestUtils.CreateCell("stuff"), TestUtils.CreateCell(">>xyz"))));
            Assert.IsFalse(TestUtils.IsMatch(new ExecuteSymbolSave(), ExecuteParameters.MakeCheck(TestUtils.CreateCell("x>>yz"))));
            Assert.IsFalse(IsMatch("x>>yz"));

            Assert.IsFalse(TestUtils.IsMatch(new ExecuteSymbolSave(), ExecuteParameters.MakeInput(TestUtils.CreateCell("stuff"), TestUtils.CreateCell("error"))));
        }

        private static bool IsMatch(string input) {
            return CellHandlerTestUtils.IsMatch(new ParseSymbol(), input);
        }

        [Test]
        public void TestSaveString() {
            Parse cell = TestUtils.CreateCell(">>xyz");
            StringFixture fixture = new StringFixture();
            fixture.Field = "abc";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("abc", Fixture.Recall("xyz"));
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void CellContentWhenSaving()
        {
            Parse cell = TestUtils.CreateCell(">>xyz");
            StringFixture fixture = new StringFixture();
            fixture.Field = "abc";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual(" <span class=\"fit_grey\">abc &gt;&gt;xyz</span>", cell.Body);
        }

        [Test]
        public void TestRecallString() {
            Parse cell = TestUtils.CreateCell("<<def");
            StringFixture fixture = new StringFixture();
            Fixture.Save("def","ghi");
            fixture.CellOperation.Input(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("ghi", fixture.Field);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void CellContentWhenRecalling_Right()
        {
            Parse cell = TestUtils.CreateCell("<<def");
            StringFixture fixture = new StringFixture();
            Fixture.Save("def","ghi");
            fixture.Field = "ghi";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("ghi <span class=\"fit_grey\">&lt;&lt;def</span>", cell.Body);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }		

        [Test]
        public void CellContentWhenRecalling_Wrong()
        {
            Parse cell = TestUtils.CreateCell("<<def");
            StringFixture fixture = new StringFixture();
            Fixture.Save("def","ghi");
            fixture.Field = "xyz";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("ghi <span class=\"fit_grey\">&lt;&lt;def</span> <span class=\"fit_label\">expected</span><hr />xyz <span class=\"fit_label\">actual</span>", cell.Body);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }	

        [Test]
        public void CellContentWhenRecalling_Input()
        {
            Parse cell = TestUtils.CreateCell("<<def");
            StringFixture fixture = new StringFixture();
            Fixture.Save("def","ghi");
            fixture.Field = "xyz";
            fixture.CellOperation.Input(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("ghi <span class=\"fit_grey\">&lt;&lt;def</span>", cell.Body);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void CellContentWhenRecalling_Evaluate()
        {
            Parse cell = TestUtils.CreateCell("<<def");
            StringFixture fixture = new StringFixture();
            Fixture.Save("def","ghi");
            fixture.Field = "xyz";
            fixture.CellOperation.Compare(new TypedValue("xyz"), cell);
            Assert.AreEqual("ghi <span class=\"fit_grey\">&lt;&lt;def</span>", cell.Body);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestEvaluateRecallStringPass() {
            Parse cell = TestUtils.CreateCell("<<def");
            StringFixture fixture = new StringFixture();
            Fixture.Save("def","ghi");
            fixture.Field = "ghi";
            Assert.IsTrue(fixture.CellOperation.Compare(new TypedValue("ghi"), cell));
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestEvaluateRecallStringFail() {
            Parse cell = TestUtils.CreateCell("<<def");
            StringFixture fixture = new StringFixture();
            Fixture.Save("def","ghi");
            fixture.Field = "not ghi";
            Assert.IsFalse(fixture.CellOperation.Compare(new TypedValue("not ghi"), cell));
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestEvaluateRecallStringFailNull() {
            Parse cell = TestUtils.CreateCell("<<def");
            StringFixture fixture = new StringFixture();
            Fixture.Save("def","ghi");
            fixture.Field = null;
            Assert.IsFalse(fixture.CellOperation.Compare(new TypedValue(null, typeof(string)), cell));
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestCheckRecallValuePass()
        {
            Parse cell = TestUtils.CreateCell("<<theKey");
            StringFixture fixture = new StringFixture();
            Fixture.Save("theKey","theValue");
            fixture.Field = "theValue";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestCheckRecallValuePassPerson()
        {
            Parse cell = TestUtils.CreateCell("<<thePerson");
            PersonFixture fixture = new PersonFixture();
            Person person = new Person("Eeek", "Gadd");
            Fixture.Save("thePerson", person);
            fixture.Field = person;
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestCheckRecallValueFail()
        {
            Parse cell = TestUtils.CreateCell("<<theKey");
            StringFixture fixture = new StringFixture();
            Fixture.Save("theKey","theValue");
            fixture.Field = "anotherValue";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestCheckRecallValueFailPerson()
        {
            Parse cell = TestUtils.CreateCell("<<thePerson");
            PersonFixture fixture = new PersonFixture();
            Person person = new Person("Eeek", "Gadd");
            Person person2 = new Person("Eeek", "Gadds");
            Fixture.Save("thePerson", person);
            fixture.Field = person2;
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }
    }
}