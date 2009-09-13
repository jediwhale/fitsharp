// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text.RegularExpressions;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseType: CellOperator, ParseOperator<Cell> {
        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return type == typeof(Type) || type == typeof(RuntimeType);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            var runtimeType = Processor.ApplicationUnderTest.FindType(new TypeMatcher(parameters.Value.Text));
            return new TypedValue(type == typeof (RuntimeType) ? runtimeType : (object)runtimeType.Type, type);
        }

        private class TypeMatcher: NameMatcher {
            private static readonly Regex fullyQualifiedRegex = new Regex(@"^([A-Za-z_][A-Za-z\d_]+\.)+[A-Za-z_][A-Za-z\d_]+$");

            public string MatchName { get; private set; }

            public TypeMatcher(string matchName) {
                MatchName = matchName.Trim();
            }

            public bool Matches(string candidateName) {
                string baseName = fullyQualifiedRegex.IsMatch(candidateName)
                                      ? MatchName
                                      : new GracefulName(MatchName).IdentifierName.ToString();
                var typeIdentifier = new IdentifierName(baseName);
                var typeFixtureIdentifier = new IdentifierName(baseName + "fixture");
                return typeIdentifier.Matches(candidateName) || typeFixtureIdentifier.Matches(candidateName);
            }
        }
    }
}