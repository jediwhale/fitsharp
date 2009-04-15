// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Operators;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class FailKeywordHandlerTest: CellOperatorTest
    {
        [Test]
        public void TestMatch()
        {
            Assert.IsTrue(IsMatch("fail[]"));
            Assert.IsTrue(IsMatch("fail[some value]"));
        }

        private static bool IsMatch(string input) {
            return IsMatch(new CompareFail(), "stuff", typeof (object), input);
        }

        [Test]
        public void TestFailInt()
        {
            Parse cell = TestUtils.CreateCell("fail[1]");
            MakeIntFixture();
            intFixture.Field = 2;
            TestUtils.DoCheck(intFixture, TestUtils.CreateCellRange("field"), cell);
            AssertCellPasses(cell);
            TestUtils.VerifyCounts(intFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestFailOnCorrectInt()
        {
            Parse cell = TestUtils.CreateCell("fail[2]");
            MakeIntFixture();
            intFixture.Field = 2;
            TestUtils.DoCheck(intFixture, TestUtils.CreateCellRange("field"), cell);
            AssertCellFails(cell);
            TestUtils.VerifyCounts(intFixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestFailString()
        {
            Parse cell = TestUtils.CreateCell("fail[some string]");
            MakeStringFixture();
            stringFixture.Field = "some other string";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("field"), cell);
            AssertCellPasses(cell);
            TestUtils.VerifyCounts(stringFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestFailPerson()
        {
            Parse cell = TestUtils.CreateCell("fail[Doctor Jeckyll]");
            MakePersonFixture();
            personFixture.Field = new Person("Mister", "Hyde");
            TestUtils.DoCheck(personFixture, TestUtils.CreateCellRange("field"), cell);
            AssertCellPasses(cell);
            TestUtils.VerifyCounts(personFixture, 1, 0, 0, 0);
        }
    }
}