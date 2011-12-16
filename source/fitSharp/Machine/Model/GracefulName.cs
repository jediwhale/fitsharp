// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Linq;
using System.Text;

namespace fitSharp.Machine.Model {
    public class GracefulName {

        readonly string originalName;

        public GracefulName(string originalName) {
            this.originalName = originalName;
        }

        public IdentifierName IdentifierName {
            get {
                return new IdentifierName(
                    originalName.StartsWith("\"") && originalName.EndsWith("\"")
                           ? originalName.Substring(1, originalName.Length - 2)
                           : originalName.ToCharArray()
                                                    .Where(c => char.IsLetter(c) || char.IsDigit(c) || c == '_')
                                                    .Aggregate(new StringBuilder(), (s, c) => s.Append(c))
                                                    .ToString());
            }
        }

        public override string ToString() {
            return IdentifierName.ToString();
        }
    }
}
