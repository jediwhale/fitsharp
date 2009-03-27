// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Data;
using System.Xml;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class DoFixtureFlowUnderTest: DoFixture {

        //private static SimpleDateFormat DATE_FORMAT = new SimpleDateFormat("yyyy/MM/dd HH:mm");

        public DoFixtureFlowUnderTest(): base(new SystemUnderTest()) {
            //registerParseDelegate(Date.class,DATE_FORMAT);
        }
        public Point xY(int x, int y) {
            return new Point(x,y);
        }
        public bool right() {
            throw new ApplicationException("Ambiguity with special action");
        }
        public bool summary() {
            return true;
        }
        public bool Action(Parse cells) {
            return true;
        }
        public void specialAction(Parse cells) {
            cells = cells.More;
            if (cells.Text == "right")
                Right(cells);
            else if (cells.Text == "wrong")
                Wrong(cells);
        }
        public void hiddenMethod() {
        }
        public Fixture fixtureObject(int initial) {
            return new MyColumnFixture(initial);
        }
        public object aPoint() {
            return new System.Drawing.Point(2,3);
        }
        public DateTime getDate() {
            return new DateTime(2004-1900,2,3);
        }
        public void getException() {
            throw new ApplicationException("Forced exception");
        }
        public Integer anInteger() {
            return new Integer(23);
        }
        public MyClass myClass() {
            return new MyClass(3);
        }
        public ClassWithNoTypeAdapter useToString() {
            return new ClassWithNoTypeAdapter();
        }
        public class ClassWithNoTypeAdapter {
            public String toString() {
                return "77";
            }
        }
        public class MyClass {
            private int i;

            public MyClass(int i) {
                this.i = i;
            }
            public static MyClass Parse(string s) {
                return new MyClass(Int32.Parse(s));
            }
            public override string ToString() {
                return ""+i;
            }
            public override bool Equals(object Object) {
                if (!(Object is MyClass))
                    return false;
                return ((MyClass)Object).i == i;
            }
            public override int GetHashCode() {
                return i;
            }
        }
        public class MyColumnFixture: fit.ColumnFixture {
            public int x = 0;
            public MyColumnFixture(int initial) {
                x = initial;
            }
            public int getX() {
                return x;
            }
        }
        public Fixture getSlice(int row, int column) {
            return new LocalRowFixture(row,column);
        }
        public class LocalRowFixture: fit.RowFixture {
            private Local[,][] rows = { { new Local[] {
                new Local("A0a"), new Local("A0b") }, new Local[] {
                    new Local("A1a"), new Local("A1b") }, new Local[] {
                        new Local("A2a"), new Local("A2b") }, new Local[] {
                            new Local("A3a"), new Local("A3b") }
            }, { new Local[] {
                new Local("B0a"), new Local("B0b") }, new Local[] {
                    new Local("B1a"), new Local("B1b") }, new Local[] {
                        new Local("B2a"), new Local("B2b") }, new Local[] {
                            new Local("B3a"), new Local("B3b") }
            }
            };
            private int row, column;
		
            public LocalRowFixture(int row, int column) {
                this.row = row;
                this.column = column;
            }
            public override object[] Query() {
                return rows[row,column];
            }
            public override Type GetTargetClass() {
                return typeof(Local);
            }
        }
        public class Local {
            public string s;

            public Local(string s) {
                this.s = s;
            }
        }
        public PointHolder PointHolder {get {return new PointHolder();}}

        public WithStatic aClassWithStatic {get {return new WithStatic();}}

        public DataTable aTable() {
            DataTable table = new DataTable();
            table.Columns.Add("x", typeof(int));
            table.Columns.Add("y", typeof(int));
            DataRow row;
            row = table.NewRow();
            row["x"] = 0;
            row["y"] = 0;
            table.Rows.Add(row);
            row = table.NewRow();
            row["x"] = 5;
            row["y"] = 5;
            table.Rows.Add(row);
            return table;
        }

        public XmlDocument aDocument() {
            XmlDocument document = new XmlDocument();
            document.LoadXml("<root><child>text</child></root>");
            return document;
        }
    }

    public class PointHolder {
        public System.Drawing.Point Point {get {return new System.Drawing.Point(24,7);}}
    }

    public class WithStatic{
        public static int StaticMethod(string s) {
            return s.Length;
        }
        public int NonStaticMethod() {
            return 99;
        }
    }

    public class Integer {
        public Integer(int theValue) {myValue = theValue;}
        public static int ParseInt(string theValue) {return Int32.Parse(theValue);}
        public double DoubleValue() {return (double)myValue;}
        private int myValue;
    }
}