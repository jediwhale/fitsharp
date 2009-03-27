// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;

namespace fit.Test.Acceptance {
    public class SucceedConstraint: ConstraintFixture {
        public SucceedConstraint() {
            RepeatString = "ditto";
        }
        public bool aB(int a, int b) {
            return a < b;
        }
        public int bC(int b, int c) {
            return b + c;
        }
    }
}