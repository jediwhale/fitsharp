// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Operators;

namespace fitSharp.Slim.Service {
    public class SlimOperators: Operators<string, SlimProcessor>, Copyable {

        public SlimOperators() {
            Add(new ComposeDefault(), 0);
            Add(new ExecuteDefault(), 0);
            Add(new ParseDefault(), 0);
            Add(new InvokeLibrary(), 0);

            Add(new ExecuteImport(), 0);
            Add(new ExecuteMake(), 0);
            Add(new ExecuteCall(), 0);
            Add(new ExecuteCallAndAssign(), 0);
            Add(new ExecuteAssign(), 0);

            Add(new ParseType<string, SlimProcessor>(), 0);
            Add(new ParseList(), 0);
            Add(new ParseDictionary(), 0);
            Add(new ComposeDictionary(), 0);
            Add(new ParseSymbol(), 2);

            Add(new ComposeException(), 0);
            Add(new ComposeBoolean(), 0);
            Add(new ComposeList(), 0);
        }

        public SlimOperators(Operators<string, SlimProcessor> other): this() {
            Copy(other);
        }

        public Copyable Copy() {
            return new SlimOperators(this);
        }
    }
}
