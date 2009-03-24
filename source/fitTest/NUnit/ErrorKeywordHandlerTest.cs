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
    public class ErrorKeywordHandlerTest
    {
        [Test]
        public void MatchesErrorKeyword()
        {
            Assert.IsTrue(TestUtils.IsMatch(new ExecuteError(), ExecuteParameters.MakeCheck(TestUtils.CreateCell("error"))));
            Assert.IsFalse(TestUtils.IsMatch(new ExecuteError(), ExecuteParameters.MakeInput(TestUtils.CreateCell("stuff"), TestUtils.CreateCell("error"))));
        }

        [Test]
        public void TestDoCheckErrorRight()
        {
            Parse cell = TestUtils.CreateCell("error");
            ErrorThrowingFixture fixture = new ErrorThrowingFixture();
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("ErrorThrowingMethod"), cell);
            Assert.IsTrue(cell.Tag.IndexOf("pass") > -1);
            Assert.IsTrue(cell.Body.IndexOf("error") > -1);
            CellHandlerTestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckErrorWrong()
        {
            Parse cell = TestUtils.CreateCell("error");
            StringFixture fixture = new StringFixture();
            fixture.Field = "some value";
            fixture.CellOperation.Check(fixture, TestUtils.CreateCellRange("field"), cell);
            Assert.IsTrue(cell.Tag.IndexOf("fail") > -1);
            Assert.IsTrue(cell.Body.IndexOf("error") > -1);
            Assert.IsTrue(cell.Body.IndexOf("some value") > -1);
            CellHandlerTestUtils.VerifyCounts(fixture, 0, 1, 0, 0);
        }
    }
}