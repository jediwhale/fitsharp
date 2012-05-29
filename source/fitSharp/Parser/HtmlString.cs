// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Linq;
using System.Text;

namespace fitSharp.Parser {
    public class HtmlString {
	    public static bool IsStandard;

        public HtmlString(string theHtml) {
            myHtml = theHtml;
        }
        
        public string ToPlainText() {
            string result = myHtml != null ? UnEscape(UnFormat(myHtml)): string.Empty;
            if (result.Any(c => c != ' ')) {
                return result;
            }
            return string.Empty;
        }
        
        static string UnFormat(string theInput) {
            var result = new TextOutput(IsStandard);
            var scan = new Scanner(theInput);
            while (true) {
                scan.FindTokenPair("<", ">", ourValidTagFilter);
                result.Append(scan.Leader);
                if (scan.Body.IsEmpty) break;
                if (IsStandard) result.AppendTag(GetTag(scan.Body));
            }
            return result.ToString();
        }
	    
        static bool IsValidTag(Substring theBody) {
            return theBody[0] == '/' || char.IsLetter(theBody[0]);
        }
	    
        static readonly TokenBodyFilter ourValidTagFilter = IsValidTag;
	    
        static string GetTag(Substring theInput) {
            var current = theInput;
            var tag = new StringBuilder();
            if (theInput[0] == '/') {
                tag.Append('/');
                current = current.Skip(1);
            }
            while (!current.IsEmpty && char.IsLetter(current[0])) {
                tag.Append(current[0]);
                current = current.Skip(1);
            }
            return tag.ToString().ToLower();
        }
	    
        static string UnEscape(string theInput) {
            var scan = new Scanner(theInput);
            var result = new StringBuilder();
            while (true) {
                scan.FindTokenPair("&", ";");
                result.Append(scan.Leader);
                if (scan.Body.IsEmpty) break;
                if (scan.Body.Equals("lt")) result.Append('<');
                else if (scan.Body.Equals("gt")) result.Append('>');
                else if (scan.Body.Equals("amp")) result.Append('&');
                else if (scan.Body.Equals("nbsp")) result.Append(' ');
                else if (scan.Body.Equals("quot")) result.Append('"');
                else {
                    result.Append('&');
                    result.Append(scan.Body);
                    result.Append(';');
                }
            }
            return result.ToString();
        }

        readonly string myHtml;
	    
        class TextOutput {

            readonly bool isStandard;
	        
            public TextOutput(bool isStandard) {
                this.isStandard = isStandard;
                myText = new StringBuilder();
                myLastTag = string.Empty;
                myWhitespace = false;
            }
	        
            public void Append(Substring theInput) {
                var current = theInput;
                while (!current.IsEmpty) {
                    var input = current[0];
                    if (isStandard && input != '\u00a0' && char.IsWhiteSpace(input)) {
                        if (!myWhitespace) {
                            myText.Append(' ');
                            myLastTag = myLastTag + " ";
                        }
                        myWhitespace = true;
                    }
                    else {
                        switch (input) {
                            case '\u201c':
                                input = '"'; break;
                            case '\u201d':
                                input = '"'; break;
                            case '\u2018':
                                input = '\''; break;
                            case '\u2019':
                                input = '\''; break;
                            case '\u00a0':
                                input = ' '; break;
                            case '&':
                                if (current.ContainsAt(1, "nbsp;")) {
                                    input = ' ';
                                    current = current.Skip(5);
                                }
                                break;
                        }
                        myText.Append(input);
                        myWhitespace = false;
                        myLastTag = string.Empty;
                    }
                    current = current.Skip(1);
                }
            }
	        
            public void AppendTag(string theInput) {
                if (theInput == "br") {
                    myText.Append("<br />");
                    myWhitespace = false;
                }
                else if (myLastTag.StartsWith("/p") && theInput == "p") {
                    if (myLastTag == "/p ") myText.Remove(myText.Length - 1, 1);
                    myWhitespace = false;
                    myText.Append("<br />");
                }
                myLastTag = theInput;
            }
	        
            public override string ToString() {
                return isStandard ? myText.ToString().Trim().Replace("<br>", "\n").Replace("<br />", "\n") : myText.ToString();
            }
	        
            readonly StringBuilder myText;
            string myLastTag;
            bool myWhitespace;
        }
    }
}