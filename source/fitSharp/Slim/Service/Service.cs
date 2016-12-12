// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Operators;

namespace fitSharp.Slim.Service {
    public class Service: ProcessorBase<string, SlimProcessor>, SlimProcessor {
        private static readonly Regex singleSymbolPattern = new Regex("^\\$([a-zA-Z]\\w*)$");

        private readonly SlimOperators operators;
        private readonly Stack<TypedValue> libraryInstances = new Stack<TypedValue>();

        public Service(Memory memory) : base(memory) {
            operators = memory.GetItem<SlimOperators>();
            operators.Processor = this;

            Memory.GetItem<SavedInstances>();
            Memory.GetItem<Symbols>();

            PushLibraryInstance(new TypedValue(new Actors(this)));
            PushLibraryInstance(new TypedValue(new SlimFunctions(this)));
        }

        public void PushLibraryInstance(TypedValue instance) {
            libraryInstances.Push(instance);
        }

        public IEnumerable<TypedValue> LibraryInstances { get { return libraryInstances; } }

        public TypedValue LoadSymbol(string input) {
            return singleSymbolPattern.IsMatch(input)
                ? new TypedValue(this.Get<Symbols>().GetValue(input.Substring(1)))
                : TypedValue.Void;
        }

        protected override Operators<string, SlimProcessor> Operators {
            get { return operators; }
        }

        private class Actors {
            private const string actorInstanceName = "scriptTableActor";

            private readonly Stack<object> actors = new Stack<object>();
            private readonly SlimProcessor processor;
 
            public Actors(SlimProcessor processor) {
                this.processor = processor;
            }

            public object GetFixture() {
                return processor.Get<SavedInstances>().GetValue(actorInstanceName);
            }

            public void PushFixture() {
                actors.Push(GetFixture());
            }

            public void PopFixture() {
                processor.Get<SavedInstances>().Save(actorInstanceName, actors.Pop());
            }
        }

        class SlimFunctions {
            public SlimFunctions(SlimProcessor processor) {
                this.processor = processor;
            }

            public void _Configure(string feature, string item, string value) {
                processor.Memory.GetItem<Trace>().File(value);
            }

            readonly SlimProcessor processor;
        }
    }
}