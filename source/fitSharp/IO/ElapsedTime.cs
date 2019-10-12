// Copyright © 2019 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

namespace fitSharp.IO {
    public class ElapsedTime {

        public override string ToString() {
            var elapsed = Clock.Instance.Stop(watch);
            return elapsed.TotalMinutes < 1.0
                ? $"{elapsed.Seconds:#0}.{elapsed.Milliseconds:000}"
                : $"{elapsed.TotalHours:####00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}.{elapsed.Milliseconds:000}";
        }

        readonly int watch = Clock.Instance.Start();
    }
}
