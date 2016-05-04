// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

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

        public void SetCount(string cellStatus, int count) {
            counts.Add(cellStatus, count);
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
