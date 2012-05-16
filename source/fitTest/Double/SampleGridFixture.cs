// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using System.Collections.Generic;

namespace fit.Test.Double {
    public class SampleGridFixture: GridFixture {
        public SampleGridFixture(): base(SetupSampleGridFixture.Grid) {}
    }

    public class SetupSampleGridFixture: SetUpFixture {
        public static object[][] Grid { get { return list.ToArray(); } }

        static List<object[]> list;

        public SetupSampleGridFixture() {
            list = new List<object[]>();
        }

        public void Zero12(string zero, string one, string two) {
            list.Add(new[] {zero, one, two});
        }
    }
}
