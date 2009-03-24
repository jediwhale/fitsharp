// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class DefaultCellHandlerTest
    {

        [Test]
        public void TestDoInput()
        {
            Parse cell = TestUtils.CreateCell("xyz");
            StringFixture fixture = new StringFixture();
            fixture.CellOperation.Input(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("xyz", fixture.Field);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckCellRight()
        {
            Parse cell = TestUtils.CreateCell("xyz");
            StringFixture fixture = new StringFixture();
            fixture.Field = "xyz";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.IsTrue(cell.Tag.IndexOf("pass") > -1);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckCellWrong() {
            Parse cell = TestUtils.CreateCell("xyz");
            StringFixture fixture = new StringFixture();
            fixture.Field = "abc";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.IsTrue(cell.Tag.IndexOf("fail") > -1);
            Assert.IsTrue(cell.Body.IndexOf("abc") > -1);
            Assert.IsTrue(cell.Body.IndexOf("xyz") > -1);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestDoCheckCellWrongNull() {
            Parse cell = TestUtils.CreateCell("xyz");
            StringFixture fixture = new StringFixture();
            fixture.Field = null;
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.IsTrue(cell.Tag.IndexOf("fail") > -1);
            Assert.IsTrue(cell.Body.IndexOf("null") > -1);
            Assert.IsTrue(cell.Body.IndexOf("xyz") > -1);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }


        [Test]
        public void TestInvoke()
        {
            FixtureWithExecutableMethod.Calls = 0;
            Parse cell = TestUtils.CreateCell("do");
            FixtureWithExecutableMethod fixture = new FixtureWithExecutableMethod();
            fixture.CellOperation.TryInvoke(fixture, new CellRange(cell, 1));
            Assert.AreEqual(1, FixtureWithExecutableMethod.Calls);
        }

        [Test]
        public void TestEvaluateWrong() {
            Parse cell = TestUtils.CreateCell("xyz");
            StringFixture fixture = new StringFixture();
            fixture.Field = "abc";
            Assert.IsFalse(fixture.CellOperation.Compare(new TypedValue("abc"), cell));
            Assert.IsFalse(cell.Tag.IndexOf("fail") > -1);
            Assert.IsFalse(cell.Body.IndexOf("abc") > -1);
            Assert.IsTrue(cell.Body.IndexOf("xyz") > -1);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
			
        }

        [Test]
        public void TestEvaluateRight() {
            Parse cell = TestUtils.CreateCell("xyz");
            StringFixture fixture = new StringFixture();
            fixture.Field = "xyz";
            Assert.IsTrue(fixture.CellOperation.Compare(new TypedValue("xyz"), cell));
            Assert.IsFalse(cell.Tag.IndexOf("pass") > -1);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        class FixtureWithExecutableMethod : Fixture
        {
            public static int Calls = 0;

            public void Do()
            {
                Calls++;
            }
        }

    }
}