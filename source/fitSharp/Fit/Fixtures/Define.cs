// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Linq;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Fixtures {
    public class Define: Interpreter {
        public void Interpret(CellProcessor processor, Tree<Cell> table) {
            processor.TestStatus.TableCount--;
            var name = processor.ParseTree<Cell, MemberName>(new EnumeratedTree<Cell>(table.Branches[0].Branches.Skip(1).Alternate()));
            processor.Get<Procedures>().Save(name.ToString(), table);
        }
    }
}
