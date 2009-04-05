// FitNesse.NET
// Copyright © 2006-2008 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit;
using fitlibrary.table;

namespace fitlibrary {

    public class TableAdapter: CellFactory {

        public bool CanMake(object theValue) {
            if (theValue == null) return false;
            Type tableType = typeof(Table);
            return (tableType.IsAssignableFrom(theValue.GetType()));
        }

        public Parse Make(object theValue, string theFormat) {
            return new Parse("td", string.Empty, MakeTable((Table)theValue, theFormat), null);
        }

        private static Parse MakeTable(Table theTable, string theFormat) {
            Parse rows = null;
            for (int row = theTable.Rows() - 1; row >= 0 ; row--) {
                Parse cells = null;
                for (int cell = theTable.Cells(row) - 1; cell >= 0 ; cell--) {
                    Parse newCell = CellFactoryRepository.Instance.Make(theTable.StringAt(row,cell), theFormat);
                    newCell.More = cells;
                    cells = newCell;
                }
                rows = new Parse("tr", string.Empty, cells, rows);
                
            }
            return new Parse("table", string.Empty,  rows, null);
        }
    }
}
