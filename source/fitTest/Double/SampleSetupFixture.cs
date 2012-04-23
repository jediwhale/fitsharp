// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitSharp.Samples;

namespace fit.Test.Double {
    public class SampleSetupFixture: SetUpFixture {

        public SampleSetupFixture() {
            Log.Clear();
        }

        public void Ab(int a, int b) {
            Log.Write(string.Format("Ab({0},{1})", a, b));
        }

        protected override void SetUp() {
            Log.Write("SetUp()");
        }

        protected override void TearDown() {
            Log.Write("TearDown()");
        }
    }
}
