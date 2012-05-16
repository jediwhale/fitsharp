// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

namespace fitSharp.Samples {
    public class Log {
        public static string Content { get; private set; }

        static Log() { Clear(); }

        public static void Clear() {
            Content = string.Empty;
        }

        public static void Write(string message) {
            if (Content.Length > 0) Content += " ";
            Content += message;
        }
    }
}
