// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Document;

namespace fitSharp.Swim {
    public class ResultPage {
        private readonly List<List<ResultCell>> resultCells;
        private readonly List<string> columnHeaders;
        private readonly List<string> rowHeaders;

        public int ColumnCount { get { return columnHeaders.Count; } }
        public int RowCount { get { return rowHeaders.Count; } }

        public ResultPage(IEnumerable<Step> inputSteps) {
            columnHeaders = new List<string>();
            rowHeaders = new List<string>();
            resultCells = new List<List<ResultCell>>();
            foreach (Step step in inputSteps) {
                if (!step.HasAttribute(StepAttribute.Column)) continue;
                if (!step.HasAttribute(StepAttribute.Row)) continue;
                int row = rowHeaders.IndexOf(step.GetAttribute(StepAttribute.Row));
                if (row < 0) {
                    row = rowHeaders.Count;
                    rowHeaders.Add(step.GetAttribute(StepAttribute.Row));
                    resultCells.Add(new List<ResultCell>());
                }
                int column = columnHeaders.IndexOf(step.GetAttribute(StepAttribute.Column));
                if (column < 0) {
                    column = columnHeaders.Count;
                    columnHeaders.Add(step.GetAttribute(StepAttribute.Column));
                }
                while (resultCells[row].Count <= column) resultCells[row].Add(new ResultCell());
                resultCells[row][column].AddStep(step);
            }
        }

        public string GetColumnHeader(int column) { return columnHeaders[column]; }
        public string GetRowHeader(int row) { return rowHeaders[row]; }
        public ResultCell GetResultCell(int row, int column) { return resultCells[row][column]; }
    }
}
