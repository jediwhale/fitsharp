// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using System.Data;
using System.Linq;
using fitlibrary;
using fitSharp.Samples;

namespace fit.Test.Double {
    public class SampleArrayFixture: ArrayFixture {
        public SampleArrayFixture(): base(SetUpSampleArrayFixture.List) {}
    }

    public class SampleTableArrayFixture: ArrayFixture {
        public SampleTableArrayFixture(): base(SetUpSampleArrayFixture.Table) {}
    }

    public class SampleMixedArrayFixture: ArrayFixture {
        public SampleMixedArrayFixture(): base(SetUpSampleArrayFixture.Mixed) {}
    }

    public class SetUpSampleArrayFixture: SetUpFixture {
        public static List<object> List;

        public static DataTable Table {
            get {
                var table = new DataTable();
                table.Columns.Add("columna", typeof(string));
                table.Columns.Add("columnb", typeof(string));
                foreach (SampleItem item in List) {
                    var row = table.NewRow();
                    row["columna"] = item.ColumnA;
                    row["columnb"] = item.ColumnB;
                    table.Rows.Add(row);
                }
                return table;
            }
        }

        public static List<object> Mixed {
            get {
                var mixed = new List<object> {List[0]};
                foreach (SampleItem item in List.Skip(1)) {
                    mixed.Add(new Dictionary<string, string> {{"columna", item.ColumnA}, {"columnb", item.ColumnB}});
                }
                return mixed;
            }
        }

        public SetUpSampleArrayFixture() {
            List = new List<object>();
        }

        public void ColumnA(string theValue) {
            var item = new SampleItem {ColumnA = theValue};
            List.Add(item);
        }

        public void ColumnAColumnB(string valueA, string valueB) {
            var item = new SampleItem {ColumnA = valueA, ColumnB = valueB};
            List.Add(item);
        }

        public void ColumnAColumnBOther(string valueA, string valueB, string other) {
            if (!string.IsNullOrEmpty(valueA)) {
                List.Add(new SampleItem {ColumnA = valueA, ColumnB = valueB});
            }
            else {
                List.Add(new OtherItem{Other = other});
            }
        }

        public void MotherFather(Person mother, Person father) {
            List.Add(new Parents {Mother = mother, Father = father});
        }
    }

    public class SampleItem {
        public string ColumnA;
        public string ColumnB;
    }

    public class OtherItem {
        public string Other;
    }

    public class Parents {
        public Person Mother;
        public Person Father;
    }
}
