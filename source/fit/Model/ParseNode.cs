// FitNesse.NET
// Copyright © 2006, 2008 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit;
using fitSharp.Machine.Model;

namespace fitlibrary {

	public class ParseNode {

        public static bool IsTable(Parse theNode) {
            return theNode != null && ourTableIdentifier.IsStartOf(theNode.Tag);
        }

        public static Parse MakeCells(params object[] theContents) {
            Parse cells = null;
            for (int i = theContents.Length-1; i >=0; i--) {
                cells = new Parse("td", theContents[i].ToString(), null, cells);
            }
            return cells;
        }
    
        public static Parse MakeRows(params object[] theCells) {
            Parse rows = null;
            for (int i = theCells.Length-1; i >=0; i--) {
                rows = new Parse("tr", null, (Parse)theCells[i], rows);
            }
            return rows;
        }

        public static Parse MakeTables(params object[] theRows) {
            Parse tables = null;
            for (int i = theRows.Length-1; i >=0; i--) {
                tables = new Parse("table", null, (Parse)theRows[i], tables);
            }
            return tables;
        }

	    private static readonly IdentifierName ourTableIdentifier = new IdentifierName("<table");
    }
}
