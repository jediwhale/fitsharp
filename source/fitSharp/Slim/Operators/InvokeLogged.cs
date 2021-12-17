// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Slim.Operators {
    public class InvokeLogged: SlimOperator, InvokeOperator<string> {
        public bool CanInvoke(TypedValue instance, MemberName memberName, Tree<string> parameters) { return true; }

        public TypedValue Invoke(TypedValue instance, MemberName memberName, Tree<string> parameters) {
            var invokeOperator = new InvokeLibrary(Processor, MakeLogged);
            return invokeOperator.Invoke(instance, memberName, parameters);
        }

        static InvokeOperator<string> MakeLogged(SlimProcessor processor) {
            return new InvokeDefaultLogged<string, Processor<string>> {Processor = processor};
        }
    }
}
