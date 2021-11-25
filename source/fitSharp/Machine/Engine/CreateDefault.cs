// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class CreateDefault<T,P>: Operator<T, P>, CreateOperator<T> where P: class, Processor<T> {
        public bool CanCreate(NameMatcher memberName, Tree<T> parameters) {
            return true;
        }

        public TypedValue Create(NameMatcher memberName, Tree<T> parameters) {
            var runtimeType = new RuntimeType(Processor.ApplicationUnderTest.FindType(memberName));
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
            object[] parameterList = new ParameterList<T>(Processor).GetParameterList(TypedValue.Void, parameters, member);
            try {
                return member.Invoke(parameterList);
            }
            catch (System.Exception e) {
                throw new CreateException(runtimeType.Type, parameterList.Length, e.InnerException ?? e);
            }
        }
    }
}
