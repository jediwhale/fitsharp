// Copyright (c) 2003 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

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