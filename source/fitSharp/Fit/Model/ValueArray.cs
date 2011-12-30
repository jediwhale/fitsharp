// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Model {

	public class ValueArray {
        public ValueArray(string repeatKeyword) {
            this.repeatKeyword = repeatKeyword;
        }

        public Tree<Cell> GetCells(IEnumerable<Tree<Cell>> sourceCells) {
            var currentCells = new List<Tree<Cell>>();
            var cellCount = 0;
            foreach (var source in sourceCells) {
                if (source.Value.Text == repeatKeyword && cellCount < previousCells.Count) {
                    currentCells.Add(previousCells[cellCount]);
                }
                else {
                    currentCells.Add(source);
                    if (cellCount < previousCells.Count) {
                        previousCells[cellCount] = source;
                    }
                    else {
                        previousCells.Add(source);
                    }
                }
                cellCount++;
            }
            return new EnumeratedTree<Cell>(currentCells);
        }

        readonly string repeatKeyword;
	    readonly List<Tree<Cell>> previousCells = new List<Tree<Cell>>();
    }
}
