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
    public class ParseBlankTest
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
            Assert.IsTrue(CellHandlerTestUtils.IsMatch(new ParseBlank(), "blank"));
            Assert.IsFalse(CellHandlerTestUtils.IsMatch(new ParseBlank(), "is blank"));
        }

        [Test]
        public void TestDoInputBlank()
        {
            StringFixture fixture = new StringFixture();
            fixture.CellOperation.Input(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("", fixture.Field);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankRight() {
            StringFixture fixture = new StringFixture();
            fixture.Field = "";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("", fixture.Field);
            CellHandlerTestUtils.AssertCellPasses(cell);
            CellHandlerTestUtils.AssertValueInBody(cell, "blank");
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoEvaluateBlankRight() {
            StringFixture fixture = new StringFixture();
            fixture.Field = "";
            Assert.IsTrue(fixture.CellOperation.Compare(new TypedValue(string.Empty), cell));
            Assert.AreEqual("", fixture.Field);
            CellHandlerTestUtils.AssertValueInBody(cell, "blank");
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankWrongValue()
        {
            StringFixture fixture = new StringFixture();
            fixture.Field = "some value";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("some value", fixture.Field);
            CellHandlerTestUtils.AssertCellFails(cell);
            CellHandlerTestUtils.AssertValuesInBody(cell, new string[] {"blank", "some value"});
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankNullValue()
        {
            StringFixture fixture = new StringFixture();
            fixture.Field = null;
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual(null, fixture.Field);
            CellHandlerTestUtils.AssertCellFails(cell);
            CellHandlerTestUtils.AssertValuesInBody(cell, new string[] {"blank", "null"});
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankWrongTypeRightValue()
        {
            PersonFixture fixture = new PersonFixture();
            fixture.Field = new Person("", "");
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("", fixture.Field.ToString());
            CellHandlerTestUtils.AssertCellPasses(cell);
            CellHandlerTestUtils.AssertValuesInBody(cell, new string[] {"blank"});
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckBlankWrongTypeWrongValue() {
            PersonFixture fixture = new PersonFixture();
            fixture.Field = new Person("john", "doe");
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("john doe", fixture.Field.ToString());
            CellHandlerTestUtils.AssertCellFails(cell);
            CellHandlerTestUtils.AssertValuesInBody(cell, new string[] {"blank", "john doe"});
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }
    }
}