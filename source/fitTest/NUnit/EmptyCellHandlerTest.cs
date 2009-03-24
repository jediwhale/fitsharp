// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Engine;
using fit.Operators;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class EmptyCellHandlerTest
    {
        private Parse cell;

        [SetUp]
        public void SetUp() {
            cell = TestUtils.CreateCell("");
        }

        [Test]
        public void TestMatch() {
            Assert.IsTrue(TestUtils.IsMatch(new ExecuteEmpty(), ExecuteParameters.MakeCheck(TestUtils.CreateCell(""))));
            Assert.IsTrue(TestUtils.IsMatch(new ExecuteEmpty(), ExecuteParameters.MakeInput(TestUtils.CreateCellRange("stuff"), TestUtils.CreateCell(""))));
        }

        [Test]
        public void TestInputWhereNullValueExists() {
            StringFixture fixture = new StringFixture();
            fixture.CellOperation.Input(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual(null, fixture.Field);
            CellHandlerTestUtils.AssertValuesInBody(cell, new string[] {"fit_grey", "null"});
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestInputWhereBlankValueExists() {
            StringFixture fixture = new StringFixture();
            fixture.Field = "";
            fixture.CellOperation.Input(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("", fixture.Field);
            CellHandlerTestUtils.AssertValuesInBody(cell, new string[] {"fit_grey", "blank"});
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestInputWhereValueExists() {
            IntFixture fixture = new IntFixture();
            fixture.Field = 37;
            fixture.CellOperation.Input(fixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual(37, fixture.Field);
            CellHandlerTestUtils.AssertValuesInBody(cell, new string[] {"fit_grey", "37"});
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestInputNullValueWithMethod() {
            StringFixture fixture = new StringFixture();
            fixture.CellOperation.Input(fixture, TestUtils.CreateCellRange("Set"), cell);
            Assert.AreEqual(null, fixture.Field);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestCheckNullValue() {
            StringFixture fixture = new StringFixture();
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.AssertValuesInBody(cell, new string[] {"fit_grey", "null"});
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestCheckBlankValue() {
            StringFixture fixture = new StringFixture();
            fixture.Field = "";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.AssertValuesInBody(cell, new string[] {"fit_grey", "blank"});
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestCheckNonNullNonBlankValue() {
            StringFixture fixture = new StringFixture();
            fixture.Field = "a value";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("Field"), cell);
            CellHandlerTestUtils.AssertValuesInBody(cell, new string[] {"fit_grey", "a value"});
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 0, 0, 0);
        }
    }
}