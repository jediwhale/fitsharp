// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fitSharp.Fit.Model;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseMemberName: CellOperator, ParseOperator<Cell> {
        private static readonly Dictionary<char, string> specialCharacterConversion;

        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return type == typeof(MemberName);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            StringBuilder nameParts = Processor.Get<Settings>().BehaviorHas("fitlibrary1")
                ? parameters.Leaves().Aggregate(new StringBuilder(), (t, cell) => AppendWithConversion(t, cell.Text))
                : parameters.Leaves().Aggregate(new StringBuilder(), (t, cell) => Append(t, cell.Text));

            if (nameParts.Length == 0) nameParts.Append("blank");
            string name = nameParts.ToString();
            return new TypedValue(MemberName.Parse(name));
        }

        private static StringBuilder Append(StringBuilder nameParts, string name) {
            return nameParts.Append(name);
        }

        private static StringBuilder AppendWithConversion(StringBuilder nameParts, IEnumerable<char> name) {
            return nameParts.Append(
                             name.Aggregate(new StringBuilder(), 
                                 (t, character) => !specialCharacterConversion.ContainsKey(character)
                                                                          ? t.Append(character)
                                                                          : t.Append(specialCharacterConversion[character])));
        }

        static ParseMemberName() {
            specialCharacterConversion = new Dictionary<char, string> {
                {'!', "bang"},
                {'"', "quote"},
                {'#', "hash"},
                {'$', "dollar"},
                {'%', "percent"},
                {'&', "ampersand"},
                {'\'', "single quote"},
                {'(', "left parenthesis"},
                {')', "right parenthesis"},
                {'*', "star"},
                {'+', "plus"},
                {',', "comma"},
                {'-', "minus"},
                {'.', "dot"},
                {'/', "slash"},
                {':', "colon"},
                {';', "semicolon"},
                {'<', "less than"},
                {'>', "greater than"},
                {'=', "equals"},
                {'?', "question"},
                {'@', "at"},
                {'[', "left square bracket"},
                {']', "right square bracket"},
                {'\\', "backslash"},
                {'^', "caret"},
                {'`', "backquote"},
                {'{', "left brace"},
                {'}', "right brace"},
                {'|', "bar"},
                {'~', "tilde"}
            };
        }
    }
}
