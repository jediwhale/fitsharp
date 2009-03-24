// FitLibrary for FitNesse .NET.
// Copyright (c) 2006 Syterra Software Inc. Released under the terms of the GNU General Public License version 2 or later.
// Based on designs from Fit (c) 2002 Cunningham & Cunningham, Inc., FitNesse by Object Mentor Inc., FitLibrary (c) 2003-2006 Rick Mugridge, University of Auckland, New Zealand.

using System;
using System.Collections;
using System.Xml;
using fit;
using fitSharp.Machine.Model;

namespace fitlibrary {

	public class XmlFixture: UnnamedCollectionFixtureBase {

        public XmlFixture(XmlDocument theDocument): base(new XmlEnumerator(theDocument)) {}

        protected override ListMatchStrategy MatchStrategy {
            get {return new XmlMatchStrategy();}
        }

        private class XmlMatchStrategy: ListMatchStrategy {
            public bool IsOrdered {get { return true; }}
            public bool SurplusAllowed {get {return false;}}
            public TypedValue[] ActualValues(object theActualRow) {
                var actuals = (object[]) theActualRow;
                var result = new TypedValue[actuals.Length];
                for (int i = 0; i < actuals.Length; i++) result[i] = new TypedValue(actuals[i], actuals[i] == null ? typeof(void) : typeof(string));
                return result;
            }
            public bool IsExpectedSize(Parse theExpectedCells, object theActualRow) {
                return (theExpectedCells.Size == ((object[])theActualRow).Length);
            }
            public bool FinalCheck(Fixture fixture) {return true;}
        }

        private class XmlEnumerator: IEnumerator {

            public XmlEnumerator(XmlDocument theDocument) {
                myDocument = theDocument;
                Reset();
            }

            public void Reset() {
                myElements = null;
            }

            public object Current {
                get {
                    ArrayList result = new ArrayList();
                    for (int i = 0; i < depth - 1; i++) result.Add(null);
                    result.Add(CurrentElement.Name);
                    string text = String.Empty;
                    foreach (XmlNode child in CurrentElement.ChildNodes) {
                        if (child is XmlText) text += ((XmlText)child).Value;
                    }
                    if (text.Length > 0) result.Add(text);
                    foreach (XmlAttribute attribute in CurrentElement.Attributes) {
                        result.Add(attribute.Name);
                        result.Add(attribute.Value);
                    }
                    return result.ToArray();
                }
            }

            public bool MoveNext() {
                if (myElements == null) {
                    myElements = new Stack();
                    myElements.Push(myDocument.DocumentElement);
                    depth = 1;
                    return true;
                }
                XmlNode next = CurrentElement.FirstChild;
                while (next == null || !(next is XmlElement)) {
                    if (next == null) {
                        try {
                            next = (XmlElement)myElements.Pop();
                            depth--;
                        }
                        catch (InvalidOperationException) {
                            return false;
                        }
                    }
                    next = next.NextSibling;
                }
                myElements.Push(next);
                depth++;
                return true;
            }

            private XmlElement CurrentElement {get {return (XmlElement)myElements.Peek();}}

            private XmlDocument myDocument;
            private Stack myElements;
            private int depth;

        }
	}
}
