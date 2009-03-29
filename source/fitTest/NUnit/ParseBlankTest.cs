// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Test.Acceptance;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
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
            stringFixture.CellOperation.Input(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("", stringFixture.Field);
            VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankRight() {
            MakeStringFixture();
            stringFixture.Field = "";
            stringFixture.CellOperation.Check(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("", stringFixture.Field);
            AssertCellPasses(cell);
            AssertValueInBody(cell, "blank");
            VerifyCounts(stringFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoEvaluateBlankRight() {
            MakeStringFixture();
            stringFixture.Field = "";
            Assert.IsTrue(stringFixture.CellOperation.Compare(new TypedValue(string.Empty), cell));
            Assert.AreEqual("", stringFixture.Field);
            AssertValueInBody(cell, "blank");
            VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankWrongValue()
        {
            MakeStringFixture();
            stringFixture.Field = "some value";
            stringFixture.CellOperation.Check(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("some value", stringFixture.Field);
            AssertCellFails(cell);
            AssertValuesInBody(cell, new string[] {"blank", "some value"});
            VerifyCounts(stringFixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankNullValue()
        {
            MakeStringFixture();
            stringFixture.Field = null;
            stringFixture.CellOperation.Check(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual(null, stringFixture.Field);
            AssertCellFails(cell);
            AssertValuesInBody(cell, new string[] {"blank", "null"});
            VerifyCounts(stringFixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankWrongTypeRightValue()
        {
            MakePersonFixture();
            personFixture.Field = new Person("", "");
            personFixture.CellOperation.Check(personFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("", personFixture.Field.ToString());
            AssertCellPasses(cell);
            AssertValuesInBody(cell, new string[] {"blank"});
            VerifyCounts(personFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankWrongTypeWrongValue() {
            MakePersonFixture();
            personFixture.Field = new Person("john", "doe");
            personFixture.CellOperation.Check(personFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("john doe", personFixture.Field.ToString());
            AssertCellFails(cell);
            AssertValuesInBody(cell, new string[] {"blank", "john doe"});
            VerifyCounts(personFixture, 0, 1, 0, 0);
        }
    }
}