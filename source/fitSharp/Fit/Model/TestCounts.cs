// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;

namespace fitSharp.Fit.Model {
    public class TestCounts {
        private readonly Dictionary<string, int> counts = new Dictionary<string, int>();

        public TestCounts() {}

        public TestCounts(TestCounts other) {
            counts = new Dictionary<string, int>(other.counts);
        }

        public TestCounts Subtract(TestCounts other) {
            var result = new TestCounts(this);
            foreach (string cellStatus in other.counts.Keys) result.counts[cellStatus] = result.GetCount(cellStatus) - other.GetCount(cellStatus);
            return result;
        }

        public int GetCount(string cellStatus) {
            return counts.ContainsKey(cellStatus) ? counts[cellStatus] : 0;
        }

        public void AddCount(string cellStatus) {
            if (string.IsNullOrEmpty(cellStatus)) return;
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
                                     GetCount(TestStatus.Right),
                                     GetCount(TestStatus.Wrong),
                                     GetCount(TestStatus.Ignore),
                                     GetCount(TestStatus.Exception));
            }
        }

        public int FailCount { get { return GetCount(TestStatus.Wrong) + GetCount(TestStatus.Exception); } }

        public string Letter {
            get {
                return GetCount(TestStatus.Exception) > 0 ? "E"
                           : (GetCount(TestStatus.Wrong) > 0 ? "F" : ".");
            }
        }

        public string Style {
            get {
                if (GetCount(TestStatus.Exception) > 0) return TestStatus.Exception;
                if (GetCount(TestStatus.Wrong) > 0) return TestStatus.Wrong;
                if (GetCount(TestStatus.Right) > 0 ) return TestStatus.Right;
                if (GetCount(TestStatus.Ignore) > 0) return TestStatus.Ignore;
                return string.Empty;
            }
        }

    }
}
