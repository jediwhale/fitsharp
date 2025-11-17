// Copyright © 2025 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

namespace fit.Test.Double {
    public class SampleColumnFixture: ColumnFixture {
        public SampleColumnFixture() {
            SetSystemUnderTest(new SampleDomain());
        }
    }
}
