// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Linq;

namespace fitSharp.Machine.Model {
    public class GracefulNameMatcher: NameMatcher {
        public GracefulNameMatcher(params string[] originalNames) {
            matchers = originalNames.Select(originalName =>
                originalName.Contains(".")
                    ? new IdentifierName(originalName)
                    : new GracefulName(originalName).IdentifierName).ToArray();
        }

        public bool Matches(string candidateName) {
            return matchers.Any(matcher => matcher.Matches(candidateName));
        }

        public string MatchName {
            get { return matchers[0].MatchName; }
        }

        readonly IdentifierName[] matchers;
    }
}
