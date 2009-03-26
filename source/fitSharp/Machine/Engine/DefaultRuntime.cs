// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class DefaultRuntime<T>: RuntimeOperator<T> {
        public bool TryCreate(Processor<T> processor, string memberName, Tree<T> parameters, ref TypedValue result) {
            var runtimeType = processor.ParseString<RuntimeType>(memberName);
            if (parameters.Branches.Count == 0) {
                result = CreateWithoutParameters(runtimeType);
            }
            else {
                result = CreateWithParameters(processor, parameters, runtimeType);
            }
            return true;
        }

        private static TypedValue CreateWithoutParameters(RuntimeType runtimeType) {
            try {
                return runtimeType.CreateInstance();
            }
            catch (System.Exception e) {
                throw new CreateException(runtimeType.Type, 0, e);
            }
        }

        private static TypedValue CreateWithParameters(Processor<T> processor, Tree<T> parameters, RuntimeType runtimeType) {
            RuntimeMember member = runtimeType.GetConstructor(parameters.Branches.Count);
            object[] parameterList = GetParameterList(processor, TypedValue.Void, parameters, member);
            try {
                return member.Invoke(parameterList);
            }
            catch (System.Exception e) {
                throw new CreateException(runtimeType.Type, parameterList.Length, e);
            }
        }

        public bool TryInvoke(Processor<T> processor, TypedValue instance, string memberName, Tree<T> parameters,
                              ref TypedValue result) {
            RuntimeMember member = RuntimeType.FindInstance(instance.Value, memberName, parameters.Branches.Count);
            result = member != null
                         ? member.Invoke(GetParameterList(processor, instance, parameters, member))
                         : TypedValue.MakeInvalid(new MemberMissingException(instance.Type, memberName,
                                                                             parameters.Branches.Count));
            return true;
        }

        private static object[] GetParameterList(Processor<T> processor, TypedValue instance, Tree<T> parameters, RuntimeMember member) {
            var parameterList = new List<object>();
            int i = 0;
            foreach (Tree<T> parameter in parameters.Branches) {
                TypedValue parameterValue;
                try {
                    parameterValue = processor.Parse(member.GetParameterType(i), instance, parameter);
                }
                catch (System.Exception e) {
                    throw new ParseException<T>(member.Name, member.GetParameterType(i), i+1, parameter.Value, e);
                }
                parameterList.Add(parameterValue.Value);
                i++;
            }
            return parameterList.ToArray();
        }
    }
}