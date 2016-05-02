// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Samples.Fit;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture]
    public class InvokeFlowKeywordTest {

        [SetUp] public void SetUp() {
            processor = Builder.CellProcessor();
            keywords = new InvokeFlowKeyword {Processor = processor};
            interpreter = new TypedValue(new DefaultFlowInterpreter(new TestDomain()));
        }

        [Test] public void ChecksExpectedValue() {
            var row = new CellTree("check", "stuff", "some stuff");
            Invoke(row);
            Assert.AreEqual(fitSharp.Fit.Model.TestStatus.Right, row.Last().Value.GetAttribute(CellAttribute.Status));
        }

        [Test] public void CheckFieldsForWrapsResultInList() {
            var row = new CellTree("check fields for", "stuff");
            var result = Invoke(row).GetValue<List<object>>();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("some stuff", result[0]);
        }

        [Test] public void NameKeywordAssignsASymbol() {
            var row = new CellTree("name", "symbol", "stuff");
            Invoke(row);
            Assert.AreEqual("some stuff", processor.Get<Symbols>().GetValue("symbol"));
        }

        [Test] public void ShowAsKeywordComposesWithAttributes() {
            var row = new CellTree("show as", "raw", "stuff");
            Invoke(row);
            Assert.IsTrue(row.Last().Value.HasAttribute(CellAttribute.Raw));
        }

        [Test] public void WaitUntilRepeatsExpectedValueCheck() {
            var row = new CellTree("wait until", "next count", "2");
            Invoke(row);
            Assert.AreEqual(fitSharp.Fit.Model.TestStatus.Right, row.Last().Value.GetAttribute(CellAttribute.Status));
        }

        [Test] public void WaitUntilRepeatsUpToLimit() {
            processor.Get<Symbols>().Save("WaitFor.Count", 10);
            processor.Get<Symbols>().Save("WaitFor.Time", 1);
            var row = new CellTree("wait until", "next count", "101");
            Invoke(row);
            Assert.AreEqual(fitSharp.Fit.Model.TestStatus.Wrong, row.Last().Value.GetAttribute(CellAttribute.Status));
        }

        TypedValue Invoke(CellTree row) {
            var keyword = row.Branches[0].Value.Text;
            return keywords.InvokeSpecial(interpreter, new MemberName(keyword), row);
        }

        InvokeFlowKeyword keywords;
        TypedValue interpreter;
        CellProcessorBase processor;

        class TestDomain {
            public string Stuff = "some stuff";
            public int NextCount() {
                return count++;
            }
            int count;
        }
    }
}
