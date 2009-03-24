// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Application;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class SubstringHandlerTest
    {
        private Parse cell;

        [SetUp]
        public void SetUp()
        {
            Context.Configuration.GetItem<Service>().AddOperator(new CompareSubstring());
        }

        [Test]
        public void TestMatch()
        {
            Assert.IsTrue(IsMatch("..text.."));
            Assert.IsFalse(IsMatch("text.."));
            Assert.IsFalse(IsMatch("..text"));
        }

        private static bool IsMatch(string input) {
            return TestUtils.IsMatch(new CompareSubstring(), null, typeof (object), input);
        }

        [Test]
        public void TestPass() {
            StringFixture fixture = new StringFixture();
            fixture.Field = "abcde";
            cell = TestUtils.CreateCell("..bcd..");
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.AssertCellPasses(cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestStartsWithPass() {
            StringFixture fixture = new StringFixture();
            fixture.Field = "abcde";
            cell = TestUtils.CreateCell("..abc..");
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.AssertCellPasses(cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestEndsWithPass() {
            StringFixture fixture = new StringFixture();
            fixture.Field = "abcde";
            cell = TestUtils.CreateCell("..cde..");
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.AssertCellPasses(cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestFails() {
            StringFixture fixture = new StringFixture();
            fixture.Field = "abcde";
            cell = TestUtils.CreateCell("..bce..");
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.AssertCellFails(cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestFailNull() {
            StringFixture fixture = new StringFixture();
            fixture.Field = null;
            cell = TestUtils.CreateCell("..bce..");
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.AssertCellFails(cell);
            CellHandlerTestUtils.AssertValueInBody(cell, "null");
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }
    }
}