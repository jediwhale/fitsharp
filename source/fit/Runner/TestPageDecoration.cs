// FitNesse.NET
// Copyright (c) 2006, 2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Text;

namespace fit.Runner {
    public class TestPageDecoration {
	    
        public TestPageDecoration(string theSetUp, string theTearDown) {
            mySetUp = string.Empty;
            mySetUpHead = string.Empty;
            myTearDown = string.Empty;
            myTearDownHead = string.Empty;

            if (theSetUp.Length > 0) {
                HtmlString setUp = new HtmlString(theSetUp);
                mySetUp = setUp.Body;
                mySetUpHead = setUp.Head;
            }
            if (theTearDown.Length > 0) {
                HtmlString tearDown = new HtmlString(theTearDown);
                myTearDown = tearDown.Body;
                myTearDownHead = tearDown.Head;
            }
        }

        public TestPageDecoration MakeChild(string theSetUp, string theTearDown) {
            TestPageDecoration child = new TestPageDecoration(theSetUp, theTearDown);
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
            StringBuilder result = new StringBuilder();
            HtmlString input = new HtmlString(theInput);
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
	        
            private string myContent;
            private int myBodyStart;
            private int myBodyEnd;
            private int myHeadStart;
            private int myHeadEnd;
        }
    }
}