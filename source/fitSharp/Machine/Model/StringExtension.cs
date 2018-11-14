// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (https://opensource.org/licenses/cpl1.0.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fitSharp.Machine.Model {
    public static class StringExtension {

        public static string Before(this string source, string value) {
            return source.Substring(0, source.LengthBefore(value));
        }

        public static string Before(this string source, IEnumerable<string> values) {
            return source.Substring(0, values.Select(source.LengthBefore).Min());
        }

        public static int LengthBefore(this string source, string value) {
            var index = source.IndexOf(value, StringComparison.OrdinalIgnoreCase);
            return index < 0 ? source.Length : index;
        }

        public static string AsPath(this string source) {
            return source.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
