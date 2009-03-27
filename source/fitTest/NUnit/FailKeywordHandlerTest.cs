// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Operators;
using fit.Test.Acceptance;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class FailKeywordHandlerTest
    {
        [Test]
        public void TestMatch()
        {
            Assert.IsTrue(IsMatch("fail[]"));
            Assert.IsTrue(IsMatch("fail[some value]"));
        }

        private static bool IsMatch(string input) {
            return TestUtils.IsMatch(new CompareFail(), "stuff", typeof (object), input);
        }

        [Test]
        public void TestFailInt()
        {
            Parse cell = TestUtils.CreateCell("fail[1]");
            IntFixture fixture = new IntFixture();
            fixture.Field = 2;
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("field"), cell);
            Assert.IsTrue(cell.Tag.IndexOf("pass") > -1);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestFailOnCorrectInt()
        {
            Parse cell = TestUtils.CreateCell("fail[2]");
            IntFixture fixture = new IntFixture();
            fixture.Field = 2;
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("field"), cell);
            Assert.IsTrue(cell.Tag.IndexOf("fail") > -1);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestFailString()
        {
            Parse cell = TestUtils.CreateCell("fail[some string]");
            StringFixture fixture = new StringFixture();
            fixture.Field = "some other string";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("field"), cell);
            Assert.IsTrue(cell.Tag.IndexOf("pass") > -1);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestFailPerson()
        {
            Parse cell = TestUtils.CreateCell("fail[Doctor Jeckyll]");
            PersonFixture fixture = new PersonFixture();
            fixture.Field = new Person("Mister", "Hyde");
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("field"), cell);
            Assert.IsTrue(cell.Tag.IndexOf("pass") > -1);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }
    }
}