// Copyright © 2010 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Model;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;
using fitSharp.Fit.Service;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class DefaultCellHandlerTest: CellOperatorTest
    {

        [Test]
        public void TestDoInput()
        {
            Parse cell = TestUtils.CreateCell("xyz");
            MakeStringFixture();
            TestUtils.DoInput(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("xyz", stringFixture.Field);
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckCellRight()
        {
            Parse cell = TestUtils.CreateCell("xyz");
            MakeStringFixture();
            stringFixture.Field = "xyz";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            AssertCellPasses(cell);
            TestUtils.VerifyCounts(stringFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckCellWrong() {
            Parse cell = TestUtils.CreateCell("xyz");
            MakeStringFixture();
            stringFixture.Field = "abc";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            AssertCellFails(cell);
            Assert.IsTrue(cell.Body.IndexOf("abc") > -1);
            Assert.IsTrue(cell.Body.IndexOf("xyz") > -1);
            TestUtils.VerifyCounts(stringFixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestDoCheckCellWrongNull() {
            Parse cell = TestUtils.CreateCell("xyz");
            MakeStringFixture();
            stringFixture.Field = null;
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            AssertCellFails(cell);
            Assert.IsTrue(cell.Body.IndexOf("null") > -1);
            Assert.IsTrue(cell.Body.IndexOf("xyz") > -1);
            TestUtils.VerifyCounts(stringFixture, 0, 1, 0, 0);
        }


        [Test]
        public void TestInvoke()
        {
            FixtureWithExecutableMethod.Calls = 0;
            Parse cell = TestUtils.CreateCell("do");
            service = new Service.Service();
            var fixture = new FixtureWithExecutableMethod {Processor = service};
            fixture.CellOperation.TryInvoke(fixture, new CellRange(cell, 1));
            Assert.AreEqual(1, FixtureWithExecutableMethod.Calls);
        }

        [Test]
        public void TestEvaluateWrong() {
            Parse cell = TestUtils.CreateCell("xyz");
            MakeStringFixture();
            stringFixture.Field = "abc";
            Assert.IsFalse(stringFixture.CellOperation.Compare(new TypedValue("abc"), cell));
            Assert.AreNotEqual(TestStatus.Wrong, cell.GetAttribute(CellAttribute.Status));
            Assert.IsFalse(cell.Body.IndexOf("abc") > -1);
            Assert.IsTrue(cell.Body.IndexOf("xyz") > -1);
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
			
        }

        [Test]
        public void TestEvaluateRight() {
            Parse cell = TestUtils.CreateCell("xyz");
            MakeStringFixture();
            stringFixture.Field = "xyz";
            Assert.IsTrue(stringFixture.CellOperation.Compare(new TypedValue("xyz"), cell));
            Assert.AreNotEqual(TestStatus.Right, cell.GetAttribute(CellAttribute.Status));
            TestUtils.VerifyCounts(stringFixture, 0, 0, 0, 0);
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
