// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Operators;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class NullKeywordHandlerTest: CellOperatorTest
    {	
        [Test]
        public void MatchesNullString() {
            Assert.IsTrue(IsMatch(new ParseNull(), "null"));
        }

        [Test]
        public void TestDoInputNull()
        {
            Parse cell = TestUtils.CreateCell("null");
            MakeStringFixture();
            stringFixture.CellOperation.Input(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual(null, stringFixture.Field);
            VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckNullRight()
        {
            Parse cell = TestUtils.CreateCell("null");
            MakeStringFixture();
            stringFixture.Field = null;
            stringFixture.CellOperation.Check(stringFixture, TestUtils.CreateCellRange("field"), cell);
            Assert.AreEqual(null, stringFixture.Field);
            AssertCellPasses(cell);
            Assert.IsTrue(cell.Body.IndexOf("null") > -1);
            VerifyCounts(stringFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckNullWrong()
        {
            Parse cell = TestUtils.CreateCell("null");
            MakeStringFixture();
            stringFixture.Field = "some value";
            stringFixture.CellOperation.Check(stringFixture, TestUtils.CreateCellRange("field"), cell);
            Assert.AreEqual("some value", stringFixture.Field);
            AssertCellFails(cell);
            Assert.IsTrue(cell.Body.IndexOf("null") > -1);
            Assert.IsTrue(cell.Body.IndexOf("some value") > -1);
            VerifyCounts(stringFixture, 0, 1, 0, 0);
        }
    }
}