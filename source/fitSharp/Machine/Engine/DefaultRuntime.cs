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
            var memberIdentifier = new IdentifierName(memberName);
            RuntimeMember member = RuntimeType.FindInstance(instance.Value, memberIdentifier, parameters.Branches.Count);
            return member != null
                       ? member.Invoke(GetParameterList(instance, parameters, member))
                       : TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberIdentifier.SourceName,
                                                                           parameters.Branches.Count));
        }

        object[] GetParameterList(TypedValue instance, Tree<T> parameters, RuntimeMember member) {
            return parameters.Branches.Aggregate(new List<object>(), (parameterList, parameter) => {
                TypedValue parameterValue;
                int i = parameterList.Count;
                try {
                    parameterValue = Processor.Parse(member.GetParameterType(i), instance, parameter);
                }
                catch (System.Exception e) {
                    throw new ParseException<T>(member.Name, member.GetParameterType(i), i+1, parameter.Value, e);
                }
                parameterList.Add(parameterValue.Value);
                return parameterList;
            }).ToArray();
        }
    }
}