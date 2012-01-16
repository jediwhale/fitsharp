// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Model;
using Moq;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class SetUpFixtureTest {
        [Test] public void MethodIsInvokedForEachRow() {
            Parse table =
                Parse.ParseFrom("<table><tr><td></td></tr><tr><td>method</td></tr><tr><td>value</td></tr></table>");
            var processor = new Mock<CellProcessor>();
            var setUp = new SetUpFixture {Processor = processor.Object};
            setUp.DoTable(table);
            processor.Verify(o => o.Operate<ExecuteOperator>(setUp,
                It.Is<Tree<Cell>>(c => c.ValueAt(0).Text == "method"),
                It.Is<Tree<Cell>>(c => c.ValueAt(0).Text == "value"),
                It.Is<Cell>(c => c.Text == "value")));
        }
    }
}
