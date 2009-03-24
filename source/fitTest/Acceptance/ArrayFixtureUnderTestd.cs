// FitLibrary for FitNesse .NET.
// Copyright (c) 2005 Syterra Software Inc. Released under the terms of the GNU General Public License version 2 or later.

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