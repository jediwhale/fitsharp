// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;
using fitSharp.Fit.Application;
using fitSharp.Fit.Model;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CompareString: CellOperator, CompareOperator<Cell> {
        private static readonly Options defaultOption = Options.Parse(",IgnoreWhitespace");

        public bool CanCompare(TypedValue actual, Tree<Cell> expected) {
            var options = (OptionsList)Context.Configuration.GetItem(GetType().FullName);
            if (!options.HasPrefix(expected.Value.Text)) return false;
            if (actual.Type != typeof(string)) return false;

            object actualValue = actual.Value;
            return actualValue != null;
        }

        public bool Compare(TypedValue actual, Tree<Cell> expected) {
            var options = (OptionsList)Context.Configuration.GetItem(GetType().FullName);
            return options.IsMatch(actual.ValueString, expected.Value.Text);
        }

        public CompareString() {
            Context.Configuration.SetItem(GetType(), new OptionsList());
        }

        private class OptionsList: ConfigurationList<Options> {
            public bool HasPrefix(string cellValue) {
                if (myList.Count == 0) return true;
                foreach (Options option in myList) {
                    if (option.MatchesPrefix(cellValue)) return true;
                }
                return false;
            }

            public bool IsMatch(string actual, string expected) {
                if (myList.Count == 0) return defaultOption.MatchesValue(actual, expected);
                foreach (Options option in myList) {
                    if (option.MatchesPrefix(expected)) {
                        return option.MatchesValue(actual, expected);
                    }
                }
                return false;
            }

            public override Options Parse(string theValue) { return Options.Parse(theValue); }

            public override ConfigurationList<Options> Make() { return new OptionsList(); }

        }

        private class Options {
            [Flags] private enum CompareOptions {
                None,
                IgnoreWhitespace,
                IgnoreCase
            }

            public static Options Parse(string input) {
                var result = new Options();
                string[] parts = input.Split(',');
                if (parts.Length > 0) result.prefix = new IdentifierName(parts[0]);
                if (parts.Length == 1) result.compareOptions = CompareOptions.IgnoreWhitespace;
                for (int i = 1; i < parts.Length; i++) {
                    result.compareOptions |= (CompareOptions) Enum.Parse(typeof (CompareOptions), parts[i]);
                }
                return result;
            }

            public bool MatchesPrefix(string value) {
                return prefix.IsStartOf(value);
            }

            public bool MatchesValue(string actual, string expected) {
                string expectedString = expected.Substring(prefix.Length);
                string actualString = actual;
                if ((compareOptions & CompareOptions.IgnoreWhitespace) == CompareOptions.IgnoreWhitespace) {
                    expectedString = StripWhitespace(expectedString);
                    actualString = StripWhitespace(actual);
                }
                return string.Compare(
                           expectedString,
                           actualString,
                           (compareOptions & CompareOptions.IgnoreCase) == CompareOptions.IgnoreCase)
                       == 0;
            }

            private static string StripWhitespace(object input) {
                if (input== null) return null;
                var result = new StringBuilder();
                foreach (char character in input.ToString()) {
                    if (char.IsWhiteSpace(character)) continue;
                    result.Append(character);
                }
                return result.ToString();
            }

            private IdentifierName prefix;
            private CompareOptions compareOptions;
        }
    }
}