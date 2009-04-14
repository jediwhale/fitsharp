// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CompareNumeric : CompareOperator<Cell> {
        private static readonly Type[] numericTypes = {
            typeof(byte), typeof(sbyte), typeof(decimal), typeof(double),
            typeof(float), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(short), typeof(ushort)
        };

        private static readonly Comparison[] comparisons = {
            new Comparison(">=", 0, 1), new Comparison("<=", -1, 0), new Comparison("< ", -1, -1),
            new Comparison(">", 1, 1), new Comparison("<", -1, -1)
        };

        public bool TryCompare(Processor<Cell> processor, TypedValue instance, Tree<Cell> parameters, ref bool result) {
            if (parameters.Value.Text.StartsWith("<<")
                || FindComparison(parameters.Value.Text) == null
                || Array.IndexOf(numericTypes, instance.Type) < 0) return false;

            object actual = instance.Value;
            Cell cell = parameters.Value;
            Comparison comparison = FindComparison(cell.Text);

            var rest = new CellSubstring(cell, cell.Text.Substring(comparison.Operator.Length));
            object expected = processor.Parse(instance.Type, rest).Value;
            parameters.Value.AddToAttribute(CellAttributes.InformationPrefixKey, actual.ToString(), CellAttributes.PrefixFormat);

            int compare = actual is float || actual is double
                              ? (Convert.ToDouble(actual) < Convert.ToDouble(expected)
                                     ? -1
                                     : (Convert.ToDouble(actual) > Convert.ToDouble(expected) ? 1 : 0))
                              : decimal.Compare(Convert.ToDecimal(actual), Convert.ToDecimal(expected));
            result = comparison.MinCompare <= compare && compare <= comparison.MaxCompare;
            return true;
        }

        private static Comparison FindComparison(string theExpectedValue) {
            foreach (Comparison comparison in comparisons) {
                if (theExpectedValue.StartsWith(comparison.Operator)) return comparison;
            }
            return null;
        }

        private class Comparison {
            public Comparison(string theOperator, int theMinCompare, int theMaxCompare) {
                Operator = theOperator;
                MinCompare = theMinCompare;
                MaxCompare = theMaxCompare;
            }

            public readonly string Operator;
            public readonly int MinCompare;
            public readonly int MaxCompare;
        }
    }
}