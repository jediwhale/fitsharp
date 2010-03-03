// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitlibrary.table;

namespace fit.Test.Acceptance {
    public class DoTable: DoFixture {

        public Table FirstCellValue(Table theTable) {
            return theTable.TableAt(0, 0);
        }

        public string FirstCellStringValue(Table theTable) {
            return theTable.StringAt(0, 0);
        }

        public Table aTable() {
            return new ParseTable(Parse.ParseFrom("<html><table><tr><td>one</td><td>two</td><td>three</td></tr></table></html>"));
        }
        public Table nullTable() {
            return null;
        }
    }
}