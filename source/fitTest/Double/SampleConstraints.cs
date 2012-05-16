// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
using fitlibrary;
using fitSharp.Samples;

namespace fit.Test.Double {
    public class SampleConstraints {
        public ConstraintFixture MakeFixture() {
            return new SampleConstraint(true);
        }

        public ConstraintFixture MakeFixture(bool expectedValue) {
            return new SampleConstraint(expectedValue);
        }

        public ConstraintFixture MakeFixtureRepeat(string repeatString) {
            var result = new SampleConstraint(true) {RepeatString = repeatString};
            return result;
        }

    }

    public class SampleConstraint: ConstraintFixture {
        public SampleConstraint() {
            Log.Clear();
        }

        public SampleConstraint(bool expectedValue): base(expectedValue) {
            Log.Clear();
        }

        public bool GreaterThan(int x, int y) {
            Log.Write(string.Format("GreaterThan({0},{1})", x, y));
            return x > y;
        }

        public int AddTo(int x, int y) {
            return x + y;
        }

        public void SetUp() {
            Log.Write("SetUp()");
        }

        public void TearDown() {
            Log.Write("TearDown()");
        }
    }
}
