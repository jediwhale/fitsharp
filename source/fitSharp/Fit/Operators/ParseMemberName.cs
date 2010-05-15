// Copyright © 2009 Syterra Software Inc. All rights reserved.
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
        private static readonly Dictionary<char, string> digitConversion;

        public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
            return type == typeof(MemberName);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
            StringBuilder nameParts = Processor.Configuration.GetItem<Settings>().BehaviorHas("fitlibrary1")
                ? parameters.Leaves().Aggregate(new StringBuilder(), (t, cell) => AppendWithConversion(t, cell.Text))
                : parameters.Leaves().Aggregate(new StringBuilder(), (t, cell) => Append(t, cell.Text));

            if (nameParts.Length == 0) nameParts.Append("blank");
            string name = nameParts.ToString();
            if (char.IsDigit(name, 0)) {
                name = digitConversion[name[0]] + name.Substring(1);
            }
            return new TypedValue(new MemberName(new GracefulName(name).IdentifierName.ToString()));
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

            digitConversion = new Dictionary<char, string> {
                {'0', "zero"},
                {'1', "one"},
                {'2', "two"},
                {'3', "three"},
                {'4', "four"},
                {'5', "five"},
                {'6', "six"},
                {'7', "seven"},
                {'8', "eight"},
                {'9', "nine"}
            };
        }
    }
}