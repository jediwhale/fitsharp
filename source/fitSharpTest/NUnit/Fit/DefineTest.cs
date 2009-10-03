// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Fixtures;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;
using Moq;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class DefineTest {

        [Test] public void ProcedureIsSaved() {
            var processor = new Mock<CellProcessor>();
            var define = new Define {Processor = processor.Object};
            var input = new CellTree(new CellTree("define", "myprocedure"), new CellTree("stuff"));
            define.Interpret(input);
            processor.Verify(p => p.Store(It.Is<Procedure>(v => v.Id == "myprocedure" && v.Instance == input)));
        }
    }
}
