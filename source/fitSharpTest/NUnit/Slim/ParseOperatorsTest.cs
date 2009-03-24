// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Operators;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class ParseOperatorsTest {
        private Processor<string> processor;

        [SetUp] public void SetUp() {
            processor = new Processor<string>(new ApplicationUnderTest());
            processor.AddMemory<Symbol>();
        }

        [Test] public void ParseSymbolReplacesWithValue() {
            processor.Store(new Symbol("symbol", "testvalue"));
            Assert.AreEqual("testvalue", Parse(new ParseSymbol(), typeof(object), new TreeLeaf<string>("$symbol")));
        }

        [Test] public void ParseSymbolReplacesEmbeddedValues() {
            processor.Store(new Symbol("symbol1", "test"));
            processor.Store(new Symbol("symbol2", "value"));
            Assert.AreEqual("-testvalue-", Parse(new ParseSymbol(), typeof(object), new TreeLeaf<string>("-$symbol1$symbol2-")));
        }

        [Test] public void TreeIsParsedForList() {
            var list =
                Parse(new ParseList(), typeof (List<int>), new TreeList<string>().AddBranchValue("5").AddBranchValue("4")) as List<int>;
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(5, list[0]);
            Assert.AreEqual(4, list[1]);
        }

        private object Parse(ParseOperator<string> parseOperator, Type type, Tree<string> parameters) {
            TypedValue result = TypedValue.Void;
            Assert.IsTrue(parseOperator.TryParse(processor, type, TypedValue.Void, parameters, ref result));
            return result.Value;
        }
    }
}