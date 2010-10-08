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
        readonly Func<Characters, bool> isWordContent;
        public CharacterType StartOfLine { get; private set;}

        public TextTableScanner(string input, Func<Characters, bool> isWordContent) {
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
            source.MoveWhile(() => source.Type == CharacterType.WhiteSpace);
            return
                source.Type == CharacterType.End || source.Type == CharacterType.EndTest ? MakeEndOfTest() :
                source.Type == CharacterType.Newline ? MakeNewline() :
                source.Type == CharacterType.BeginCell ? MakeToken(TokenType.BeginCell) :
                source.Type == CharacterType.EndCell ? MakeToken(TokenType.EndCell) :
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
            StartOfLine = source.Type;
            if (source.Type == CharacterType.Separator) source.MoveNext();
            return new Token(TokenType.Newline);
        }

        Token MakeToken(TokenType tokenType) {
            source.MoveNext();
            return new Token(tokenType);
        }

        Token MakeQuotedWord() {
            string quote = source.Content;
            source.MoveNext();
            source.Start();
            source.MoveWhile(() => source.Content != quote && source.Type != CharacterType.End);
            var result = new Token(TokenType.Word, source.FromStart);
            source.MoveNext();
            return result;
        }

        Token MakeDelimitedWord() {
            source.Start();
            source.MoveWhile(() => isWordContent(source));
            var result = new Token(TokenType.Word, source.FromStart.TrimEnd());
            if (source.Type == CharacterType.Separator) source.MoveNext();
            return result;
        }
    }
}
