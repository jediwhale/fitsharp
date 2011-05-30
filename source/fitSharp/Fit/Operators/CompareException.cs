// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text.RegularExpressions;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CompareException: CellOperator, CompareOperator<Cell> {
        public bool CanCompare(TypedValue actual, Tree<Cell> expected) {
            return exceptionIdentifier.IsStartOf(expected.Value.Text) && expected.Value.Text.EndsWith("]");
        }

        public bool Compare(TypedValue actual, Tree<Cell> expected) {
            if (actual.IsValid) return false;

            var actualException = actual.GetValue<System.Exception>();
            var exceptionContent = expected.Value.Text.Substring("exception[".Length, expected.Value.Text.Length - ("exception[".Length + 1));

            if (IsMessageOnly(exceptionContent)) {
                return actualException.Message == exceptionContent.Substring(1, exceptionContent.Length - 2);
            }
            if (IsExceptionTypeNameOnly(exceptionContent)) {
                var actualContent = actualException.GetType().Name + ": \"" + actualException.Message + "\"";
                return exceptionContent == actualContent;
            }
            return actualException.GetType().Name == exceptionContent;
        }

        static readonly IdentifierName exceptionIdentifier = new IdentifierName("exception[");
        static readonly Regex regexForMessageOnly = new Regex("^\".*\"$");
        static readonly Regex regexForExceptionTypeNameOnly = new Regex("^.*: \".*\"$");

        static bool IsExceptionTypeNameOnly(string exceptionContent) {
            return regexForExceptionTypeNameOnly.IsMatch(exceptionContent);
        }

        static bool IsMessageOnly(string exceptionContent) {
            return regexForMessageOnly.IsMatch(exceptionContent);
        }
    }
}