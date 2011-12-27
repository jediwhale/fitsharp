// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Fixtures;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;
using fitSharp.Test.Double.Fit;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class DefineTest {
        [Test] public void ProcedureIsSaved() {
            TestDefine(new CellTree("define", "myprocedure"));
        }

        [Test] public void MultiCellNameIsUsed() {
            TestDefine(new CellTree("define", "my", "p1", "procedure", "p2"));
        }

        private static void TestDefine(Tree<Cell> defineRow) {
            var processor = Builder.CellProcessor();
            var define = new Define();
            var input = new CellTree(defineRow, new CellTree("stuff"));
            define.Interpret(processor, input);
            Assert.AreEqual(input, processor.Get<Procedures>().GetValue("myprocedure"));
        }

    }
}
