// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitSharp.Samples;

namespace fit.Test.Double {
    public class SampleCalculates {
        public CalculateFixture MakeFixtureWithRepeat(string repeat) {
            return new SampleCalculateFixture(repeat);
        }
    }

    public class SampleCalculateFixture: CalculateFixture {
        public SampleCalculateFixture() {
            Log.Clear();
        }

        public SampleCalculateFixture(string repeat): this() {
            RepeatString = repeat;
        }

        public int PlusAB(int a, int b) {
            Log.Write(string.Format("PlusAB({0},{1})", a, b));
            return a + b;
        }

        public int Plus(int a, int b) {
            return a + b;
        }

        public int Plus1A(int a) {
            return a + 1;
        }

        public int MinusAB(int a, int b) {
            return a - b;
        }

        public string DoubleA(string a) {
            return a + a;
        }

        public int Increment() {
            return ++count;
        }

        public void VoidA(int a) {}

        public void SetUp() {
            Log.Write("SetUp()");
        }

        public void TearDown() {
            Log.Write("TearDown()");
        }
        int count;
    }
}
