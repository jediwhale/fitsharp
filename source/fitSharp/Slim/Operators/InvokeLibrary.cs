// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Slim.Operators {
    public class InvokeLibrary: SlimOperator, InvokeOperator<string> {
        public bool CanInvoke(TypedValue instance, MemberName memberName, Tree<string> parameters) { return true; }

        public TypedValue Invoke(TypedValue instance, MemberName memberName, Tree<string> parameters) {
            var runtime = new InvokeDefault<string, Processor<string>> {Processor = Processor};
            var result = runtime.Invoke(instance, memberName, parameters);
            if (!IsMemberMissing(result)) return result;

            foreach (var libraryInstance in Processor.LibraryInstances) {
                var libraryResult = runtime.Invoke(libraryInstance, memberName, parameters);
                if (!IsMemberMissing(libraryResult)) return libraryResult;
            }
            return result;
        }

        private static bool IsMemberMissing(TypedValue result) {
            if (result.IsValid) return false;
            var exception = result.GetValue<System.Exception>();
            return exception != null  && exception is MemberMissingException;
        }
    }
}
