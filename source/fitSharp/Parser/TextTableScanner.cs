// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;

namespace fitSharp.Parser {
    public class TextTableScanner {
        readonly IEnumerator<Token> enumerator;
        readonly Characters source;

        public TextTableScanner(string input) {
            enumerator = Tokens.GetEnumerator();
            source = new Characters(input);
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
            source.MoveWhile(() => source.Type == CharacterType.WhiteSpace);
            return
                source.Type == CharacterType.End || source.Type == CharacterType.EndTest ? MakeEndOfTest() :
                source.Type == CharacterType.Newline ? MakeNewline() :
                source.Type == CharacterType.Quote ? MakeQuotedWord() :
                MakeDelimitedWord();
        }

        Token MakeEndOfTest() {
            source.Start();
            source.MoveWhile(() => source.Type != CharacterType.End && source.Type != CharacterType.BeginTest);
            if (source.Type == CharacterType.End) return new Token(TokenType.End, source.FromStart);
            source.MoveNext();
            return new Token(TokenType.Leader, source.FromStart);
        }

        Token MakeNewline() {
            source.MoveNext();
            return new Token(TokenType.Newline);
        }

        Token MakeQuotedWord() {
            string quote = source.Content;
            source.MoveNext();
            source.Start();
            source.MoveWhile(() => source.Content != quote);
            var result = new Token(TokenType.Word, source.FromStart);
            source.MoveNext();
            return result;
        }

        Token MakeDelimitedWord() {
            source.Start();
            source.MoveWhile(() => source.Type == CharacterType.Letter || source.Type == CharacterType.WhiteSpace);
            var result = new Token(TokenType.Word, source.FromStart.TrimEnd());
            if (source.Type == CharacterType.Separator) source.MoveNext();
            return result;
        }

        enum CharacterType {
            End,
            Newline,
            WhiteSpace,
            Quote,
            Separator,
            BeginTest,
            EndTest,
            Letter
        }

        class Characters {
            readonly string input;
            int next;
            int length;
            int start;

            public CharacterType Type { get; private set; }
            public string Content { get { return length == 0 ? string.Empty : input.Substring(next, length); } }
            public string FromStart { get { return next > start ? input.Substring(start, next - start) : string.Empty; } }

            public Characters(string input) {
                this.input = input;
                next = -1;
                length = 1;
                MoveNext();
            }

            public void Start() {
                start = next;
            }

            public void MoveNext() {
                next += length;
                if (next >= input.Length) {
                    length = 0;
                    Type = CharacterType.End;
                }
                else {
                    length = 1;
                    if (input[next] == '\n') Type = CharacterType.Newline;
                    else if (char.IsWhiteSpace(input[next])) Type = CharacterType.WhiteSpace;
                    else if (input[next] == '"' || input[next] == '\'') Type = CharacterType.Quote;
                    else if (input[next] == '|') Type = CharacterType.Separator;
                    else if (string.Compare("<br", 0, input, next, 3, StringComparison.OrdinalIgnoreCase) == 0
                        && input.IndexOf(">", next + 3, StringComparison.Ordinal) >= 0) {
                        Type = CharacterType.Newline;
                        length = input.IndexOf(">", next + 3, StringComparison.Ordinal) - next + 1;
                    }
                    else if (string.Compare("test@", 0, input, next, 5, StringComparison.OrdinalIgnoreCase) == 0) {
                        Type = CharacterType.BeginTest;
                        length = 5;
                    }
                    else if (string.Compare("@test", 0, input, next, 5, StringComparison.OrdinalIgnoreCase) == 0) {
                        Type = CharacterType.EndTest;
                        length = 5;
                    }
                    else Type = CharacterType.Letter;
                }
            }

            public void MoveWhile(Func<bool> condition) { while (condition()) MoveNext(); }
        }
    }
}
