// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Model;

namespace fitSharp.Parser {

    // Parses a HTML string, recognizing tables with embedded lists and tables.
    // Uses a recursive descent parsing approach.
    // The lexical analyzer is unusual - it skips everything until it finds the next expected token.
    public class HtmlTables {

        public Tree<CellBase> Parse(string input) {
            var alternationParser = new AlternationParser();
            var cells = new ListParser("td", alternationParser, false);
            var rows = new ListParser("tr", cells, true);
            var tables = new ListParser("table", rows, true);
            var items = new ListParser("li", alternationParser, false);
            var lists = new ListParser("ul", items, true);
            alternationParser.ChildParsers = new [] {tables, lists};
            var result = new TreeList<CellBase>(new CellBase("root"));
            foreach (Tree<CellBase> branch in tables.Parse(new LexicalAnalyzer(input))) result.AddBranch(branch);
            return result;
        }

        interface ElementParser {
            List<Tree<CellBase>> Parse(LexicalAnalyzer theAnalyzer);
            string Keyword {get;}
        }

        class ListParser: ElementParser {

            readonly ElementParser myChildParser;
            readonly string myKeyword;
            readonly bool IRequireChildren;
            
            public ListParser(string theKeyword, ElementParser theChildParser, bool thisRequiresChildren) {
                myChildParser = theChildParser;
                myKeyword = theKeyword;
                IRequireChildren = thisRequiresChildren;
            }

            public string Keyword {get {return myKeyword;}}

            public List<Tree<CellBase>> Parse(LexicalAnalyzer theAnalyzer) {
                var list = new List<Tree<CellBase>>();
                Tree<CellBase> first = ParseOne(theAnalyzer);
                if (first != null) {
                    list.Add(first);
                    List<Tree<CellBase>> rest = Parse(theAnalyzer);
                    list.AddRange(rest);
                    if (rest.Count == 0) first.Value.SetAttribute(CellAttribute.Trailer, theAnalyzer.Trailer);
                }
                return list;
            }

            public Tree<CellBase> ParseOne(LexicalAnalyzer theAnalyzer) {
                theAnalyzer.GoToNextToken(myKeyword);
                if (theAnalyzer.Token.Length == 0) return null;
                return ParseElement(theAnalyzer);
            }

            Tree<CellBase> ParseElement(LexicalAnalyzer theAnalyzer) {
                string tag = theAnalyzer.Token;
                string leader = theAnalyzer.Leader;
                theAnalyzer.PushEnd("/" + myKeyword);
                List<Tree<CellBase>> children = myChildParser.Parse(theAnalyzer);
                if (IRequireChildren && children.Count == 0) {
                    throw new ApplicationException(string.Format("Can't find tag: {0}", myChildParser.Keyword));
                }
                theAnalyzer.PopEnd();
                theAnalyzer.GoToNextToken("/" + myKeyword);
                if (theAnalyzer.Token.Length == 0) throw new ApplicationException("expected /" + myKeyword + " tag");
                var result = new TreeList<CellBase>(new CellBase(HtmlToText(theAnalyzer.Leader)));
                result.Value.SetAttribute(CellAttribute.Body, theAnalyzer.Leader);
                result.Value.SetAttribute(CellAttribute.EndTag, theAnalyzer.Token);
                result.Value.SetAttribute(CellAttribute.Leader, leader);
                result.Value.SetAttribute(CellAttribute.StartTag, tag);
                foreach (Tree<CellBase> child in children) result.AddBranch(child);
                return result;
            }
        }

	    static string HtmlToText(string theHtml) {
	        return new HtmlString(theHtml).ToPlainText();
        }

        class AlternationParser: ElementParser {
            public List<Tree<CellBase>> Parse(LexicalAnalyzer theAnalyzer) {
                var result = new List<Tree<CellBase>>();
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
                    result.Add(firstChildParser.ParseOne(theAnalyzer));
                    result.AddRange(Parse(theAnalyzer));
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
