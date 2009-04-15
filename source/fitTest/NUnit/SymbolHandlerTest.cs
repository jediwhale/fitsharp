// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class SymbolHandlerTest: CellOperatorTest
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
            return IsMatch(new ParseSymbol(), input);
        }

        [Test]
        public void TestSaveString() {
            Parse cell = TestUtils.CreateCell(">>xyz");
            MakeStringFixture();
            stringFixture.Field = "abc";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("abc", LoadSymbol("xyz"));
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void CellContentWhenSaving()
        {
            Parse cell = TestUtils.CreateCell(">>xyz");
            MakeStringFixture();
            stringFixture.Field = "abc";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual(">>xyz<span class=\"fit_grey\"> abc</span>", cell.Body);
        }

        [Test]
        public void TestRecallString() {
            Parse cell = TestUtils.CreateCell("<<def");
            MakeStringFixture();
            StoreSymbol("def", "ghi");
            TestUtils.DoInput(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("ghi", stringFixture.Field);
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void CellContentWhenRecalling_Right()
        {
            Parse cell = TestUtils.CreateCell("<<def");
            MakeStringFixture();
            StoreSymbol("def", "ghi");
            stringFixture.Field = "ghi";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("<<def<span class=\"fit_grey\"> ghi</span>", cell.Body);
            TestUtils.VerifyCounts(stringFixture, 1, 0, 0, 0);
        }

        [Test]
        public void CellContentWhenRecalling_Wrong()
        {
            Parse cell = TestUtils.CreateCell("<<def");
            MakeStringFixture();
            StoreSymbol("def", "ghi");
            stringFixture.Field = "xyz";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("<<def<span class=\"fit_grey\"> ghi</span> <span class=\"fit_label\">expected</span><hr />xyz <span class=\"fit_label\">actual</span>", cell.Body);
            TestUtils.VerifyCounts(stringFixture, 0, 1, 0, 0);
        }	

        [Test]
        public void CellContentWhenRecalling_Input()
        {
            Parse cell = TestUtils.CreateCell("<<def");
            MakeStringFixture();
            StoreSymbol("def", "ghi");
            stringFixture.Field = "xyz";
            TestUtils.DoInput(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("<<def<span class=\"fit_grey\"> ghi</span>", cell.Body);
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void CellContentWhenRecalling_Evaluate()
        {
            Parse cell = TestUtils.CreateCell("<<def");
            MakeStringFixture();
            StoreSymbol("def", "ghi");
            stringFixture.Field = "xyz";
            stringFixture.CellOperation.Compare(new TypedValue("xyz"), cell);
            Assert.AreEqual("<<def<span class=\"fit_grey\"> ghi</span>", cell.Body);
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestEvaluateRecallStringPass() {
            Parse cell = TestUtils.CreateCell("<<def");
            MakeStringFixture();
            StoreSymbol("def", "ghi");
            stringFixture.Field = "ghi";
            Assert.IsTrue(stringFixture.CellOperation.Compare(new TypedValue("ghi"), cell));
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestEvaluateRecallStringFail() {
            Parse cell = TestUtils.CreateCell("<<def");
            MakeStringFixture();
            StoreSymbol("def", "ghi");
            stringFixture.Field = "not ghi";
            Assert.IsFalse(stringFixture.CellOperation.Compare(new TypedValue("not ghi"), cell));
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestEvaluateRecallStringFailNull() {
            Parse cell = TestUtils.CreateCell("<<def");
            MakeStringFixture();
            StoreSymbol("def", "ghi");
            stringFixture.Field = null;
            Assert.IsFalse(stringFixture.CellOperation.Compare(new TypedValue(null, typeof(string)), cell));
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestCheckRecallValuePass()
        {
            Parse cell = TestUtils.CreateCell("<<theKey");
            MakeStringFixture();
            StoreSymbol("theKey", "theValue");
            stringFixture.Field = "theValue";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            TestUtils.VerifyCounts(stringFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestCheckRecallValuePassPerson()
        {
            Parse cell = TestUtils.CreateCell("<<thePerson");
            MakePersonFixture();
            Person person = new Person("Eeek", "Gadd");
            StoreSymbol("thePerson", person);
            personFixture.Field = person;
            TestUtils.DoCheck(personFixture, TestUtils.CreateCellRange("Field"), cell);
            TestUtils.VerifyCounts(personFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestCheckRecallValueFail()
        {
            Parse cell = TestUtils.CreateCell("<<theKey");
            MakeStringFixture();
            StoreSymbol("theKey","theValue");
            stringFixture.Field = "anotherValue";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            TestUtils.VerifyCounts(stringFixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestCheckRecallValueFailPerson()
        {
            Parse cell = TestUtils.CreateCell("<<thePerson");
            MakePersonFixture();
            Person person = new Person("Eeek", "Gadd");
            Person person2 = new Person("Eeek", "Gadds");
            StoreSymbol("thePerson", person);
            personFixture.Field = person2;
            TestUtils.DoCheck(personFixture, TestUtils.CreateCellRange("Field"), cell);
            TestUtils.VerifyCounts(personFixture, 0, 1, 0, 0);
        }

        private object LoadSymbol(string symbolName) {
            return service.Load(new Symbol(symbolName)).Instance;
        }

        private void StoreSymbol(string symbolName, object symbolValue) {
            service.Store(new Symbol(symbolName, symbolValue));
        }

    }
}