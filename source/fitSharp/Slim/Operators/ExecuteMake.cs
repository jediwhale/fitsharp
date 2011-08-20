// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Slim.Operators {
    public class ExecuteMake: InvokeInstructionBase {

        public ExecuteMake(): base("make") {}

        protected override Tree<string> ExecuteOperation(Tree<string> parameters) {
            var singleSymbol = Processor.LoadSymbol(parameters.Branches[3].Value);
            var newInstance = singleSymbol.IsObject
                ? singleSymbol
                : Processor.Create(parameters.Branches[3].Value, ParameterTree(parameters, 4));
            string name = parameters.Branches[2].Value; 
            Processor.Get<SavedInstances>().Save(name, newInstance.Value);
            if (name.StartsWith("library")) {
                Processor.PushLibraryInstance(newInstance);
            }
            return DefaultResult(parameters);
        }
    }
}