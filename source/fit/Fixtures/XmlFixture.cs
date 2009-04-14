// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Xml;
using fit;
using fit.Engine;
using fit.Operators;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
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
            public TypedValue[] ActualValues(Processor<Cell> processor, object theActualRow) {
                var actuals = (object[]) theActualRow;
                var result = new TypedValue[actuals.Length];
                for (int i = 0; i < actuals.Length; i++) result[i] = new TypedValue(actuals[i], actuals[i] == null ? typeof(void) : typeof(string));
                return result;
            }
            public bool IsExpectedSize(Parse theExpectedCells, object theActualRow) {
                return (theExpectedCells.Size == ((object[])theActualRow).Length);
            }
            public bool FinalCheck(TestStatus testStatus) {return true;}
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
