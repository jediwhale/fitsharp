// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fitlibrary;

namespace fit.Test.Double {
    public class SampleArrayFixture: ArrayFixture {
        public static List<SampleItem> List;
        public SampleArrayFixture(): base(List) {}
    }

    public class SetUpSampleArrayFixture: SetUpFixture {
        public SetUpSampleArrayFixture() {
            SampleArrayFixture.List = new List<SampleItem>();
        }
        public void ColumnA(string theValue) {
            var item = new SampleItem {ColumnA = theValue};
            SampleArrayFixture.List.Add(item);
        }
        public void ColumnAColumnB(string valueA, string valueB) {
            var item = new SampleItem {ColumnA = valueA, ColumnB = valueB};
            SampleArrayFixture.List.Add(item);
        }
    }

    public class SampleItem {
        public string ColumnA;
        public string ColumnB;
    }
}
