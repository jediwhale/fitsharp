// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using fitlibrary;
using fitlibrary.exception;

namespace fit.Test.Acceptance {
    public class SystemUnderTest {
        private int mySum = 0;
        private string concat = "";
        private int myValue = 0;
        private int myIntProperty = 2;

        public int add(int i) {
            mySum += i;
            return mySum;
        }
        public int sum() {
            return mySum;
        }
        public int plus() {
            return sum();
        }
        public void addAndAppend(int i, string s) {
            add(i);
            concat += s;
        }
        public void addAmpersandAppend(int i, string s) {
            addAndAppend(i,s);
        }
        public string appends() {
            return concat;
        }
        public string plusPlus() {
            return appends();
        }
        public bool aRightAction(int i) {
            return true;
        }
        public bool aWrongAction(double x, double y) {
            return false;
        }
        public int anExceptionAction() {
            throw new ApplicationException("testing");
        }
        public void setValue(int i) {
            myValue = i;
        }
        public void add() {
            add(myValue);
        }
        public DateTime sameDate(DateTime date) {
            return date;
        }
        public void hiddenMethod() {
            throw new FitFailureException("testing");
        }
        
        public Fixture anotherObject() {
            return new SequenceFixture(new A());
        }
        public  class A {
            public bool accessOther() {
                return true;
            }
            public int otherInt() {
                return 4;
            }
        }
        
        public bool booleanProperty() {
            return true;
        }
        public bool isBooleanProperty() {
            return true;
        }
        public int IntProperty {
            get {return myIntProperty;}
            set { myIntProperty = value;}
        }
        public int IntField = 2;
            
        public Object[] anArrayOfPoint() {
            return new Object[] { new System.Drawing.Point(0,0), new System.Drawing.Point(5,5) };
        }
        
        public IList aListOfPoint() {
            IList list = new ArrayList();
            list.Add(new System.Drawing.Point(0,0));
            list.Add(new System.Drawing.Point(5,5));
            return list;
        }
        
        public IList copyOfListOfPoint(IList list) {
            return list;
        }
        public Point SutXSutY(int x, int y) {
            return new Point(x,y);
        }
        public IEnumerator anIteratorOfPoint() {
            return aListOfPoint().GetEnumerator();
        }
        /*
        public IDictionary aSetOfPoint() {
            Hashtable set = new Hashtable();
            set.add(new Point(0,0));
            set.add(new Point(5,5));
            return set;
        }
        public SortedSet aSortedSetOfPoint() {
            SortedSet set = new TreeSet(new Comparator() {
                                            public int compare(Object p1, Object p2) {
                                                Point pt1 = (Point)p1;
                                                Point pt2 = (Point)p2;
                                                if (pt1.x < pt2.x)
                                                    return -1;
                                                if (pt1.x > pt2.x)
                                                    return 1;
                                                if (pt1.y < pt2.y)
                                                    return -1;
                                                if (pt1.y > pt2.y)
                                                    return 1;
                                                return 0;
                                            }});
    set.add(new Point(0,0));
    set.add(new Point(5,5));
    return set;
}*/
        public IDictionary aMapOfPoint() {
            Hashtable map = new Hashtable();
            map.Add("0,0",new System.Drawing.Point(0,0));
            map.Add("5,5",new System.Drawing.Point(5,5));
            return map;
        }

        public int plusAB(int a, int b) {
            return a + b;
        }
        public string shown() {
            return "<ul><li>ita<li>lics</ul>";
        }
    }
}