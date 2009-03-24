// FitNesse.NET
// Copyright © 2006,2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fit;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitlibrary {

	public class CellRange: Tree<Cell> {
	    private readonly IEnumerable<Parse> cells;

        public CellRange(IEnumerable<Parse> cells) {
            this.cells = cells;
        }

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
	            foreach (Parse cell in cells) yield return cell;
	        }
	    }

	    public override Cell Value { get { return null; } }

	    public override bool IsLeaf { get { return false; } }

	    public override ReadList<Tree<Cell>> Branches {
	        get {
	            var result = new BranchList<Cell>();
	            foreach (Parse currentCell in cells) {
                    result.Add(currentCell);
                }
	            return result;
	        }
	    }
	}
}
