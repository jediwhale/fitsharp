// FitLibrary for FitNesse .NET.
// Copyright (c) 2006 Syterra Software Inc. Released under the terms of the GNU General Public License version 2 or later.
// Based on designs from Fit (c) 2002 Cunningham & Cunningham, Inc., FitNesse by Object Mentor Inc., FitLibrary (c) 2003-2006 Rick Mugridge, University of Auckland, New Zealand.

using fit;
using fitSharp.Fit.Exception;

namespace fitlibrary.table {

    public class ParseTable: Table {

        public ParseTable(Parse theParse) {
            if (theParse == null) throw new FitFailureException("No embedded table.");
            myParse = theParse;
            myHashCode = myParse.ToString().GetHashCode();
        }

        public string StringAt(int theRow, int theCell) {
            Parse cell = Cell(theRow, theCell);
            return (cell == null || cell.Body == null ? "null" : cell.Text);
        }

        public Table TableAt(int theRow, int theCell) {
            Parse cell = Cell(theRow, theCell);
            if (cell == null) return null;
            return (cell.Parts != null ? new ParseTable(cell.Parts) : new ParseTable(cell));
        }

        public int Rows() {
            return myParse.Parts.Size;
        }

        public int Cells(int theRow) {
            return myParse.At(0, theRow).Parts.Size;
        }

        private Parse Cell(int theRow, int theCell) {
            Parse row = myParse.Parts;
            if (row == null || theRow >= row.Size) return null;
            Parse cell = row.At(theRow).Parts;
            if (cell == null || theCell >= cell.Size) return null;
            return cell.At(theCell);
        }

        public override string ToString() {
            return (myParse.Parts != null ? myParse.ToString() : myParse.Text);
        }

        public override bool Equals(object theOther) {
            Table other = theOther as Table;
            if (other == null) return false;
            return Equals(myParse, other);
        }

        private bool Equals(Parse theTable, Table theOtherTable) {
            int rowCount = 0;
            for (Parse row = theTable.Parts; row != null; row = row.More, rowCount++) {
                int cellCount = 0;
                for (Parse cell = row.Parts; cell != null; cell = cell.More, cellCount++) {
                   if (cell.Parts != null) {
                       if (!Equals(cell.Parts,  theOtherTable.TableAt(rowCount, cellCount))) return false;
                   }
                   else {
                       if (cell.Text != theOtherTable.StringAt(rowCount, cellCount)) return false;
                   }
                }
                if (theOtherTable.TableAt(rowCount, cellCount) != null) return false;
            }
            if (theOtherTable.TableAt(rowCount, 0) != null) return false;
            return true;
        }

        public override int GetHashCode() {
            return myHashCode;
        }

        private Parse myParse;
        private int myHashCode;
    }

}
