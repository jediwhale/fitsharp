// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Text;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseMemberNameExtended: ParseOperator<Cell> {
        private static readonly Dictionary<char, string> specialCharacterConversion;
        private static readonly Dictionary<char, string> digitConversion;

        public bool TryParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
            if (type != typeof(MemberName)) return false;

            var nameParts = new StringBuilder();
            
            foreach (Cell namePart in parameters.Leaves) {
                Append(nameParts, namePart.Text);
            }
            if (nameParts.Length == 0) nameParts.Append("blank");
            string name = nameParts.ToString();
            if (char.IsDigit(name, 0)) {
                name = digitConversion[name[0]] + name.Substring(1);
            }

            result = new TypedValue(new MemberName(new GracefulName(name).IdentifierName.ToString()));

            return true;
        }

        private static void Append(StringBuilder nameParts, string namePart) {
            foreach (char character in namePart) {
                if (!specialCharacterConversion.ContainsKey(character)) {
                    nameParts.Append(character);
                }
                else {
                    nameParts.Append(specialCharacterConversion[character]);
                }
            }
        }

        static ParseMemberNameExtended() {
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