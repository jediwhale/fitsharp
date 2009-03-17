// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

namespace fitSharp.Machine.Model {
    public class LanguageName: NameMatcher {

        public string MatchName { get; private set; }
        
        public LanguageName(string name) {
            MatchName = name.Trim();
        }

        public bool Matches(string candidateName) {
            return candidateName == MatchName;
        }

        public override bool Equals(object otherObject) {
            return Equals(otherObject as LanguageName);
        }

        public bool Equals(LanguageName other) {
            return other != null && Matches(other.MatchName);
        }

        public override string ToString() { return MatchName; }
        public override int GetHashCode() { return MatchName.GetHashCode(); }
    }
}