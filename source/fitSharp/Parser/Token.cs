// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

namespace fitSharp.Parser {
    public enum TokenType {
        End,
        Leader,
        Newline,
        Word,
        BeginCell,
        EndCell
    }

    public class Token {
        public TokenType Type { get; private set; }
        public string Content { get; private set; }

        public Token(TokenType type, string content) {
            Type = type;
            Content = content;
        }

        public Token(TokenType type): this(type, string.Empty) {}
    }
}
