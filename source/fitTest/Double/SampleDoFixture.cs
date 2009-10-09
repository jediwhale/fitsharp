// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;

namespace fit.Test.Double {
    public class SampleDoFixture: DoFixture {
        public SampleDoFixture() {}
        public SampleDoFixture(string theText) {Text = theText;}

        public string Text;
        public int ArgumentCount { get { return Args.GetLength(0); }}
        public string GetArgument(int theIndex) { return Args[theIndex]; }

        public string GetArgumentInputAsString(int theIndex) { return (string)GetArgumentInput(theIndex, typeof(string)); }

        public int[] GetArgumentInputAsArray(int theIndex) {
            var result = new int[] {};
            result = (int[]) GetArgumentInput(theIndex, result.GetType());
            return result;
        }

        public int One() { return 1; }

        public RowFixture MakeSampleRow() {
            return new SampleRowFixture();
        }

        public Fixture MakeSampleColumn() {
            return new SampleColumnFixture();
        }

        public float DivideFloat(float x, float y) {
            return x/y;
        }

        public double DivideDouble(double x, double y) {
            return x/y;
        }

        public SampleDoFixture MakeSampleDo(string theText) {
            return new SampleDoFixture(theText);
        }
    }
}