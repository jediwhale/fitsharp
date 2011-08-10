// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Fixtures {
    public class ConfigureFixture: Interpreter {
        public void Interpret(CellProcessor processor, Tree<Cell> table) {
            processor.TestStatus.TableCount--;
            TypedValue result = processor.Invoke(
                new TypedValue(processor.Configuration.GetItem(table.Branches[0].Branches[1].Value.Text)),
                table.Branches[0].Branches[2].Value.Text,
                new CellTree());
            result.ThrowExceptionIfNotValid();
            if (result.IsVoid) return;
            table.Branches[0].Branches[2].Value.SetAttribute(CellAttribute.Folded, result.ValueString);
        }
    }
}
