// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Text;

namespace fitSharp.Parser {
    public class Characters {
        public const string TextStoryTestBegin = "test@";
        readonly string input;
        int next;
        int length;

        public bool AtEnd { get { return next >= input.Length; } }

        public CharacterType Type { get; private set; }
        public string Content { get { return length == 0 ? string.Empty : input.Substring(next, length); } }

        public Characters(string input) {
            this.input = input.Replace("\r", string.Empty);
            next = -1;
            length = 1;
            MoveNext();
        }

        public void MoveNext() {
            next += length;
            if (AtEnd) {
                length = 0;
                return;
            }
            length = 1;
            if (next < input.Length - 1 && input[next] == '\\') {
                next++;
                Type = CharacterType.Letter;
            }
            else if (input[next] == '[' && next + 1 < input.Length && input[next+1] == '\n') {
                Type = CharacterType.BeginCell;
                length = 2;
            }
            else if (input[next] == '\n' && next + 1 < input.Length && input[next+1] == ']') {
                Type = CharacterType.EndCell;
                length = 2;
            }
            else if (input[next] == '\n') Type = CharacterType.Newline;
            else if (char.IsWhiteSpace(input[next])) Type = CharacterType.WhiteSpace;
            else if (input[next] == '"' || input[next] == '\'') Type = CharacterType.Quote;
            else if (input[next] == '|') Type = CharacterType.Separator;
            else if (string.Compare("<br", 0, input, next, 3, StringComparison.OrdinalIgnoreCase) == 0
                && input.IndexOf(">", next + 3, StringComparison.Ordinal) >= 0) {
                Type = CharacterType.Newline;
                length = input.IndexOf(">", next + 3, StringComparison.Ordinal) - next + 1;
            }
            else if (string.Compare(TextStoryTestBegin, 0, input, next, 5, StringComparison.OrdinalIgnoreCase) == 0) {
                Type = CharacterType.BeginTest;
                length = 5;
            }
            else if (string.Compare("@test", 0, input, next, 5, StringComparison.OrdinalIgnoreCase) == 0) {
                Type = CharacterType.EndTest;
                length = 5;
            }
            else Type = CharacterType.Letter;
        }

        public string Until(Func<bool> condition) {
            var startPosition = next;
            while (!AtEnd && !condition()) MoveNext();
            return FromPosition(startPosition);
        }

        string FromPosition(int startPosition) {
            if (next <= startPosition) return string.Empty;
            var result = new StringBuilder();
            for (var current = startPosition; current < next; current++) {
                if (current < next - 1 && input[current] == '\\') current++;
                result.Append(input[current]);
            }
            return result.ToString();
        }
    }
}
