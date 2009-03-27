// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class SampleArrayFixture: ArrayFixture {
        public static List<SampleItem> List;
        public SampleArrayFixture(): base(List) {}
    }

    public class SetUpSampleArrayFixture: SetUpFixture {
        public SetUpSampleArrayFixture() {
            SampleArrayFixture.List = new List<SampleItem>();
        }
        public void ColumnA(string theValue) {
            SampleItem item = new SampleItem();
            item.ColumnA = theValue;
            SampleArrayFixture.List.Add(item);
        }
    }

    public class SampleItem {
        public string ColumnA;
        public string ColumnB;
    }
}