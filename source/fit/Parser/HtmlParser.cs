// FitNesse.NET
// Copyright (c) 2007,2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Parser;

namespace fit {

    // Parses a HTML string, producing a Fit Parse tree.
    public class HtmlParser {

        public Parse Parse(string theInput) {
            return new HtmlTables<Parse>(new ParseNodeFactory()).Parse(theInput);
        }

        private class ParseNodeFactory: ParseTreeNodeFactory<Parse> {
            public Parse MakeNode(string startTag, string endTag, string leader, string body, Parse firstChild) {
                return new Parse(startTag, endTag, leader, body, firstChild);
            }

            public void AddTrailer(Parse node, string trailer) {
                node.Trailer = trailer;
            }

            public void AddSibling(Parse node, Parse sibling) {
                node.More = sibling;
            }
        }
    }
}
