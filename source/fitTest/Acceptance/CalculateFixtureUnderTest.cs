// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Globalization;
using fitlibrary;
using fitlibrary.tree;

namespace fit.Test.Acceptance {
    public class CalculateFixtureUnderTest: CalculateFixture {
        private int count = 1;
	
        public int plusAB(int a, int b) {
            return a + b;
        }
        public int sum(int a, int b) {
            return a + b;
        }
        public int minusAB(int a, int b) {
            return a - b;
        }
        public int plusA(int a) {
            return a;
        }
        public string doubleA(string a) {
            return a + a;
        }
        public string getCamelFieldName(string name) {
            return name;
        }
        public string plusName(string name) {
            return name+"+";
        }
        public string exceptionMethod() {
            throw new ApplicationException("test");
        }
        public void voidMethod() {
        }
        public int increment() {
            return count++;
        }
        public Calendar useCalendar(Calendar calendar) {
            return calendar;
        }
        public ListTree plus12(Tree t1, Tree t2) {
            return new ListTree("", new Tree[]{ t1, t2 });
        }
    }
}