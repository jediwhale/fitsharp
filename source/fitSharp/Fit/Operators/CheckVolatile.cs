// Copyright © 2013 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Threading;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CheckVolatile : CellOperator, CheckOperator {
        public bool CanCheck(CellOperationValue actualValue, Tree<Cell> expectedCell) {
            return actualValue.IsVolatile;
        }

        public TypedValue Check(CellOperationValue actualValue, Tree<Cell> expectedCell) {
            try {
                CheckEventuallyMatches(actualValue, expectedCell);
            }
            catch (IgnoredException) {}
            Processor.TestStatus.MarkCellWithLastResults(expectedCell.Value);
            return TypedValue.Void;
        }

        void CheckEventuallyMatches(CellOperationValue actualValue, Tree<Cell> expectedCell) {
            var wait = Processor.Memory.GetItem<Settings>().WaitTime;
            if (wait == 0) wait = defaultWaitTime;
            var sleep = 0;
            TypedValue actual = TypedValue.Void;
            for (var count = 0; count < 100; count++) {
                Processor.Get<Logging>().BeginCell(expectedCell.Value);
                try {
                    actual = actualValue.GetTypedActual(Processor);
                }
                finally {
                    Processor.Get<Logging>().EndCell(expectedCell.Value);
                }
                if (Processor.Compare(actual,expectedCell)) {
                    Processor.TestStatus.MarkRight(expectedCell.Value);
                    return;
                }
                var sleepThisTime = (wait*count)/100 - sleep;
                if (sleepThisTime <= 0) continue;
                Thread.Sleep(sleepThisTime);
                sleep += sleepThisTime;
            }
            var actualCell = Processor.Compose(actual);
            Processor.TestStatus.MarkWrong(expectedCell.Value, actualCell.Value.Text);
        }

        const int defaultWaitTime = 1000;
    }
}
