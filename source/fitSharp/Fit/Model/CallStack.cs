// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Model {
    public class CallStack {
        readonly Stack<CallFrame> frames = new Stack<CallFrame>();

        public CallStack() { Push();}

        public TypedValue PopReturn() { return frames.Pop().ReturnValue; }
        public void Push() { frames.Push(new CallFrame()); }

        public void SetReturn(TypedValue value) {
            frames.Peek().ReturnValue = value;
        }

        public TypedValue DomainAdapter {
            get { return frames.Peek().DomainAdapter; }
            set { frames.Peek().DomainAdapter = value; }
        }

        public T GetSystemUnderTest<T>() where T: class {
            var candidate = DomainAdapter;
            while (candidate.HasValue) {
                var systemUnderTest = candidate.GetValueAs<T>();
                if (systemUnderTest != null) return systemUnderTest;
                candidate = new TypedValue(candidate.As<DomainAdapter, object>(
                    domainAdapter => domainAdapter.SystemUnderTest,
                    () => null));
            }
            return null;
        }

        class CallFrame {
            public TypedValue ReturnValue;
            public TypedValue DomainAdapter;

            public CallFrame() {
                ReturnValue = TypedValue.Void;
                DomainAdapter = TypedValue.Void;
            }
        }
    }
}
