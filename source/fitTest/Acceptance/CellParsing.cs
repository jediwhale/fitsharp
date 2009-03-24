// FitNesse.NET
// Copyright © 2007,2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Text;
using fit.Engine;
using fitlibrary;
using fitlibrary.table;
using fitlibrary.tree;
using fitSharp.Fit.Model;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Test.Acceptance {
    public class CellParsing: SequenceFixture {

        public string ParseWithCustomParser(string theSource) {
            Context.Configuration.GetItem<Service>().AddOperator(new SampleCustomParser());
            CellParsingTestClass testClass = (CellParsingTestClass)
                                             InputValue(new Parse("td", theSource, null, null), typeof(CellParsingTestClass));
            string result =  QuotedString(testClass.ToString());
            return result;
        }

        public string ParseAsTypeCode(string theSource) {
            TypeCode result =
                (TypeCode)
                InputValue(new Parse("td", theSource, null, null), typeof (TypeCode));
            return QuotedString(result.ToString());
        }

        public string ParseAsNullableInt(string theSource) {
            Nullable<int> result =
                (Nullable<int>)InputValue(new Parse("td", theSource ?? "null", null, null), typeof (Nullable<int>));
            return QuotedString(result.ToString());
        }

        public bool CheckParseString(Parse theCells) {
            string result = (string)InputValue(theCells.More, typeof(string));
            ExpectedValueCell expected = new ExpectedValueCell(theCells.Last);
            return IsEqual(expected, string.Format("'{0}'", result));
        }

        private bool IsEqual(ExpectedValueCell cell, object value) {
            return cell.IsEqual(new TypedValue(value));
        }

        public bool CheckParseInteger(Parse theCells) {
            int result = (int)InputValue(theCells.More, typeof(int));
            ExpectedValueCell expected = new ExpectedValueCell(theCells.Last);
            return IsEqual(expected, result.ToString("###,###,###"));
        }

        public bool CheckParseTestClass(Parse theCells) {
            CellParsingTestClass result = (CellParsingTestClass)InputValue(theCells.More, typeof(CellParsingTestClass));
            ExpectedValueCell expected = new ExpectedValueCell(theCells.Last);
            return IsEqual(expected, result.Content);
        }

        public bool CheckParseTree(Parse theCells) {
            Tree result = (Tree)InputValue(theCells.More, typeof(Tree));
            ExpectedValueCell expected = new ExpectedValueCell(theCells.Last);
            return IsEqual(expected, TreeString(result));
        }

        private object InputValue(Parse cell, Type theType) {
            return Context.Configuration.GetItem<Service>().Parse(theType, new TypedValue(this), cell).Value;
        }

        private static string QuotedString(string theSourceString) {
            return string.Format("'{0}'", theSourceString);
        }

        private static string TreeString(Tree theTree) {
            StringBuilder result = new StringBuilder(theTree.Title);
            result.Append("[");
            foreach (Tree child in theTree.GetChildren()) {
                result.AppendFormat("{0} ", TreeString(child));
            }
            result.Append("]");
            return result.ToString();
        }

        public bool CheckParseTable(Parse theCells) {
            Table result = (Table)InputValue(theCells.More, typeof(Table));
            ExpectedValueCell expected = new ExpectedValueCell(theCells.Last);
            return IsEqual(expected, TableString(result));
        }

        private static string TableString(Table theTable) {
            StringBuilder result = new StringBuilder();
            result.Append("{");
            for (int row = 0; row < theTable.Rows(); row++) {
                result.Append("[");
                for (int cell = 0; cell < theTable.Cells(row); cell++) {
                    result.AppendFormat("{0} ", theTable.StringAt(row, cell));
                }
                result.Append("]");
            }
            result.Append("}");
            return result.ToString();
        }

        public bool CheckParseList(Parse theCells) {
            IList result = (IList)InputValue(theCells.More, typeof(IList));
            ExpectedValueCell expected = new ExpectedValueCell(theCells.Last);
            return IsEqual(expected, result);
        }

        public object AB(int theX, int theY) {
            return new Point(theX, theY);
        }
    }

    public class CellParsingTestClass {
        public string Content;
        public CellParsingTestClass(string theContent) {Content = theContent;}
        public static CellParsingTestClass Parse(string theInput) {
            return new CellParsingTestClass(theInput.Trim().ToUpper());
        }
        public override string ToString() {return Content;}
    }

    public class SampleCustomParser: ParseOperator<Cell> {
        public bool TryParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
            if (type != typeof(CellParsingTestClass) || parameters.Value.Text != "one") return false;
            result =  new TypedValue(new CellParsingTestClass(parameters.Value.Text == "one" ? "1" : "0"));
            return true;
        }
    }
}