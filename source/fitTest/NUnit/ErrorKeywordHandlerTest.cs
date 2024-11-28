// Copyright � 2011 Syterra Software Inc. Includes work by Object Mentor, Inc., � 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Operators;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fit.Test.NUnit {
    [TestFixture]
    public class ErrorKeywordHandlerTest: CellOperatorTest
    {
        private ErrorThrowingFixture fixture;

        [Test]
        public void MatchesErrorKeyword()
        {
            ClassicAssert.IsTrue(IsMatch(new CompareError(), "error"));
        }

        [Test]
        public void TestDoCheckErrorRight()
        {
            Parse cell = TestUtils.CreateCell("error");
            MakeErrorFixture();
            TestUtils.DoCheck(fixture, TestUtils.CreateCellRange("ErrorThrowingMethod"), cell);
            AssertCellPasses(cell);
            ClassicAssert.IsTrue(cell.Body.IndexOf("error") > -1);
            TestUtils.VerifyCounts(fixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckErrorWrong()
        {
            Parse cell = TestUtils.CreateCell("error");
            MakeStringFixture();
            stringFixture.Field = "some value";
            TestUtils.DoCheck(stringFixture, TestUtils.CreateCellRange("field"), cell);
            AssertCellFails(cell);
            ClassicAssert.IsTrue(cell.Body.IndexOf("error") > -1);
            ClassicAssert.IsTrue(cell.Body.IndexOf("some value") > -1);
            TestUtils.VerifyCounts(stringFixture, 0, 1, 0, 0);
        }

        private void MakeErrorFixture() {
            service = new Service.Service();
            fixture = new ErrorThrowingFixture { Processor = service };
        }
    }
}