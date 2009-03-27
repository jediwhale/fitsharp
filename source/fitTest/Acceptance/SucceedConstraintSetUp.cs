// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class SucceedConstraintSetUp: ConstraintFixture {
        public void SetUp() {
            IAmSetUp = true;
        }
        public void TearDown() {
            throw new ApplicationException("tear down");
        }
        public bool AB(int a, int b) {
            return IAmSetUp && a < b;
        }
        public SucceedConstraintSetUp DoIt() {
            return new SucceedConstraintSetUp();
        }
        private bool IAmSetUp = false;
    }
}