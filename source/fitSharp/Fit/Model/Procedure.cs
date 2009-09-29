// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Model {
    public class Procedure: KeyValueMemory<string, Tree<Cell>> {
        public Procedure(string id): base(id) {}
        public Procedure(string id, Tree<Cell> instance): base(id, instance) {}

        public TypedValue Invoke(CellProcessor processor, TypedValue target) {
            var doFixture = new CellTree(new CellTree("dofixture"));
            var fixture = processor.Parse(typeof (Interpreter), target, doFixture);
            TypedValue result = processor.Execute(fixture, Instance);

            processor.TestStatus.LastAction = processor.Parse(typeof(StoryTestString), TypedValue.Void, Instance).ValueString;
            return result;
        }
    }
}