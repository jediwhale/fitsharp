// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;

namespace fitSharp.Parser {
    public class Characters {
        public const string TextStoryTestBegin = "test@";
        Substring current;

        public bool AtEnd { get { return current.AtEnd; } }

        public CharacterType Type { get; private set; }
        public string Content { get { return current.ToString(); } }

        public Characters(string input) {
            current = new Substring(input.Replace("\r", string.Empty), 0, 0);
            MoveNext();
        }

        public void MoveNext() {
            current = current.After;
            if (current.AtEnd) {
                return;
            }
            if (current.StartsWith("\\")) {
                Type = CharacterType.Letter;
                current = current.Skip(1).Truncate(1);
            }
            else if (current.StartsWith("[\n")) {
                Type = CharacterType.BeginCell;
                current = current.Truncate(2);
            }
            else if (current.StartsWith("\n]")) {
                Type = CharacterType.EndCell;
                current = current.Truncate(2);
            }
            else if (current.StartsWith("\n")) {
                Type = CharacterType.Newline;
                current = current.Truncate(1);
            }
            else if (current.StartsWith("\"") || current.StartsWith("'")) {
                Type = CharacterType.Quote;
                current = current.Truncate(1);
            }
            else if (current.StartsWith("|")) {
                Type = CharacterType.Separator;
                current = current.Truncate(1);
            }
            else if (current.StartsWith(TextStoryTestBegin, StringComparison.OrdinalIgnoreCase)) {
                Type = CharacterType.BeginTest;
                current = current.Truncate(5);
            }
            else if (current.StartsWith("@test", StringComparison.OrdinalIgnoreCase)) {
                Type = CharacterType.EndTest;
                current = current.Truncate(5);
            }
            else if (char.IsWhiteSpace(current[0])) {
                Type = CharacterType.WhiteSpace;
                current = current.Truncate(1);
            }
            else if (current.StartsWith("<br", StringComparison.OrdinalIgnoreCase)
                && current.Contains(">")) {
                Type = CharacterType.Newline;
                current = current.TruncateAfter(">");
            }
            else {
                Type = CharacterType.Letter;
                current = current.Truncate(1);
            }
        }

        public string Until(Func<bool> condition) {
            var start = current;
            while (!AtEnd && !condition()) MoveNext();
            return FromPosition(start);
        }

        string FromPosition(Substring start) {
            var from = start.Truncate(current).ToString();
            var result = new StringBuilder();
            for (var index = 0; index < from.Length; index++) {
                if (index < from.Length - 1 && from[index] == '\\') index++;
                result.Append(from[index]);
            }
            return result.ToString();
        }
    }
}
