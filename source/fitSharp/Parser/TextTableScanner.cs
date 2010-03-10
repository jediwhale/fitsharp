// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;

namespace fitSharp.Parser {
    public class TextTableScanner {
        readonly string input;
        readonly IEnumerator<Token> enumerator;
        int next;
        int length;

        public TextTableScanner(string input) {
            this.input = input;
            enumerator = Tokens.GetEnumerator();
        }

        public void MoveNext() { enumerator.MoveNext(); }
        public Token Current { get { return enumerator.Current; } }

        public IEnumerable<Token> Tokens {
            get {
                Token token = MakeEndOfTest();
                yield return token;
                while (token.Type != TokenType.End) {
                    while (true) {
                        token = MakeTestToken();
                        yield return token;
                        if ((token.Type == TokenType.End || token.Type == TokenType.Leader)) break;
                    }
                }
            }
        }

        Token MakeTestToken() {
            while (next < input.Length && IsWhiteSpace()) next++;
            return IsEndOfTest() ? MakeEndOfTest() :
                IsNewline() ? MakeNewline() :
                IsBreak() ? MakeNewlineFromBreak() :
                IsQuote() ? MakeQuotedWord() :
                MakeDelimitedWord(IsSeparator);
        }

        Token MakeEndOfTest() {
            int leader = input.IndexOf("test@", next, StringComparison.OrdinalIgnoreCase);
            if (leader < 0) return MakeEnd();
            var result = new Token(TokenType.Leader, input.Substring(next, leader + 5 - next));
            next = leader + 5;
            return result;
        }

        Token MakeEnd() {
            return next >= input.Length ? new Token(TokenType.End) : new Token(TokenType.End, input.Substring(next));
        }

        Token MakeNewline() {
            next++;
            return new Token(TokenType.Newline);
        }

        Token MakeNewlineFromBreak() {
            next = input.IndexOf(">", next, StringComparison.Ordinal);
            return MakeNewline();
        }

        Token MakeQuotedWord() {
            char quote = input[next++];
            int start = next;
            GetWordLength(() => input[next] == quote);
            return new Token(TokenType.Word, input.Substring(start, length));
        }

        Token MakeDelimitedWord(Machine.Extension.Func<bool> isDelimiter) {
            int start = next;
            GetWordLength(isDelimiter);
            while (length > 0 && char.IsWhiteSpace(input[start + length - 1])) length--;
            return new Token(TokenType.Word, input.Substring(start, length));
        }

        void GetWordLength(Machine.Extension.Func<bool> isDelimiter) {
            int start = next;
            while (next < input.Length && !IsEndOfWord()) next++;
            length = next - start;
            if (next < input.Length && isDelimiter()) {
                next++;
            }
        }

        bool IsNewline() { return input[next] == '\n'; }
        bool IsSeparator() { return input[next] == '|'; }
        bool IsQuote() { return input[next] == '"' || input[next] == '\''; }

        bool IsEndOfWord() {
            return IsNewline() || IsQuote() || IsSeparator() || IsBreak() || IsEndOfTest();
        }

        bool IsWhiteSpace() {
            return !IsNewline() && char.IsWhiteSpace(input[next]);
        }

        bool IsBreak() {
            return
                input.IndexOf("<br", next, StringComparison.OrdinalIgnoreCase) == next &&
                input.IndexOf(">", next + 3, StringComparison.Ordinal) >= 0;
        }

        bool IsEndOfTest() {
            return next >= input.Length || input.IndexOf("@test", next, StringComparison.OrdinalIgnoreCase) == next;
        }
    }
}
