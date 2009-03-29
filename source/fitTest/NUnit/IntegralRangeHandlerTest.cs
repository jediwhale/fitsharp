// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Operators;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class IntegralRangeHandlerTest: CellOperatorTest
    {
        private Parse cell;

        [SetUp]
        public void SetUp() {
            cell = TestUtils.CreateCell("0..2");
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
            return IsMatch(new CompareIntegralRange(), 777, typeof (int), input);
        }

        [Test]
        public void TestInRange() {
            MakeFixture();
            intFixture.Field = 1;
            intFixture.CellOperation.Check(intFixture, TestUtils.CreateCellRange("Field"), cell);
            VerifyCounts(intFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestStartOfRange() {
            MakeFixture();
            intFixture.Field = 0;
            intFixture.CellOperation.Check(intFixture, TestUtils.CreateCellRange("Field"), cell);
            VerifyCounts(intFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestEndOfRange() {
            MakeFixture();
            intFixture.Field = 2;
            intFixture.CellOperation.Check(intFixture, TestUtils.CreateCellRange("Field"), cell);
            VerifyCounts(intFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestNotInRange() {
            MakeFixture();
            intFixture.Field = 5;
            intFixture.CellOperation.Check(intFixture, TestUtils.CreateCellRange("Field"), cell);
            AssertCellFails(cell);
            VerifyCounts(intFixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestNegativeNumbers() {
            cell = TestUtils.CreateCell("-457..-372");
            MakeFixture();
            intFixture.Field = -400;
            intFixture.CellOperation.Check(intFixture, TestUtils.CreateCellRange("Field"), cell);
            AssertCellPasses(cell);
            VerifyCounts(intFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestNegativeLowPositiveHigh() {
            cell = TestUtils.CreateCell("-457..372");
            MakeFixture();
            intFixture.Field = 0;
            intFixture.CellOperation.Check(intFixture, TestUtils.CreateCellRange("Field"), cell);
            AssertCellPasses(cell);
            VerifyCounts(intFixture, 1, 0, 0, 0);
        }

        private void MakeFixture() {
            MakeIntFixture();
            service.AddOperator(typeof(CompareIntegralRange).FullName);
        }
    }
}