// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;

namespace fitSharp.IO {
    public interface ProgressReporter {
        void Write(string theMessage);
    }

    public static class ProgressReporterExtension {
        public static void WriteLine(this ProgressReporter reporter, string message) {
            reporter.Write(message);
            reporter.Write(Environment.NewLine);
        }
    }

    public class ConsoleReporter: ProgressReporter {
        public void Write(string theMessage) {
            Console.Write(theMessage);
        }
    }

    public class NullReporter: ProgressReporter {
        public void Write(string theMessage) {}
    }
}
