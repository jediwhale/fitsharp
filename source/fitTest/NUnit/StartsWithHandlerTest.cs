// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Application;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class StartsWithHandlerTest
    {
        private Parse cell;

        [SetUp]
        public void SetUp()
        {
            cell = TestUtils.CreateCell("abc..");
            Context.Configuration.GetItem<Service>().AddOperator(new CompareSubstring());
            Context.Configuration.GetItem<Service>().AddOperator(new CompareStartsWith());
        }

        [Test]
        public void TestMatches()
        {
            Assert.IsTrue(IsMatch("abc.."));
            Assert.IsFalse(IsMatch("..abc"));
            Assert.IsFalse(IsMatch("..abc.."));
        }

        private static bool IsMatch(string input) {
            return TestUtils.IsMatch(new CompareStartsWith(), null, typeof (object), input);
        }

        [Test]
        public void TestPass()
        {
            StringFixture fixture = new StringFixture();
            fixture.Field = "abcde";
            cell = TestUtils.CreateCell("abc..");
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.AssertCellPasses(cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestFailWhereSubstringExists() {
            StringFixture fixture = new StringFixture();
            fixture.Field = "abcde";
            cell = TestUtils.CreateCell("bcd..");
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.AssertCellFails(cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestFailNull() {
            StringFixture fixture = new StringFixture();
            fixture.Field = null;
            cell = TestUtils.CreateCell("bcd..");
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.AssertValueInBody(cell, "null");
            CellHandlerTestUtils.AssertCellFails(cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }
    }
}