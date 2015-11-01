// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Slim.Operators {
    public class ExecuteMake: InvokeInstructionBase {

        public ExecuteMake(): base("make") {}

        protected override Tree<string> ExecuteOperation(Tree<string> parameters) {
            var singleSymbol = Processor.LoadSymbol(parameters.ValueAt(3));
            var newInstance = singleSymbol.IsObject && singleSymbol.Type != typeof(string)
                ? singleSymbol
                : CreateInstance(parameters);
            var name = parameters.ValueAt(2); 
            Processor.Get<SavedInstances>().Save(name, newInstance.Value);
            if (name.StartsWith("library")) {
                Processor.PushLibraryInstance(newInstance);
            }
            return DefaultResult(parameters);
        }

        TypedValue CreateInstance(Tree<string> parameters) {
            var typeName = Processor.ParseTree<string, string>(parameters.Branches[3]);
            return Processor.Create(typeName, ParameterTree(parameters, 4));
        }
    }
}
