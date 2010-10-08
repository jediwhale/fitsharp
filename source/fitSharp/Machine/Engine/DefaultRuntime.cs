// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.Linq;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class DefaultRuntime<T,P>: Operator<T, P>, RuntimeOperator<T> where P: class, Processor<T> {

        const string namedParameterPrefix = "!";

        public bool CanCreate(string memberName, Tree<T> parameters) {
            return true;
        }

        public TypedValue Create(string memberName, Tree<T> parameters) {
            var runtimeType = Processor.ParseString<T, RuntimeType>(memberName);
            return parameters.Branches.Count == 0
                         ? CreateWithoutParameters(runtimeType)
                         : CreateWithParameters(parameters, runtimeType);
        }

        static TypedValue CreateWithoutParameters(RuntimeType runtimeType) {
            try {
                return runtimeType.CreateInstance();
            }
            catch (System.Exception e) {
                throw new CreateException(runtimeType.Type, 0, e.InnerException ?? e);
            }
        }

        TypedValue CreateWithParameters(Tree<T> parameters, RuntimeType runtimeType) {
            RuntimeMember member = runtimeType.GetConstructor(parameters.Branches.Count);
            object[] parameterList = GetParameterList(TypedValue.Void, parameters, member);
            try {
                return member.Invoke(parameterList);
            }
            catch (System.Exception e) {
                throw new CreateException(runtimeType.Type, parameterList.Length, e.InnerException ?? e);
            }
        }

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

            object[] parameterList = memberName.StartsWith(namedParameterPrefix)
                ? GetNamedParameterList(instance, parameters, member, parameterNames)
                : GetParameterList(instance, parameters, member);
            return member.Invoke(parameterList);
        }

        object[] GetParameterList(TypedValue instance, Tree<T> parameters, RuntimeMember member) {
            return parameters.Branches.Aggregate(new List<object>(),
                (parameterList, parameter) => {
                     parameterList.Add(ParseParameterValue(member, instance, parameter, parameterList.Count));
                    return parameterList;
            }).ToArray();
        }


        object[] GetNamedParameterList(TypedValue instance, Tree<T> parameters, RuntimeMember member, IList<string> parameterNames) {
            var result = new object[parameterNames.Count];

            for (int i = 0; i < parameterNames.Count; i++) {
                for (int j = 0; j < parameterNames.Count; j++) {
                    var parameterNameId = new IdentifierName(parameterNames[j]);
                    if (!parameterNameId.Matches(member.GetParameterName(i))) continue;
                    result[i] = ParseParameterValue(member, instance, parameters.Branches[2*j + 1], i);
                }
            }
            return result;
        }

        object ParseParameterValue(RuntimeMember member, TypedValue instance, Tree<T> parameter, int parameterIndex) {
            TypedValue parameterValue;
            try {
                parameterValue = Processor.Parse(member.GetParameterType(parameterIndex), instance, parameter);
            }
            catch (System.Exception e) {
                throw new ParseException<T>(member.Name, member.GetParameterType(parameterIndex), parameterIndex+1, parameter.Value, e);
            }
            return parameterValue.Value;
        }
    }
}
