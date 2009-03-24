// FitNesse.NET
// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fit.Operators {
    public class RuntimeFlow: RuntimeOperator<Cell> {
        public bool TryCreate(Processor<Cell> processor, string memberName, Tree<Cell> parameters, ref TypedValue result) {
            return false;
        }

        public bool TryInvoke(Processor<Cell> processor, TypedValue instance, string memberName, Tree<Cell> parameters, ref TypedValue result) {
            if (!parameters.IsLeaf) return false;

            if (memberName == "setup" || memberName == "teardown") {
                RuntimeMember member = RuntimeType.FindDirectInstance(instance.Value, memberName, 0);
                result = member != null
                             ? member.Invoke(new object[] {})
                             : TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberName, 0));
            }
            else {
                RuntimeMember member = RuntimeType.FindDirectInstance(instance.Value, memberName, new[] {typeof (Parse)});
                result = member != null
                             ? member.Invoke(new object[] {parameters.Value})
                             : TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberName, 1));
            }
            return true;
        }
    }
}
