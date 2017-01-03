// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Parser;

namespace fitSharp.Fit.Runner {
    public class HtmlDecorator {
        public static string AddToStart(string newText, string existingText) {
            var scanner = new Scanner(existingText);
            scanner.FindTokenPair("<!doctype", ">");
            return (scanner.Body.IsEmpty)
                ? newText + Environment.NewLine + existingText
                : scanner.Leader.ToString() + scanner.Element + Environment.NewLine + newText + scanner.Element.After;
        }
    }
}
