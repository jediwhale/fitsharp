// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Model;
using Moq;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class ConstraintFixtureTest {
        [Test] public void MethodIsInvokedForEachRow() {
            Parse table =
                HtmlParser.Instance.Parse("<table><tr><td></td></tr><tr><td>method</td></tr><tr><td>value</td></tr></table>");
            var cellOperation = new Mock<CellOperation>();
            var processor = new Mock<CellProcessor>();
            var constraint = new ConstraintFixture {CellOperation = cellOperation.Object, Processor = processor.Object};
            processor.Setup(p => p.TestStatus).Returns(new TestStatus());
            cellOperation.Setup(o => o.TryInvoke(constraint,
                It.Is<Tree<Cell>>(c => c.Branches[0].Value.Text == "method"),
                It.Is<Tree<Cell>>(c => c.Branches[0].Value.Text == "value"),
                It.Is<Tree<Cell>>(c => c.Value.Text == "value")))
                .Returns(new TypedValue(true));
            constraint.DoTable(table);
        }
    }
}
