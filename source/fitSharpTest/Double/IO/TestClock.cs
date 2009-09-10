// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.IO;

namespace fitSharp.Test.Double.IO {
    public class TestClock: TimeKeeper {
        public static TestClock Instance = new TestClock();
        public DateTime Now { get; set; }
        public DateTime UtcNow { get; set; }
    }
}