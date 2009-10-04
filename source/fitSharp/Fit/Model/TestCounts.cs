// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;

namespace fitSharp.Fit.Model {
    public class TestCounts {
        private readonly Dictionary<string, int> counts = new Dictionary<string, int>();

        public int GetCount(string cellStatus) {
            return counts.ContainsKey(cellStatus) ? counts[cellStatus] : 0;
        }

        public void AddCount(string cellStatus) {
            counts[cellStatus] = GetCount(cellStatus) + 1;
        }

        public void TallyCounts(TestCounts other) {
            foreach (string cellStatus in other.counts.Keys) counts[cellStatus] = GetCount(cellStatus) + other.GetCount(cellStatus);
        }

        public void TallyPageCounts(TestCounts other) {
            AddCount(other.Style);
        }

        public string Description {
            get {
                return string.Format("{0} right, {1} wrong, {2} ignored, {3} exceptions",
                                     GetCount(CellAttributes.RightStatus),
                                     GetCount(CellAttributes.WrongStatus),
                                     GetCount(CellAttributes.IgnoreStatus),
                                     GetCount(CellAttributes.ExceptionStatus));
            }
        }

        public int FailCount { get { return GetCount(CellAttributes.WrongStatus) + GetCount(CellAttributes.ExceptionStatus); } }

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
                return GetCount(CellAttributes.RightStatus) > 0 ? CellAttributes.RightStatus : CellAttributes.IgnoreStatus;
            }
        }

    }
}
