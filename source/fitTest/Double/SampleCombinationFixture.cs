// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitSharp.Samples;

namespace fit.Test.Double {
    public class SampleCombinationFixture: CombinationFixture {
        public SampleCombinationFixture() {
            Log.Clear();
        }

        public int Combine(int x, int y) {
            Log.Write(string.Format("Combine({0},{1})", x, y));
            return x * y;
        }

        public void SetUp() {
            Log.Write("SetUp()");
        }

        public void TearDown() {
            Log.Write("TearDown()");
        }
    }
}
