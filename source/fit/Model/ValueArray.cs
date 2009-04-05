// FitNesse.NET
// Copyright © 2006, 2007 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fit;
using fit.Model;

namespace fitlibrary {

	public class ValueArray {

	    private readonly List<Parse> previousCells = new List<Parse>();

        public ValueArray(string theRepeatKeyword) {
            myRepeatKeyword = theRepeatKeyword;
        }

        public CellRange GetCells(IEnumerable<Parse> sourceCells) {
            var currentCells = new List<Parse>();
            int cellCount = 0;
            foreach (Parse source in sourceCells) {
                if (source.Text == myRepeatKeyword && cellCount < previousCells.Count) {
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
            return new CellRange(currentCells);
        }

        private readonly string myRepeatKeyword;
    }
}
