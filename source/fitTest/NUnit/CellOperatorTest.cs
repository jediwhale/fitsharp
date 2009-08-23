// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Test.Acceptance;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {

    public class CellOperatorTest {
        protected Service.Service service;
        protected StringFixture stringFixture;
        protected IntFixture intFixture;
        protected PersonFixture personFixture;

        public static bool IsMatch(ParseOperator<Cell> parseOperator, string input) {
            TypedValue result = TypedValue.Void;
            var processor = new Processor<Cell>();
            processor.AddMemory<Symbol>();
            return parseOperator.TryParse(processor, typeof (string), TypedValue.Void, TestUtils.CreateCell(input), ref result);
        }

        public static bool IsMatch(CompareOperator<Cell> compareOperator, object instance, Type type, string value) {
            var processor = new Processor<Cell>();
            processor.AddOperator(new CompareDefault());
            return compareOperator.CanCompare(processor, new TypedValue(instance, type), TestUtils.CreateCell(value));
        }

        public static bool IsMatch(ExecuteOperator<Cell> executor, Tree<Cell> parameters) {
            var processor = new Processor<Cell>();
            processor.AddMemory<Symbol>();
            processor.AddOperator(new ParseMemberName());
            return executor.CanExecute(processor, new TypedValue(new ExecuteContext(new TestStatus(), null, new TypedValue("stuff"))), parameters);
        }

        public void MakeStringFixture() {
            service = new Service.Service();
            stringFixture = new StringFixture { Processor = service };
        }

        public void MakePersonFixture() {
            service = new Service.Service();
            personFixture = new PersonFixture { Processor = service };
        }

        public void MakeIntFixture() {
            service = new Service.Service();
            intFixture = new IntFixture { Processor = service };
        }

        public static void AssertCellPasses(Parse cell)
        {
            Assert.AreEqual(CellAttributes.RightStatus, cell.GetAttribute(CellAttributes.StatusKey));
        }

        public static void AssertCellFails(Parse cell)
        {
            Assert.AreEqual(CellAttributes.WrongStatus, cell.GetAttribute(CellAttributes.StatusKey));
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

    }
}
