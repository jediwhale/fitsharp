// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;

namespace fitSharp.Machine.Model {
    public class IdentifierName: NameMatcher {

        public string MatchName { get; private set; }
        public string SourceName { get; private set; }

        public IdentifierName(string name) {
            SourceName = name;
            MatchName = name.Trim();
        }

        public bool Matches(string name) {
            return name.Contains("_")
                ? string.Equals(MatchName.Replace("_", string.Empty), name.Replace("_", string.Empty), StringComparison.OrdinalIgnoreCase)
                : string.Equals(MatchName, name, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsStartOf(string other) {
            return other.StartsWith(MatchName, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsEmpty { get { return MatchName.Length == 0; } }

        public int Length { get { return MatchName.Length; } }

        public override bool Equals(object other) {
            return other != null && Matches(other.ToString());
        }

        public bool Equals(IdentifierName other) {
            return other != null && Matches(other.MatchName);
        }

        public override string ToString() { return MatchName.ToLower(); }
        public override int GetHashCode() { return ToString().GetHashCode(); }

    }
}