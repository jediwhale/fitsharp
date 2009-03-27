// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class AbstractNestedLists: DoFixture {
        public class Fruit {
            private string myName;
            private IList list = new ArrayList();
		
            public Fruit(string name, IList list) {
                this.myName = name;
                this.list = list;
            }
            public string Name {get {return myName;}}

            public IList Elements {get {return list;}}

            public override string ToString() {
                return "Fruit("+myName+","+list+")";
            }
        }
        public class Element {
            private string myId;
            private int myCount;
            private IList mySubElements = new ArrayList();

            public Element(string id, int count) {
                this.myId = id;
                this.myCount = count;
            }
            public Element(string id, int count, IList list): this(id,count) {
                this.mySubElements = list;
            }
            public int Count {
                get {return myCount;}
            }
            public string Id {
                get {return myId;}
            }
            public IList SubElements {
                get {return mySubElements;}
            }
            public string toString() {
                return "Element("+myId+","+myCount+")";
            }
        }
        public class SubElement {
            private string a, b;

            public SubElement(string a, string b) {
                this.a = a;
                this.b = b;
            }
            public string getA() {
                return a;
            }
            public string getB() {
                return b;
            }
            public string toString() {
                return "SubElement("+a+","+b+")";
            }
        }
    }
}