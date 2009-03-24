// FitNesse.NET
// Copyright © 2008 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Text;
using fitSharp.Fit.Application;
using fitSharp.Fit.Model;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CompareString: CompareOperator<Cell> {
        private static readonly Options defaultOption = Options.Parse(",IgnoreWhitespace");

        public bool TryCompare(Processor<Cell> processor, TypedValue instance, Tree<Cell> parameters, ref bool result) {
            var options = (OptionsList)Context.Configuration.GetItem(GetType().FullName);
            if (!options.HasPrefix(parameters.Value.Text)) return false;
            if (instance.Type != typeof(string)) return false;

            object actual = instance.Value;
            if (actual == null) return false;
            result = options.IsMatch(actual.ToString(), parameters.Value.Text);
            return true;
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