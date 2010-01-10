// Copyright © 2009,2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;

namespace fitSharp.Slim.Operators {
    public class ExecuteMake: ExecuteBase {

        public ExecuteMake(): base("make") {}

        protected override Tree<string> ExecuteOperation(Tree<string> parameters) {
            TypedValue newInstance = Processor.Create(parameters.Branches[3].Value, ParameterTree(parameters, 4));
            string name = parameters.Branches[2].Value; 
            Processor.Store(new SavedInstance(name, newInstance.Value));
            if (name.StartsWith("library")) {
                Processor.PushLibraryInstance(newInstance);
            }
            return DefaultResult(parameters);
        }
    }
}