// Copyright © 2011 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using fitSharp.Test.Double;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class ParseBlankTest: CellOperatorTest
    {
        private Parse cell;

        [SetUp]
        public void SetUp()
        {
            cell = TestUtils.CreateCell("blank");
        }

        [Test]
        public void MatchesBlankString()
        {
            Assert.IsTrue(IsMatch(new ParseBlank(), "blank"));
            Assert.IsFalse(IsMatch(new ParseBlank(), "is blank"));
        }

        [Test]
        public void TestDoInputBlank()
        {
            MakeStringFixture();
            TestUtils.DoInput(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("", stringFixture.Field);
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankRight() {
            MakeStringFixture();
            stringFixture.Field = "";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("", stringFixture.Field);
            AssertCellPasses(cell);
            AssertValueInBody(cell, "blank");
            TestUtils.VerifyCounts(stringFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoEvaluateBlankRight() {
            MakeStringFixture();
            stringFixture.Field = "";
            Assert.IsTrue(service.Compare(new TypedValue(string.Empty), cell));
            Assert.AreEqual("", stringFixture.Field);
            AssertValueInBody(cell, "blank");
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankWrongValue()
        {
            MakeStringFixture();
            stringFixture.Field = "some value";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("some value", stringFixture.Field);
            AssertCellFails(cell);
            AssertValuesInBody(cell, new[] {"blank", "some value"});
            TestUtils.VerifyCounts(stringFixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankNullValue()
        {
            MakeStringFixture();
            stringFixture.Field = null;
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual(null, stringFixture.Field);
            AssertCellFails(cell);
            AssertValuesInBody(cell, new[] {"blank", "null"});
            TestUtils.VerifyCounts(stringFixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankWrongTypeRightValue()
        {
            MakePersonFixture();
            personFixture.Field = new Person("", "");
            TestUtils.DoCheck(personFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("", personFixture.Field.ToString());
            AssertCellPasses(cell);
            AssertValuesInBody(cell, new[] {"blank"});
            TestUtils.VerifyCounts(personFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankWrongTypeWrongValue() {
            MakePersonFixture();
            personFixture.Field = new Person("john", "doe");
            TestUtils.DoCheck(personFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("john doe", personFixture.Field.ToString());
            AssertCellFails(cell);
            AssertValuesInBody(cell, new[] {"blank", "john doe"});
            TestUtils.VerifyCounts(personFixture, 0, 1, 0, 0);
        }
    }
}