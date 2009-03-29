// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Engine;
using fit.Operators;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class EmptyCellHandlerTest: CellOperatorTest
    {
        private Parse cell;

        [SetUp]
        public void SetUp() {
            cell = TestUtils.CreateCell("");
        }

        [Test]
        public void TestMatch() {
            Assert.IsTrue(IsMatch(new ExecuteEmpty(), ExecuteParameters.MakeCheck(TestUtils.CreateCell(""))));
            Assert.IsTrue(IsMatch(new ExecuteEmpty(), ExecuteParameters.MakeInput(TestUtils.CreateCellRange("stuff"), TestUtils.CreateCell(""))));
        }

        [Test]
        public void TestInputWhereNullValueExists() {
            MakeStringFixture();
            stringFixture.CellOperation.Input(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual(null, stringFixture.Field);
            AssertValuesInBody(cell, new string[] {"fit_grey", "null"});
            VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestInputWhereBlankValueExists() {
            MakeStringFixture();
            stringFixture.Field = "";
            stringFixture.CellOperation.Input(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual("", stringFixture.Field);
            AssertValuesInBody(cell, new string[] {"fit_grey", "blank"});
            VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestInputWhereValueExists() {
            MakeIntFixture();
            intFixture.Field = 37;
            intFixture.CellOperation.Input(intFixture, TestUtils.CreateCellRange("Field"), cell);
            Assert.AreEqual(37, intFixture.Field);
            AssertValuesInBody(cell, new string[] {"fit_grey", "37"});
            VerifyCounts(intFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestInputNullValueWithMethod() {
            MakeStringFixture();
            stringFixture.CellOperation.Input(stringFixture, TestUtils.CreateCellRange("Set"), cell);
            Assert.AreEqual(null, stringFixture.Field);
            VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestCheckNullValue() {
            MakeStringFixture();
            stringFixture.CellOperation.Check(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            AssertValuesInBody(cell, new string[] {"fit_grey", "null"});
            VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestCheckBlankValue() {
            MakeStringFixture();
            stringFixture.Field = "";
            stringFixture.CellOperation.Check(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            AssertValuesInBody(cell, new string[] {"fit_grey", "blank"});
            VerifyCounts(stringFixture, 0, 0, 0, 0);
        }

        [Test]
        public void TestCheckNonNullNonBlankValue() {
            MakeStringFixture();
            stringFixture.Field = "a value";
            stringFixture.CellOperation.Check(stringFixture, TestUtils.CreateCellRange("Field"), cell);
            AssertValuesInBody(cell, new string[] {"fit_grey", "a value"});
            VerifyCounts(stringFixture, 0, 0, 0, 0);
        }
    }
}