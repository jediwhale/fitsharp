// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class InvokeDefault<T,P>: Operator<T, P>, InvokeOperator<T> where P: class, Processor<T> {
        public bool CanInvoke(TypedValue instance, MemberName memberName, Tree<T> parameters) {
            return true;
        }

        public TypedValue Invoke(TypedValue instance, MemberName memberName, Tree<T> parameters) {
            RuntimeMember member;
            var parameterNames = new List<string>();
            int parameterCount;

            if (memberName.HasNamedParameters) {
                parameterNames = parameters.Branches.Alternate().Aggregate(new List<string>(),
                                                          (names, parameter) => {
                                                              names.Add(Processor.ParseTree<T, string>(parameter));
                                                              return names;
                                                          });
                parameterCount = parameterNames.Count;
                member = MemberQuery.FindInstance(Processor.FindMember, instance.Value, memberName, parameterNames);
            }
            else {
                parameterCount = parameters.Branches.Count;
                member = MemberQuery.FindInstance(Processor.FindMember, instance.Value, memberName, parameterCount);
            }

            if (member == null)
                return TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberName.Name, parameterCount));

            var parameterList = memberName.HasNamedParameters
                ? new ParameterList<T>(Processor).GetNamedParameterList(instance, parameters, member, parameterNames)
                : new ParameterList<T>(Processor).GetParameterList(instance, parameters, member);
            try {
                return member.Invoke(parameterList);
            }
            catch (TargetInvocationException e) {
                return TypedValue.MakeInvalid(e.InnerException);
            }
            catch (ValidationException e) {
                return TypedValue.MakeInvalid(e);
            }
        }
    }
}
