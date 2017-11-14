// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class InvokeSetUpTearDown: CellOperator, InvokeOperator<Cell> {

        public bool CanInvoke(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            return memberName == MemberName.SetUp || memberName == MemberName.TearDown;
        }

        public TypedValue Invoke(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            var member = MemberQuery.FindDirectInstance(instance.Value, new MemberSpecification(memberName));
            return member
                .Select(v => v.Invoke(new object[] {}))
                .OrDefault(TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberName.Name, 0)));
        }
    }
}
