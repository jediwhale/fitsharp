// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.Diagnostics;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Loggers {
    public class ElapsedTime: Logger {
        public void Register(Logging loggingFacility) {
            loggingFacility.BeginCellEvent += BeginCell;
            loggingFacility.EndCellEvent += EndCell;
        }

        void BeginCell(Cell cell) {
            var stopwatch = new Stopwatch();
            cells.Add(cell, stopwatch);
            stopwatch.Start();
        }

        void EndCell(Cell cell) {
            if (!cells.ContainsKey(cell)) return;
            var stopwatch = cells[cell];
            stopwatch.Stop();
            cell.SetAttribute(CellAttribute.Title, string.Format("elapsed: {0} ms", stopwatch.ElapsedMilliseconds));
        }

        readonly Dictionary<Cell, Stopwatch> cells = new Dictionary<Cell, Stopwatch>();
    }
}
