// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;

namespace fitSharp.Machine.Model {
    public class MemberName {
        public const string NamedParameterPrefix = "!";

        public static MemberName Parse(string name) {
            if (name.EndsWith("?")) name = name.Substring(0, name.Length - 1);
            if (name.Length > 0 && char.IsDigit(name, 0)) {
                name = digitConversion[name[0]] + name.Substring(1);
            }
            return new MemberName(new GracefulName(name).IdentifierName.ToString());
        }

        public MemberName(string name) { this.name = name; }
        public override string ToString() { return name; }

        static readonly Dictionary<char, string> digitConversion = new Dictionary<char, string> {
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

        readonly string name;
    }
}
