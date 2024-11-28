// Copyright � 2011 Syterra Software Inc. Includes work by Object Mentor, Inc., � 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Operators;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fit.Test.NUnit {
    [TestFixture]
    public class ExceptionKeywordHandlerTest: CellOperatorTest
    {
        private ExceptionThrowingFixture exceptionFixture;

        [Test]
        public void TestMatch()
        {
            ClassicAssert.IsTrue(IsMatch(new CompareException(), "exception[]"));
            ClassicAssert.IsTrue(IsMatch(new CompareException(), "exception[NullPointerException]"));
        }


        [Test]
        public void TestDoCheckExceptionClassRight()
        {
            Parse cell = TestUtils.CreateCell("exception[NullReferenceException]");
            MakeExceptionFixture();
            TestUtils.DoCheck(exceptionFixture, TestUtils.CreateCellRange("ThrowNullReferenceException"), cell);
            AssertCellPasses(cell);
            TestUtils.VerifyCounts(exceptionFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckErrorClassWrong()
        {
            Parse cell = TestUtils.CreateCell("exception[NullReferenceException]");
            MakeExceptionFixture();
            TestUtils.DoCheck(exceptionFixture, TestUtils.CreateCellRange("ThrowApplicationException"), cell);
            AssertCellFails(cell);
            TestUtils.VerifyCounts(exceptionFixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestDoCheckExceptionMessageRight()
        {
            Parse cell = TestUtils.CreateCell("exception[\"an exception\"]");
            MakeExceptionFixture();
            exceptionFixture.Message = "an exception";
            TestUtils.DoCheck(exceptionFixture, TestUtils.CreateCellRange("ThrowApplicationException"), cell);
            AssertCellPasses(cell);
            TestUtils.VerifyCounts(exceptionFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckExceptionMessageAndClassRight()
        {
            Parse cell = TestUtils.CreateCell("exception[ApplicationException: \"an exception\"]");
            MakeExceptionFixture();
            exceptionFixture.Message = "an exception";
            TestUtils.DoCheck(exceptionFixture, TestUtils.CreateCellRange("ThrowApplicationException"), cell);
            AssertCellPasses(cell);
            TestUtils.VerifyCounts(exceptionFixture, 1, 0, 0, 0);
        }

        private void MakeExceptionFixture() {
            service = new Service.Service();
            exceptionFixture = new ExceptionThrowingFixture { Processor = service };
        }
    }
}