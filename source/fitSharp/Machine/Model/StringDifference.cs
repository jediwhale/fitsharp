// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;

namespace fitSharp.Machine.Model {
    public class StringDifference {
        public StringDifference(string expected, string actual) {
            this.expected = expected;
            this.actual = actual;
        }

        public override string ToString() {
            if (expected.Length != actual.Length) {
                return string.Format("Length expected {0} was {1}", expected.Length, actual.Length);
            }
            for (var i = 0; i < expected.Length; i++) {
                if (expected[i] != actual[i]) {
                    return string.Format("At {2} expected {0} was {1}", Display(expected[i]), Display(actual[i]), i);
                }
            }
            return string.Empty;
        }

        string Display(char character) {
            if (character > 0x20 && character < 0x7F) return new string(character, 1);
            return string.Format("x{0:x}", Convert.ToInt32(character));
        }

        readonly string expected;
        readonly string actual;
    }
}
