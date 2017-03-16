// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections;
using fitSharp.Fit.Exception;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Model {

    public interface AbandonException {}

    public class TestStatus {
        public delegate void TestCountChangedHandler(TestCounts count);
        public event TestCountChangedHandler TestCountChanged;

        public const string Exception = "error";
        public const string Wrong = "fail";
        public const string Ignore = "ignore";
        public const string Right = "pass";

        public int TableCount { get; set; }
        public bool IsAbandoned { get; set; }
        public bool SuiteIsAbandoned { get; private set; }
        public string LastAction { get; set; }
        public Hashtable Summary { get; private set; }
        public TestCounts Counts { get; private set; }

        public TestStatus() {
            Reset();
        }

        public void Reset() {
            TableCount = 0;
            IsAbandoned = false;
            SuiteIsAbandoned = false;
            LastAction = string.Empty;
            Summary = new Hashtable();
            Counts = new TestCounts();
        }

        public void MarkRight(Cell cell) {
            cell.SetAttribute(CellAttribute.Status, Right);
            AddCount(Right);
        }

        public void MarkWrong(Cell cell) {
            cell.SetAttribute(CellAttribute.Status, Wrong);
            AddCount(Wrong);
        }

        public void MarkWrong(Cell cell, string actual) {
            cell.SetAttribute(CellAttribute.Actual, actual);
            MarkWrong(cell);
        }

        public void MarkIgnore(Cell cell) {
            cell.SetAttribute(CellAttribute.Status, Ignore);
            AddCount(Ignore);
        }

        public void MarkException(Cell cell, System.Exception exception) {
            if (exception is IgnoredException) return;

            var abandonException = GetAbandonException(exception);

            if (abandonException != null && IsAbandoned) throw abandonException;

            if (cell.GetAttribute(CellAttribute.Status) != Exception) {
                cell.SetAttribute(CellAttribute.Exception, (abandonException ?? exception).ToString());
                cell.SetAttribute(CellAttribute.Status, Exception);
                AddCount(Exception);
            }

            if (abandonException == null) return;

            IsAbandoned = true;
            if (abandonException is AbandonSuiteException) SuiteIsAbandoned = true;
            throw abandonException;
        }

        void AddCount(string cellStatus) {
            Counts.AddCount(cellStatus);
            if (TestCountChanged != null) TestCountChanged(Counts);
        }

        static System.Exception GetAbandonException(System.Exception exception) {
            for (var e = exception; e != null; e = e.InnerException) {
                if (e is AbandonException) return e;
            }
            return null;
        }

        public void ColorCell(Cell cell, bool isRight) {
            if (isRight)
                MarkRight(cell);
            else
                MarkWrong(cell);
        }

        public void MarkCellWithLastResults(Cell cell) {
            MarkCellWithLastResults(cell, c => { });
        }

        public void MarkCellWithLastResults(Cell cell, TestCounts beforeCounts) {
            MarkCellWithLastResults(cell, c => MarkWithCounts(c, beforeCounts));
        }

        void MarkCellWithLastResults(Cell cell, Action<Cell> markWithCounts) {
            if (cell != null && !string.IsNullOrEmpty(LastAction)) {
                cell.SetAttribute(CellAttribute.Folded, LastAction);
                markWithCounts(cell);
            }
            LastAction = null;
        }

        void MarkWithCounts(Cell cell, TestCounts beforeCounts) {
            var style = Counts.Subtract(beforeCounts).Style;
            if (!string.IsNullOrEmpty(style) && string.IsNullOrEmpty(cell.GetAttribute(CellAttribute.Status))) {
                cell.SetAttribute(CellAttribute.Status, style);
            }
        }
    }
}
