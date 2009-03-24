// FitNesse.NET
// Copyright © 2008,2009 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    public class CellHandlerTestUtils
    {
        public static bool IsMatch(ParseOperator<Cell> parseOperator, string input) {
            TypedValue result = TypedValue.Void;
            var processor = new Processor<Cell>();
            processor.AddMemory<Symbol>();
            return parseOperator.TryParse(processor, typeof (string), TypedValue.Void, TestUtils.CreateCell(input), ref result);
        }

        public static void AssertCellFails(Parse cell)
        {
            Assert.IsTrue(cell.Tag.IndexOf("fail") > -1);
        }

        public static void AssertValueInBody(Parse cell, string value)
        {
            Assert.IsTrue(cell.Body.IndexOf(value) > -1);
        }

        public static void AssertValuesInBody(Parse cell, string[] values)
        {
            foreach (string value in values)
            {
                AssertValueInBody(cell, value);
            }
        }

        public static void AssertCellPasses(Parse cell)
        {
            Assert.IsTrue(cell.Tag.IndexOf("pass") > -1);
        }

        public static void VerifyCounts(Fixture fixture, int right, int wrong, int ignores, int exceptions)
        {
            Assert.AreEqual(right, fixture.Counts.Right);
            Assert.AreEqual(wrong, fixture.Counts.Wrong);
            Assert.AreEqual(ignores, fixture.Counts.Ignores);
            Assert.AreEqual(exceptions, fixture.Counts.Exceptions);
        }
    }
}