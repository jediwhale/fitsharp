// Copyright � 2011 Syterra Software Inc. Includes work by Object Mentor, Inc., � 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Samples;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fit.Test.NUnit {
    [TestFixture]
    public class SymbolHandlerTest: CellOperatorTest
    {
        [Test] public void TestRegisterAndGet() {
            ClassicAssert.IsFalse(IsMatch(new CheckSymbolSave(), "x<<yz"));
            ClassicAssert.IsTrue(IsMatch(new CheckSymbolSave(), ">>xyz"));
            ClassicAssert.IsFalse(IsMatch(new CheckSymbolSave(), "x>>yz"));

            // Make sure we recognize symbols even in formatted content
            ClassicAssert.IsTrue(IsMatch(new CheckSymbolSave(), "\n    >>xyz\n"));
        }

        [Test] public void TestWhitespaceEnclosedSave() {
          Parse cell = TestUtils.CreateCell("\n    >>xyz\n");
          MakeStringFixture();
          stringFixture.Field = "abc";
          TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);

          ClassicAssert.AreEqual("abc", LoadSymbol("xyz"));
        }

        [Test]
        public void TestSaveString() {
            Parse cell = TestUtils.CreateCell(">>xyz");
            MakeStringFixture();
            stringFixture.Field = "abc";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            ClassicAssert.AreEqual("abc", LoadSymbol("xyz"));
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void CellContentWhenSaving()
        {
            Parse cell = TestUtils.CreateCell(">>xyz");
            MakeStringFixture();
            stringFixture.Field = "abc";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            ClassicAssert.AreEqual(">>xyz<span class=\"fit_grey\"> abc</span>", cell.Body);
        }

        [Test]
        public void CellContentWhenRecalling_Right()
        {
            Parse cell = TestUtils.CreateCell("<<def");
            MakeStringFixture();
            StoreSymbol("def", "ghi");
            stringFixture.Field = "ghi";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            ClassicAssert.AreEqual("<<def<span class=\"fit_grey\"> ghi</span>", cell.Body);
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
            ClassicAssert.AreEqual("<<def<span class=\"fit_grey\"> ghi</span> <span class=\"fit_label\">expected</span><hr />xyz <span class=\"fit_label\">actual</span><hr />At 0 expected g was x", cell.Body);
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
            ClassicAssert.AreEqual("<<def<span class=\"fit_grey\"> ghi</span>", cell.Body);
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void CellContentWhenRecalling_Evaluate()
        {
            Parse cell = TestUtils.CreateCell("<<def");
            MakeStringFixture();
            StoreSymbol("def", "ghi");
            stringFixture.Field = "xyz";
            service.Compare(new TypedValue("xyz"), cell);
            ClassicAssert.AreEqual("<<def<span class=\"fit_grey\"> ghi</span>", cell.Body);
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestEvaluateRecallStringPass() {
            Parse cell = TestUtils.CreateCell("<<def");
            MakeStringFixture();
            StoreSymbol("def", "ghi");
            stringFixture.Field = "ghi";
            ClassicAssert.IsTrue(service.Compare(new TypedValue("ghi"), cell));
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestEvaluateRecallStringFail() {
            Parse cell = TestUtils.CreateCell("<<def");
            MakeStringFixture();
            StoreSymbol("def", "ghi");
            stringFixture.Field = "not ghi";
            ClassicAssert.IsFalse(service.Compare(new TypedValue("not ghi"), cell));
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestEvaluateRecallStringFailNull() {
            Parse cell = TestUtils.CreateCell("<<def");
            MakeStringFixture();
            StoreSymbol("def", "ghi");
            stringFixture.Field = null;
            ClassicAssert.IsFalse(service.Compare(new TypedValue(null, typeof(string)), cell));
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
            var person = new Person("Eeek", "Gadd");
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
            var person = new Person("Eeek", "Gadd");
            var person2 = new Person("Eeek", "Gadds");
            StoreSymbol("thePerson", person);
            personFixture.Field = person2;
            TestUtils.DoCheck(personFixture, TestUtils.CreateCellRange("Field"), cell);
            TestUtils.VerifyCounts(personFixture, 0, 1, 0, 0);
        }

        [Test]
        public void SymbolNamesWithWhitespace() {
            StoreSymbol("\r\n\tfoo\r\n", "bar");
            ClassicAssert.AreEqual("bar", LoadSymbol("foo"));
            ClassicAssert.AreEqual("bar", LoadSymbol("\r\n\tfoo\r\n"));
            ClassicAssert.AreEqual("bar", LoadSymbol("foo\r\n"));
        }

        private object LoadSymbol(string symbolName) {
            return service.Get<Symbols>().GetValue(symbolName);
        }

        private void StoreSymbol(string symbolName, object symbolValue) {
            service.Get<Symbols>().Save(symbolName, symbolValue);
        }

    }
}
