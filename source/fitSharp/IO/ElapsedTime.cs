// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;

namespace fitSharp.IO {
    public class ElapsedTime {
        private readonly DateTime start = Clock.Instance.UtcNow;

        public override string ToString() {
            TimeSpan elapsed = Clock.Instance.UtcNow - start;
            return string.Format("{0:####00}:{1:00}:{2:00}.{3:000}", elapsed.TotalHours, elapsed.Minutes,
                                 elapsed.Seconds, elapsed.Milliseconds);
        }
    }
}
