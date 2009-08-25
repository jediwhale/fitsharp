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
        public bool CanCreate(Processor<Cell> processor, string memberName, Tree<Cell> parameters) {
            return false;
        }

        public TypedValue Create(Processor<Cell> processor, string memberName, Tree<Cell> parameters) {
            return TypedValue.Void;
        }

        public bool CanInvoke(Processor<Cell> processor, TypedValue instance, string memberName, Tree<Cell> parameters) {
            return parameters.IsLeaf;
        }

        public TypedValue Invoke(Processor<Cell> processor, TypedValue instance, string memberName, Tree<Cell> parameters) {
            if (memberName == "setup" || memberName == "teardown") {
                RuntimeMember member = RuntimeType.FindDirectInstance(instance.Value, memberName, 0);
                return member != null
                             ? member.Invoke(new object[] {})
                             : TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberName, 0));
            }
            else {
                RuntimeMember member = RuntimeType.FindDirectInstance(instance.Value, memberName, new[] {typeof (Parse)});
                return member != null
                             ? member.Invoke(new object[] {parameters.Value})
                             : TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberName, 1));
            }
        }
    }
}
