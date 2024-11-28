// Copyright � 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., � 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Operators;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fit.Test.NUnit {
    [TestFixture]
    public class NullKeywordHandlerTest: CellOperatorTest
    {	
        [Test]
        public void MatchesNullString() {
            ClassicAssert.IsTrue(IsMatch(new ParseNull(), "null"));
        }

        [Test]
        public void TestDoInputNull()
        {
            Parse cell = TestUtils.CreateCell("null");
            MakeStringFixture();
            TestUtils.DoInput(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            ClassicAssert.AreEqual(null, stringFixture.Field);
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckNullRight()
        {
            Parse cell = TestUtils.CreateCell("null");
            MakeStringFixture();
            stringFixture.Field = null;
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("field"), cell);
            ClassicAssert.AreEqual(null, stringFixture.Field);
            AssertCellPasses(cell);
            ClassicAssert.IsTrue(cell.Body.IndexOf("null") > -1);
            TestUtils.VerifyCounts(stringFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckNullWrong()
        {
            Parse cell = TestUtils.CreateCell("null");
            MakeStringFixture();
            stringFixture.Field = "some value";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("field"), cell);
            ClassicAssert.AreEqual("some value", stringFixture.Field);
            AssertCellFails(cell);
            ClassicAssert.IsTrue(cell.Body.IndexOf("null") > -1);
            ClassicAssert.IsTrue(cell.Body.IndexOf("some value") > -1);
            TestUtils.VerifyCounts(stringFixture, 0, 1, 0, 0);
        }
    }
}