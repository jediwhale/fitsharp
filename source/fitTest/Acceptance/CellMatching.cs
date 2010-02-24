// Copyright © 2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Web;
using fitlibrary;
using fitSharp.Machine.Model;

namespace fit.Test.Acceptance {
    public class CellMatching : CalculateFixture {

        public override void DoTable(Parse table) {
            Save("four", 4);
            base.DoTable(table);
        }

        public string MatchesActualTypeActualValueExpectedValue(
            string theActualType, string theActualValue, string theExpectedValue) {
            string expectedValue = theExpectedValue.StartsWith("'")
                                       ? theExpectedValue.Substring(1, theExpectedValue.Length - 2)
                                       : theExpectedValue;
            try {
                string html = "<table><tr><td>" + HttpUtility.HtmlEncode(expectedValue) + "</td></tr></table>";
                myCell = new Parse(html).Parts.Parts;
                return
                    CellOperation.Compare(MakeTypedValue(theActualValue, theActualType), myCell).ToString();
            }
            catch (Exception) {
                return "Exception";
            }
        }

        static TypedValue MakeTypedValue(string value, string type) {
            switch (type.ToLower()) {
                case "string": return new TypedValue(value, typeof(string));
                case "int": return new TypedValue(int.Parse(value), typeof(int));
                case "uint": return new TypedValue(uint.Parse(value), typeof(uint));
                case "long": return new TypedValue(long.Parse(value), typeof(long));
                case "ulong": return new TypedValue(ulong.Parse(value), typeof(ulong));
                case "short": return new TypedValue(short.Parse(value), typeof(short));
                case "ushort": return new TypedValue(ushort.Parse(value), typeof(ushort));
                case "byte": return new TypedValue(byte.Parse(value), typeof(byte));
                case "sbyte": return new TypedValue(sbyte.Parse(value), typeof(sbyte));
                case "decimal": return new TypedValue(decimal.Parse(value), typeof(decimal));
                case "double": return new TypedValue(double.Parse(value), typeof(double));
                case "float": return new TypedValue(float.Parse(value), typeof(float));
            }
            return TypedValue.Void;
        }

        public string ExpectedCellActualTypeActualValueExpectedValue(
            string theActualType, string theActualValue, string theExpectedValue) {
            return myCell.Body;
        }

        public string MatchesActualTypeActualValueExpectedValueCellHandlers(
            string theActualType, string theActualValue, string theExpectedValue, string theCellHandlers) {
            return MatchesActualTypeActualValueExpectedValue(theActualType, theActualValue, theExpectedValue);
        }

        Parse myCell;
    }
}