// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;

namespace fitSharp.Fit.Runner {
    public class TestPageDecoration {
	    
        public TestPageDecoration(string theSetUp, string theTearDown) {
            mySetUp = string.Empty;
            mySetUpHead = string.Empty;
            myTearDown = string.Empty;
            myTearDownHead = string.Empty;

            if (theSetUp.Length > 0) {
                var setUp = new HtmlString(theSetUp);
                mySetUp = setUp.Body;
                mySetUpHead = setUp.Head;
            }
            if (theTearDown.Length > 0) {
                var tearDown = new HtmlString(theTearDown);
                myTearDown = tearDown.Body;
                myTearDownHead = tearDown.Head;
            }
        }

        public TestPageDecoration MakeChild(string theSetUp, string theTearDown) {
            var child = new TestPageDecoration(theSetUp, theTearDown);
            child.mySetUpHead = mySetUpHead + child.mySetUpHead;
            child.mySetUp = mySetUp + child.mySetUp;
            child.myTearDownHead += myTearDownHead;
            child.myTearDown += myTearDown;
            return child;
        }
	    
        public bool IsEmpty {
            get {
                return
                    mySetUp.Length == 0 && myTearDown.Length == 0 && mySetUpHead.Length == 0 && myTearDownHead.Length == 0;
            }
        }
	    
        public string Decorate(string theInput) {
            var result = new StringBuilder();
            var input = new HtmlString(theInput);
            result.Append(input.Leader);
            result.Append(mySetUpHead);
            result.Append(input.Head);
            result.Append(myTearDownHead);
            result.Append(input.Middle);
            result.Append(mySetUp);
            result.Append(input.Body);
            result.Append(myTearDown);
            result.Append(input.Trailer);
            return result.ToString();
        }
	    
        private string mySetUp;
        private string myTearDown;
        private string mySetUpHead;
        private string myTearDownHead;
	    
        private class HtmlString {
	        
            public HtmlString(string theContent) {
                myContent = theContent;
                myHeadStart = myContent.IndexOf("<head", StringComparison.OrdinalIgnoreCase);
                if (myHeadStart >= 0) myHeadStart = myContent.IndexOf(">", myHeadStart, StringComparison.OrdinalIgnoreCase);
                if (myHeadStart < myContent.Length - 1) {
                    myHeadEnd = myContent.IndexOf("</head>", myHeadStart + 1, StringComparison.OrdinalIgnoreCase);
                    if (myHeadEnd < 0) {
                        myHeadStart = -1;
                        myHeadEnd = 0;
                    }
                }
                myBodyStart = myContent.IndexOf("<body", myHeadEnd + 1, StringComparison.OrdinalIgnoreCase);
                if (myBodyStart >= 0) myBodyStart = myContent.IndexOf(">", myBodyStart, StringComparison.OrdinalIgnoreCase);
                if (myBodyStart < 0 && myHeadEnd > 0) myBodyStart = myHeadEnd + 6;
                myBodyEnd = myContent.Length;
                if (myBodyStart < myBodyEnd - 1) {
                    myBodyEnd = myContent.IndexOf("</body>", myBodyStart + 1, StringComparison.OrdinalIgnoreCase);
                    if (myBodyEnd < 0) myBodyEnd = myContent.Length;
                }
            }
	        
            public string Leader {
                get {
                    return (myHeadStart >= 0? myContent.Substring(0, myHeadStart + 1): string.Empty);
                }
            }
	        
            public string Head {
                get {
                    return
                        (myHeadEnd > myHeadStart + 1
                             ? myContent.Substring(myHeadStart + 1, myHeadEnd - myHeadStart - 1)
                             : string.Empty);
                }
            }
	        
            public string Middle {
                get {
                    return
                        (myBodyStart > myHeadEnd - 1
                             ? myContent.Substring(myHeadEnd, myBodyStart - myHeadEnd + 1)
                             : string.Empty);
                }
            }
	        
            public string Body {
                get {
                    return
                        (myBodyEnd > myBodyStart + 1
                             ? myContent.Substring(myBodyStart + 1, myBodyEnd - myBodyStart - 1)
                             : string.Empty);
                }
            }
	        
            public string Trailer {
                get {
                    return (myBodyEnd < myContent.Length? myContent.Substring(myBodyEnd) : string.Empty);
                }
            }
	        
            private readonly string myContent;
            private readonly int myBodyStart;
            private readonly int myBodyEnd;
            private readonly int myHeadStart;
            private readonly int myHeadEnd;
        }
    }
}