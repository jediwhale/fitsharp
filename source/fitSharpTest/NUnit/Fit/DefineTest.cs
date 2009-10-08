// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Fixtures;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;
using Moq;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class DefineTest {
        [Test] public void ProcedureIsSaved() {
            TestDefine(IsSinglePartName, new CellTree("define", "myprocedure"));
        }

        private static bool IsSinglePartName(Tree<Cell> c) {
            return c.Branches.Count == 1 && c.Branches[0].Value.Text == "myprocedure";
        }

        [Test] public void MultiCellNameIsUsed() {
            TestDefine(IsMultiPartName, new CellTree("define", "my", "p1", "procedure", "p2"));
        }

        private static bool IsMultiPartName(Tree<Cell> c) {
            return c.Branches.Count == 2 && c.Branches[0].Value.Text == "my" && c.Branches[1].Value.Text == "procedure";
        }

        private static void TestDefine(Func<Tree<Cell>, bool> isName, Tree<Cell> defineRow) {
            var processor = new Mock<CellProcessor>();
            processor
                .Setup(p => p.Parse(typeof (MemberName), TypedValue.Void, It.Is<Tree<Cell>>(c => isName(c))))
                .Returns(new TypedValue(new MemberName("myprocedure")));
            var define = new Define {Processor = processor.Object};
            var input = new CellTree(defineRow, new CellTree("stuff"));
            define.Interpret(input);
            processor.Verify(p => p.Store(It.Is<Procedure>(v => v.Id == "myprocedure" && v.Instance == input)));
        }

    }
}
