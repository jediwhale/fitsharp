// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.Linq;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class InvokeDefault<T,P>: Operator<T, P>, InvokeOperator<T> where P: class, Processor<T> {
        const string namedParameterPrefix = "!";

        public bool CanInvoke(TypedValue instance, string memberName, Tree<T> parameters) {
            return true;
        }

        public TypedValue Invoke(TypedValue instance, string memberName, Tree<T> parameters) {
            RuntimeMember member;
            var parameterNames = new List<string>();
            int parameterCount;

            if (memberName.StartsWith(namedParameterPrefix)) {
                parameterNames = parameters.Branches.Alternate().Aggregate(new List<string>(),
                                                          (names, parameter) => {
                                                              names.Add(Processor.ParseTree<T, string>(parameter));
                                                              return names;
                                                          });
                parameterCount = parameterNames.Count;
                member = RuntimeType.FindInstance(instance.Value, new IdentifierName(memberName.Substring(namedParameterPrefix.Length)), parameterNames);
            }
            else {
                parameterCount = parameters.Branches.Count;
                member = RuntimeType.FindInstance(instance.Value, new IdentifierName(memberName), parameterCount);
            }

            if (member == null)
                return TypedValue.MakeInvalid(new MemberMissingException(instance.Type, new IdentifierName(memberName).SourceName, parameterCount));

            var parameterList = memberName.StartsWith(namedParameterPrefix)
                ? new ParameterList<T>(Processor).GetNamedParameterList(instance, parameters, member, parameterNames)
                : new ParameterList<T>(Processor).GetParameterList(instance, parameters, member);
            return member.Invoke(parameterList);
        }
    }
}
