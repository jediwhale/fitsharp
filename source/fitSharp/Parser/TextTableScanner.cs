// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;

namespace fitSharp.Parser {
    public class TextTableScanner {
        readonly IEnumerator<Token> enumerator;
        readonly Characters source;
        readonly Func<CharacterType, bool> isWordContent;
        public CharacterType StartOfLine { get; private set;}

        public TextTableScanner(string input, Func<CharacterType, bool> isWordContent) {
            enumerator = Tokens.GetEnumerator();
            source = new Characters(input);
            this.isWordContent = isWordContent;
            StartOfLine = source.Type;
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
            source.Until(() => source.Type != CharacterType.WhiteSpace);
            return
                source.AtEnd || source.Type == CharacterType.EndTest ? MakeEndOfTest() :
                source.Type == CharacterType.Newline ? MakeNewline() :
                source.Type == CharacterType.BeginCell ? MakeToken(TokenType.BeginCell) :
                source.Type == CharacterType.EndCell ? MakeToken(TokenType.EndCell) :
                source.Type == CharacterType.Quote ? MakeQuotedWord() :
                MakeDelimitedWord();
        }

        Token MakeEndOfTest() {
            var content = source.Until(() => source.Type == CharacterType.BeginTest);
            if (source.AtEnd) return new Token(TokenType.End, content);
            content += source.Content;
            source.MoveNext();
            return new Token(TokenType.Leader, content);
        }

        Token MakeNewline() {
            source.MoveNext();
            StartOfLine = source.Type;
            if (source.Type == CharacterType.Separator) source.MoveNext();
            return new Token(TokenType.Newline);
        }

        Token MakeToken(TokenType tokenType) {
            source.MoveNext();
            return new Token(tokenType);
        }

        Token MakeQuotedWord() {
            var quote = source.Content;
            source.MoveNext();
            var result = new Token(TokenType.Word, source.Until(() => source.Type == CharacterType.Quote && source.Content == quote));
            source.MoveNext();
            return result;
        }

        Token MakeDelimitedWord() {
            var result = new Token(TokenType.Word, source.Until(() => !isWordContent(source.Type)).TrimEnd().Replace('_', ' '));
            if (source.Type == CharacterType.Separator) source.MoveNext();
            return result;
        }
    }
}
