// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Engine;
using fit.Operators;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class ExceptionKeywordHandlerTest: CellOperatorTest
    {
        private ExceptionThrowingFixture exceptionFixture;

        [Test]
        public void TestMatch()
        {
            Assert.IsTrue(IsMatch(new ExecuteException(), ExecuteParameters.MakeCheck(TestUtils.CreateCell("exception[]"))));
            Assert.IsTrue(IsMatch(new ExecuteException(), ExecuteParameters.MakeCheck(TestUtils.CreateCell("exception[NullPointerException]"))));
            Assert.IsFalse(IsMatch(new ExecuteException(), ExecuteParameters.MakeInput(TestUtils.CreateCell("stuff"), TestUtils.CreateCell("exception[]"))));
        }


        [Test]
        public void TestDoCheckExceptionClassRight()
        {
            //???ObjectFactory.AddNamespace("System");
            Parse cell = TestUtils.CreateCell("exception[NullReferenceException]");
            MakeExceptionFixture();
            exceptionFixture.CellOperation.Check(exceptionFixture, TestUtils.CreateCellRange("ThrowNullReferenceException"), cell);
            Assert.IsTrue(cell.Tag.IndexOf("pass") > -1);
            VerifyCounts(exceptionFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckErrorClassWrong()
        {
            //???ObjectFactory.AddNamespace("System");
            Parse cell = TestUtils.CreateCell("exception[NullReferenceException]");
            MakeExceptionFixture();
            exceptionFixture.CellOperation.Check(exceptionFixture, TestUtils.CreateCellRange("ThrowApplicationException"), cell);
            Assert.IsTrue(cell.Tag.IndexOf("fail") > -1);
            VerifyCounts(exceptionFixture, 0, 1, 0, 0);
        }

        [Test]
        public void TestDoCheckExceptionMessageRight()
        {
            //???ObjectFactory.AddNamespace("System");
            Parse cell = TestUtils.CreateCell("exception[\"an exception\"]");
            MakeExceptionFixture();
            exceptionFixture.Message = "an exception";
            exceptionFixture.CellOperation.Check(exceptionFixture, TestUtils.CreateCellRange("ThrowApplicationException"), cell);
            Assert.IsTrue(cell.Tag.IndexOf("pass") > -1);
            VerifyCounts(exceptionFixture, 1, 0, 0, 0);
        }

        [Test]
        public void TestDoCheckExceptionMessageAndClassRight()
        {
            //???ObjectFactory.AddNamespace("System");
            Parse cell = TestUtils.CreateCell("exception[ApplicationException: \"an exception\"]");
            MakeExceptionFixture();
            exceptionFixture.Message = "an exception";
            exceptionFixture.CellOperation.Check(exceptionFixture, TestUtils.CreateCellRange("ThrowApplicationException"), cell);
            Assert.IsTrue(cell.Tag.IndexOf("pass") > -1);
            VerifyCounts(exceptionFixture, 1, 0, 0, 0);
        }

        private void MakeExceptionFixture() {
            service = new Service();
            exceptionFixture = new ExceptionThrowingFixture { Service = service };
        }
    }
}