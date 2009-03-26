// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ParseMemberName: ParseOperator<Cell> {
        public bool TryParse(Processor<Cell> processor, Type type, TypedValue instance, Tree<Cell> parameters, ref TypedValue result) {
            if (type != typeof(MemberName)) return false;
            var nameParts = new StringBuilder();
            foreach (Cell namePart in parameters.Leaves) {
                nameParts.Append(namePart.Text);
            }
            result = new TypedValue(new MemberName(new GracefulName(nameParts.ToString()).IdentifierName.ToString()));

            return true;
        }
    }
}