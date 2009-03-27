// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Data;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class ArrayFixtureUnderTestd: ArrayFixture {

        public ArrayFixtureUnderTestd(): base(MakeDataTable()) {}

        public static DataTable MakeDataTable() {
            DataTable table = new DataTable();
            table.Columns.Add("columna", typeof(int));
            table.Columns.Add("columnb", typeof(string));
            DataRow row;
            row = table.NewRow();
            row["columna"] = 1;
            row["columnb"] = "two";
            table.Rows.Add(row);
            row = table.NewRow();
            row["columna"] = 1;
            row["columnb"] = "one";
            table.Rows.Add(row);
            row = table.NewRow();
            row["columna"] = 2;
            row["columnb"] = "two";
            table.Rows.Add(row);
            return table;
        }
    }
}