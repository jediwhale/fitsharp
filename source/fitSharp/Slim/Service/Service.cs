// Copyright © 2011 Syterra Software Inc. All rights reserved.
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

        public Service(): this(new Configuration()) {}

        public Service(Configuration configuration) : base(configuration) {
            operators = configuration.GetItem<SlimOperators>();
            operators.Processor = this;

            AddMemory<SavedInstance>();
            AddMemory<Symbol>();

            PushLibraryInstance(new TypedValue(new Actors(this)));
        }

        public void PushLibraryInstance(TypedValue instance) {
            libraryInstances.Push(instance);
        }

        public IEnumerable<TypedValue> LibraryInstances { get { return libraryInstances; } }

        public Symbol LoadSymbol(string input) {
            return singleSymbolPattern.IsMatch(input) ? Load(new Symbol(input.Substring(1))) : null;
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
                return processor.Load(new SavedInstance(actorInstanceName)).Instance;
            }

            public void PushFixture() {
                actors.Push(GetFixture());
            }

            public void PopFixture() {
                processor.Store(new SavedInstance(actorInstanceName, actors.Pop()));
            }
        }
    }
}