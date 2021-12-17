// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.IO;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class InvokeDefaultLogged<T,P>: Operator<T, P>, InvokeOperator<T> where P: class, Processor<T> {
        public bool CanInvoke(TypedValue instance, MemberName memberName, Tree<T> parameters) {
            return true;
        }

        public TypedValue Invoke(TypedValue instance, MemberName memberName, Tree<T> parameters) {
            return new InvokeDefault<T, P>(Processor, LogInvoke).Invoke(instance, memberName, parameters);
        }

        static void LogInvoke(RuntimeMember member) {
            File.AppendAllLines("invoke.log", new [] {member.Name});
        }
    }
}
