// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fitlibrary.exception;
using fitSharp.Machine.Model;

namespace fit.Model {
    public class CellRange: Tree<Cell> {
        private readonly IEnumerable<Parse> cells;

        public CellRange(Parse theCells): this(theCells, theCells != null ? theCells.Size : 0) {}

        public CellRange(Parse theCells, int theCellCount) {
            cells = Enumerate(theCells, theCellCount);
        }

        private static IEnumerable<Parse> Enumerate(Parse firstCell, int count) {
            Parse current = firstCell;
            for (int i = 0 ; i < count; i++, current = current.More) {
                yield return current;
            }
        }

        public IEnumerable<Parse> Cells {
            get {
                return cells;
            }
        }

        public Cell Value { get { return null; } }

        public bool IsLeaf { get { return false; } }

        public ReadList<Tree<Cell>> Branches {
            get {
                var result = new BranchList<Cell>();
                foreach (Parse currentCell in cells) {
                    result.Add(currentCell);
                }
                return result;
            }
        }

        public static CellRange GetMethodCellRange(Parse cells, int excludedCellCount) {
            Parse restOfTheRow = cells.More;
            if (restOfTheRow == null) {
                throw new FitFailureException("Missing cells for embedded method");
            }
            return new CellRange(restOfTheRow, restOfTheRow.Size - excludedCellCount);
        }
    }
}
