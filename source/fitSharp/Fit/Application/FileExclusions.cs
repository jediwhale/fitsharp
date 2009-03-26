// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text.RegularExpressions;

namespace fitSharp.Fit.Application {
    public class FileExclusions: ConfigurationList<string> {
        public override string Parse(string theValue) { return theValue; }

        public override ConfigurationList<string> Make() { return new FileExclusions(); }

        public bool IsExcluded(string theName) {
            foreach (string pattern in myList) {
                if (Regex.IsMatch(theName, pattern)) return true;
            }
            return false;
        }
    }
}