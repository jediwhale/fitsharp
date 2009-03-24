// Copyright (c) 2004 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

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
            return new ParseTable(HtmlParser.Instance.Parse("<html><table><tr><td>one</td><td>two</td><td>three</td></tr></table></html>"));
        }
        public Table nullTable() {
            return null;
        }
    }
}