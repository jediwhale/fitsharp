// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Slim.Operators {
    public class InvokeLibrary: SlimOperator, InvokeOperator<string> {
        public InvokeLibrary() {
            makeOperator = MakeDefault;
        }
        
        public InvokeLibrary(SlimProcessor processor, Func<SlimProcessor, InvokeOperator<string>> makeOperator) {
            Processor = processor;
            this.makeOperator = makeOperator;
        }
        
        public bool CanInvoke(TypedValue instance, MemberName memberName, Tree<string> parameters) { return true; }

        public TypedValue Invoke(TypedValue instance, MemberName memberName, Tree<string> parameters) {
            var invokeOperator = makeOperator(Processor);
            var result = invokeOperator.Invoke(instance, memberName, parameters);
            if (!IsMemberMissing(result)) return result;

            foreach (var libraryInstance in Processor.LibraryInstances) {
                var libraryResult = invokeOperator.Invoke(libraryInstance, memberName, parameters);
                if (!IsMemberMissing(libraryResult)) return libraryResult;
            }
            return result;
        }

        static bool IsMemberMissing(TypedValue result) {
            if (result.IsValid) return false;
            return result.GetValue<System.Exception>() is MemberMissingException;
        }

        static InvokeOperator<string> MakeDefault(SlimProcessor processor) {
            return new InvokeDefault<string, Processor<string>> {Processor = processor};
        }

        readonly Func<SlimProcessor, InvokeOperator<string>> makeOperator;
    }
}
