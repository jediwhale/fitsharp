// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Operators;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class NullKeywordHandlerTest
    {	
        [Test]
        public void MatchesNullString() {
            Assert.IsTrue(CellHandlerTestUtils.IsMatch(new ParseNull(), "null"));
        }

        [Test]
        public void TestDoInputNull()
        {
            Parse cell = TestUtils.CreateCell("null");
            StringFixture fixture = new StringFixture();
            fixture.CellOperation.Input(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual(null, fixture.Field);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckNullRight()
        {
            Parse cell = TestUtils.CreateCell("null");
            StringFixture fixture = new StringFixture();
            fixture.Field = null;
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("field"), cell);
            Assert.AreEqual(null, fixture.Field);
            Assert.IsTrue(cell.Tag.IndexOf("pass") > -1);
            Assert.IsTrue(cell.Body.IndexOf("null") > -1);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckNullWrong()
        {
            Parse cell = TestUtils.CreateCell("null");
            StringFixture fixture = new StringFixture();
            fixture.Field = "some value";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("field"), cell);
            Assert.AreEqual("some value", fixture.Field);
            Assert.IsTrue(cell.Tag.IndexOf("fail") > -1);
            Assert.IsTrue(cell.Body.IndexOf("null") > -1);
            Assert.IsTrue(cell.Body.IndexOf("some value") > -1);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }
    }
}