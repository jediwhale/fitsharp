// Copyright © 2016 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using fitSharp.Fit.Exception;
using fitSharp.Machine.Model;

namespace fit.Model {
    public class CellRange: Tree<Cell> {
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
        public void Add(Tree<Cell> branch) { throw new InvalidOperationException("cell range is read only."); }

        public ReadList<Tree<Cell>> Branches {
            get {
                var result = new BranchList<Cell>();
                foreach (Parse currentCell in cells) {
                    result.Add(currentCell);
                }
                return result;
            }
        }

        readonly IEnumerable<Parse> cells;
    }
}
