// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitSharp.Samples;

namespace fit.Test.Double {
    public class SampleDoFixture: DoFixture {
        public SampleDoFixture(): base(new SampleDomain()) {
            Log.Clear();
        }

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

        public int One() {
            Log.Write("One()");
            return 1;
        }

        public bool Summary { get { return true; } }

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

        public string GetName(SampleDomain sample) { return sample.Name; }

        public void CheckDigit(Parse cells) {
            var cell = cells.More;
            if (char.IsDigit(cell.Text, 0)) {
                Right(cell);
            } else {
                Wrong(cell);
            }
        }

        public void SetUp() {
            Log.Write("SetUp()");
        }

        public void TearDown() {
            Log.Write("TearDown()");
        }
    }
}