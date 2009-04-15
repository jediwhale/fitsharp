// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using System.Collections.Generic;
using fitSharp.Fit.Exception;

namespace fitSharp.Fit.Model {

    public interface AbandonException {}

    public class TestStatus {
        public bool IsAbandoned { get; set; }
        public Hashtable Summary { get; private set; }

        private readonly Dictionary<string, int> counts = new Dictionary<string, int>();
        
        public TestStatus() {
            Summary = new Hashtable();
        }

        public void TallyCounts(TestStatus other) {
            foreach (string cellStatus in other.counts.Keys) counts[cellStatus] = GetCount(cellStatus) + other.GetCount(cellStatus);
        }

        public void TallyPageCounts(TestStatus other) {
            AddCount(other.Style);
        }

        public void AddCount(string cellStatus) {
            counts[cellStatus] = GetCount(cellStatus) + 1;
        }

        public int FailCount { get { return GetCount(CellAttributes.WrongStatus) + GetCount(CellAttributes.ExceptionStatus); } }

        public int GetCount(string cellStatus) {
            return counts.ContainsKey(cellStatus) ? counts[cellStatus] : 0;
        }

        public string CountDescription {
            get {
                return string.Format("{0} right, {1} wrong, {2} ignored, {3} exceptions",
                                     GetCount(CellAttributes.RightStatus),
                                     GetCount(CellAttributes.WrongStatus),
                                     GetCount(CellAttributes.IgnoreStatus),
                                     GetCount(CellAttributes.ExceptionStatus));
            }
        }

        public string Letter {
            get {
                return GetCount(CellAttributes.ExceptionStatus) > 0 ? "E"
                           : (GetCount(CellAttributes.WrongStatus) > 0 ? "F" : ".");
            }
        }

        public string Style {
            get {
                if (GetCount(CellAttributes.ExceptionStatus) > 0) return CellAttributes.ExceptionStatus;
                if (GetCount(CellAttributes.WrongStatus) > 0) return CellAttributes.WrongStatus;
                if (GetCount(CellAttributes.RightStatus) > 0) return CellAttributes.RightStatus;
                return CellAttributes.IgnoreStatus;
            }
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

            if (abandonException != null) {
                IsAbandoned = true;
                throw abandonException;
            }
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