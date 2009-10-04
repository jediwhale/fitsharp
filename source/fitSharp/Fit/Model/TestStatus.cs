// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using fitSharp.Fit.Exception;

namespace fitSharp.Fit.Model {

    public interface AbandonException {}

    public class TestStatus {
        public bool IsAbandoned { get; set; }
        public string LastAction { get; set; }
        public Hashtable Summary { get; private set; }
        public TestCounts Counts { get; private set; }
        
        public TestStatus() {
            Summary = new Hashtable();
            Counts = new TestCounts();
        }

        public void MarkRight(Cell cell) {
            cell.SetAttribute(CellAttributes.StatusKey, CellAttributes.RightStatus);
            AddCount(CellAttributes.RightStatus);
        }

        public void MarkWrong(Cell cell) {
            cell.SetAttribute(CellAttributes.StatusKey, CellAttributes.WrongStatus);
            AddCount(CellAttributes.WrongStatus);
        }

        public void MarkWrong(Cell cell, string actual) {
            cell.SetAttribute(CellAttributes.ActualKey, actual);
            MarkWrong(cell);
        }

        public void MarkIgnore(Cell cell) {
            cell.SetAttribute(CellAttributes.StatusKey, CellAttributes.IgnoreStatus);
            AddCount(CellAttributes.IgnoreStatus);
        }

        public void MarkException(Cell cell, System.Exception exception) {
            if (exception is IgnoredException) return;

            System.Exception abandonException = GetAbandonStoryTestException(exception);

            if (abandonException != null && IsAbandoned) throw abandonException;

            if (cell.GetAttribute(CellAttributes.StatusKey) != CellAttributes.ExceptionStatus) {
                cell.SetAttribute(CellAttributes.ExceptionKey, exception.ToString());
                cell.SetAttribute(CellAttributes.StatusKey, CellAttributes.ExceptionStatus);
                AddCount(CellAttributes.ExceptionStatus);
            }

            if (abandonException == null) return;

            IsAbandoned = true;
            throw abandonException;
        }

        private void AddCount(string cellStatus) {
            Counts.AddCount(cellStatus);
        }

        private static System.Exception GetAbandonStoryTestException(System.Exception exception) {
            for (System.Exception e = exception; e != null; e = e.InnerException) {
                if (typeof(AbandonException).IsAssignableFrom(e.GetType())) return e;
            }
            return null;
        }

        public void ColorCell(Cell cell, bool isRight) {
            if (isRight)
                MarkRight(cell);
            else
                MarkWrong(cell);
        }
    }
}