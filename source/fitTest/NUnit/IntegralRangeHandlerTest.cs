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
    public class IntegralRangeHandlerTest
    {
        private Parse cell;

        [SetUp]
        public void SetUp() {
            cell = TestUtils.CreateCell("0..2");
            Context.Configuration.GetItem<Service>().AddOperator(typeof(CompareIntegralRange).FullName);
        }

        [Test]
        public void MatchesInput() {
            Assert.IsTrue(IsMatch("0..2"));
            Assert.IsFalse(IsMatch("..2"));
            Assert.IsFalse(IsMatch(".."));
            Assert.IsTrue(IsMatch("-10..-2"));
            Assert.IsTrue(IsMatch("-10..23"));
            Assert.IsTrue(IsMatch("12..37"));
            Assert.IsFalse(IsMatch("a..b"));
        }

        private static bool IsMatch(string input) {
            return TestUtils.IsMatch(new CompareIntegralRange(), 777, typeof (int), input);
        }

        [Test]
        public void TestInRange() {
            IntFixture fixture = new IntFixture();
            fixture.Field = 1;
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestStartOfRange() {
            IntFixture fixture = new IntFixture();
            fixture.Field = 0;
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestEndOfRange() {
            IntFixture fixture = new IntFixture();
            fixture.Field = 2;
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestNotInRange() {
            IntFixture fixture = new IntFixture();
            fixture.Field = 5;
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.AssertCellFails(cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestNegativeNumbers() {
            cell = TestUtils.CreateCell("-457..-372");
            IntFixture fixture = new IntFixture();
            fixture.Field = -400;
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.AssertCellPasses(cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestNegativeLowPositiveHigh() {
            cell = TestUtils.CreateCell("-457..372");
            IntFixture fixture = new IntFixture();
            fixture.Field = 0;
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.AssertCellPasses(cell);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }
    }
}