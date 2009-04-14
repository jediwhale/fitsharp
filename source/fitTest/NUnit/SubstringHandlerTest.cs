// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Operators;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class SubstringHandlerTest: CellOperatorTest
    {
        private Parse cell;

        [Test]
        public void TestMatch()
        {
            Assert.IsTrue(IsMatch("..text.."));
            Assert.IsFalse(IsMatch("text.."));
            Assert.IsFalse(IsMatch("..text"));
        }

        private static bool IsMatch(string input) {
            return IsMatch(new CompareSubstring(), null, typeof (object), input);
        }

        [Test]
        public void TestPass() {
            MakeFixture();
            stringFixture.Field = "abcde";
            cell = TestUtils.CreateCell("..bcd..");
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            AssertCellPasses(cell);
            VerifyCounts(stringFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestStartsWithPass() {
            MakeFixture();
            stringFixture.Field = "abcde";
            cell = TestUtils.CreateCell("..abc..");
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            AssertCellPasses(cell);
            VerifyCounts(stringFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestEndsWithPass() {
            MakeFixture();
            stringFixture.Field = "abcde";
            cell = TestUtils.CreateCell("..cde..");
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            AssertCellPasses(cell);
            VerifyCounts(stringFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestFails() {
            MakeFixture();
            stringFixture.Field = "abcde";
            cell = TestUtils.CreateCell("..bce..");
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            AssertCellFails(cell);
            VerifyCounts(stringFixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestFailNull() {
            MakeFixture();
            stringFixture.Field = null;
            cell = TestUtils.CreateCell("..bce..");
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            AssertCellFails(cell);
            AssertValueInBody(cell, "null");
            VerifyCounts(stringFixture, 0, 1, 0, 0);
        }

        private void MakeFixture() {
            MakeStringFixture();
            service.AddOperator(new CompareSubstring());
        }
    }
}