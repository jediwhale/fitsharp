// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit
{
    [TestFixture] public class ComposeShowAsOperatorTest
    {
        [Test] public void ComposesObjectsThatItWraps() {
            var showAs = new ComposeShowAsOperator();
            Assert.IsTrue(showAs.CanCompose(new TypedValue(showAs)));
        }

        [Test] public void ComposesAsRaw() {
            var processor = new CellProcessorBase();
            processor.AddOperator(new TestCompose());
            var showAs = new ComposeShowAsOperator {Processor = processor};
            var subject = new ComposeShowAsOperator(new [] {CellAttribute.Raw}, "stuff");
            var result = showAs.Compose(new TypedValue(subject));
            Assert.IsTrue(result.Value.HasAttribute(CellAttribute.Raw));
        }

        class TestCompose: CellOperator, ComposeOperator<Cell> {
            public bool CanCompose(TypedValue instance) {
                return true;
            }

            public Tree<Cell> Compose(TypedValue instance) {
                var cell = new CellTreeLeaf("stuff");
                cell.Value.SetAttribute(CellAttribute.Add, string.Empty);
                return cell;
            }
        }
    }
}
