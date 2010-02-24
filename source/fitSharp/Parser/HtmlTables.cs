// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;

namespace fitSharp.Parser {

    public interface ParseTreeNodeFactory<T> {
        T MakeNode(string text, string startTag, string endTag, string leader, string body, T firstChild);
        void AddTrailer(T node, string trailer);
        void AddSibling(T node, T sibling);
    }

    // Parses a HTML string, recognizing tables with embedded lists and tables.
    // Uses a recursive descent parsing approach.
    // The lexical analyzer is unusual - it skips everything until it finds the next expected token.
    public class HtmlTables<T> where T: class {
        readonly ParseTreeNodeFactory<T> factory;

        public HtmlTables(ParseTreeNodeFactory<T> factory) {
            this.factory = factory;
        }

        public T Parse(string input) {
            var alternationParser = new AlternationParser(factory);
            var cells = new ListParser(factory, "td", alternationParser, false);
            var rows = new ListParser(factory, "tr", cells, true);
            var tables = new ListParser(factory, "table", rows, true);
            var items = new ListParser(factory, "li", alternationParser, false);
            var lists = new ListParser(factory, "ul", items, true);
            alternationParser.ChildParsers = new [] {tables, lists};
            return tables.Parse(new LexicalAnalyzer(input));
        }

        interface ElementParser {
            T Parse(LexicalAnalyzer theAnalyzer);
            string Keyword {get;}
        }

        class ListParser: ElementParser {

            readonly ElementParser myChildParser;
            readonly string myKeyword;
            readonly bool IRequireChildren;
            readonly ParseTreeNodeFactory<T> factory;
            
            public ListParser(ParseTreeNodeFactory<T> factory, string theKeyword, ElementParser theChildParser, bool thisRequiresChildren) {
                this.factory = factory;
                myChildParser = theChildParser;
                myKeyword = theKeyword;
                IRequireChildren = thisRequiresChildren;
            }

            public string Keyword {get {return myKeyword;}}

            public T Parse(LexicalAnalyzer theAnalyzer) {
                T list = ParseOne(theAnalyzer);
                if (list != null) {
                    T next = Parse(theAnalyzer);
                    factory.AddSibling(list, next);
                    //list.Next = next;
                    if (next == null) /*list.Trailer = theAnalyzer.Trailer*/ factory.AddTrailer(list, theAnalyzer.Trailer);
                }
                return list;
            }

            public T ParseOne(LexicalAnalyzer theAnalyzer) {
                theAnalyzer.GoToNextToken(myKeyword);
                if (theAnalyzer.Token.Length == 0) return null;
                return ParseElement(theAnalyzer);
            }

            T ParseElement(LexicalAnalyzer theAnalyzer) {
                string tag = theAnalyzer.Token;
                string leader = theAnalyzer.Leader;
                theAnalyzer.PushEnd("/" + myKeyword);
                T children = myChildParser.Parse(theAnalyzer);
                if (IRequireChildren && children == null) {
                    throw new ApplicationException(string.Format("Can't find tag: {0}", myChildParser.Keyword));
                }
                theAnalyzer.PopEnd();
                theAnalyzer.GoToNextToken("/" + myKeyword);
                if (theAnalyzer.Token.Length == 0) throw new ApplicationException("expected /" + myKeyword + " tag");
                return factory.MakeNode(HtmlToText(theAnalyzer.Leader), tag, theAnalyzer.Token, leader, theAnalyzer.Leader, children);
            }
        }

	    static string HtmlToText(string theHtml) {
	        return new HtmlString(theHtml).ToPlainText();
        }

        class AlternationParser: ElementParser {
            readonly ParseTreeNodeFactory<T> factory;

            public AlternationParser(ParseTreeNodeFactory<T> factory) {
                this.factory = factory;
            }

            public T Parse(LexicalAnalyzer theAnalyzer) {
                T result = null;
                ListParser firstChildParser = null;
                int firstPosition = int.MaxValue;
                foreach (ListParser childParser in myChildParsers) {
                    int contentPosition = theAnalyzer.FindPosition(childParser.Keyword);
                    if (contentPosition >= 0 && contentPosition < firstPosition) {
                        firstPosition = contentPosition;
                        firstChildParser = childParser;
                    }
                }
                if (firstChildParser != null) {
                    result = firstChildParser.ParseOne(theAnalyzer);
                    //result.Next = Parse(theAnalyzer);
                    factory.AddSibling(result, Parse(theAnalyzer));
                }
                return result;
            }

            public string Keyword {get {return string.Empty;}}

            public ListParser[] ChildParsers {set {myChildParsers = value;}}

            ListParser[] myChildParsers;
        }

        class LexicalAnalyzer {

            readonly string myInput;
            int myPosition;
            readonly Stack<string> myEndTokens;

            public LexicalAnalyzer(string theInput) {
                myInput = theInput;
                myPosition = 0;
                myEndTokens = new Stack<string>();
            }

            public void GoToNextToken(string theToken) {
                Token = string.Empty;
                int start = myInput.IndexOf("<" + theToken, myPosition, StringComparison.OrdinalIgnoreCase);
                if (start < 0 || start > EndPosition) return;
                Leader = myInput.Substring(myPosition, start - myPosition);
                int end = myInput.IndexOf('>', start);
                if (end < 0) return;
                Token = myInput.Substring(start, end - start + 1);
                myPosition = end + 1;
            }

            public int FindPosition(string theToken) {
                int start = myInput.IndexOf("<" + theToken, myPosition, StringComparison.OrdinalIgnoreCase);
                if (start < 0 || start > EndPosition) return -1;
                int end = myInput.IndexOf('>', start);
                if (end < 0) return -1;
                return start;
            }

            public string Trailer {
                get {
                    int endPosition = EndPosition;
                    string result = myInput.Substring(myPosition, endPosition - myPosition);
                    myPosition = endPosition;
                    return result;
                }
            }

            public string PeekEnd() {
                string endToken = null;
                try {
                    endToken = myEndTokens.Peek();
                }
                catch (InvalidOperationException) {}
                return endToken;
            }

            public void PushEnd(string theToken) {
                myEndTokens.Push(theToken);
            }

            public void PopEnd() {
                myEndTokens.Pop();
            }

            public string Leader { get; private set; }
            public string Token { get; private set; }

            int EndPosition {
                get {
                    int endInput = -1;
                    string endToken = PeekEnd();
                    if (endToken != null) {
                        endInput = myInput.IndexOf("<" + endToken, myPosition, StringComparison.OrdinalIgnoreCase);
                    }
                    if (endInput < 0) endInput = myInput.Length;
                    return endInput;
                }
            }
        }
    }
}
