// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Model {
    public class GracefulName {

        public GracefulName(string theOriginalName) {
            myOriginalName = theOriginalName;
        }

        public IdentifierName IdentifierName {
            get {
                var result = new StringBuilder();
                foreach (char original in myOriginalName) {
                    if (char.IsLetter(original) || char.IsDigit(original)) {
                        result.Append(original);
                    }
                }
                return new IdentifierName(result.ToString());
            }
        }

        private readonly string myOriginalName;
    }
}