// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;
using fitSharp.Slim.Operators;
using fitSharp.Slim.Service;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Slim {
    [TestFixture] public class ParseOperatorsTest {
        Service processor;

        [SetUp] public void SetUp() {
            processor = new Service();
        }

        [Test] public void ParseSymbolReplacesWithValue() {
            processor.Get<Symbols>().Save("symbol", "testvalue");
            Assert.AreEqual("testvalue", Parse(new ParseSymbol { Processor = processor }, typeof(object), new SlimLeaf("$symbol")));
        }

        [Test] public void ParseSymbolReplacesEmbeddedValues() {
            processor.Get<Symbols>().Save("symbol1", "test");
            processor.Get<Symbols>().Save("symbol2", "value");
            Assert.AreEqual("-testvalue-", Parse(new ParseSymbol { Processor = processor }, typeof(object), new SlimLeaf("-$symbol1$symbol2-")));
        }

        [Test] public void ParseSymbolIgnoresUndefinedSymbols() {
            Assert.AreEqual("$symbol", processor.Parse(typeof(object), TypedValue.Void, new SlimLeaf("$symbol")).ValueString);
        }

        [Test] public void ParseSymbolIgnoresEmbeddedUndefinedSymbols() {
            Assert.AreEqual("-$symbol-", processor.Parse(typeof(object), TypedValue.Void, new SlimLeaf("-$symbol-")).ValueString);
        }

        [Test] public void ParseSymbolWithDoubleDollar() {
            processor.Get<Symbols>().Save("symbol", "testvalue");
            Assert.AreEqual("$testvalue", processor.Parse(typeof(object), TypedValue.Void, new SlimLeaf("$$symbol")).ValueString);
        }

        [Test] public void ParseSymbolEmbeddedWithDoubleDollar() {
            processor.Get<Symbols>().Save("symbol", "testvalue");
            Assert.AreEqual("-$testvaluetestvalue-", processor.Parse(typeof(object), TypedValue.Void, new SlimLeaf("-$$symbol$symbol-")).ValueString);
        }

        [Test] public void ParseSymbolMatchingRequestedType() {
            processor.Get<Symbols>().Save("symbol", AppDomain.CurrentDomain);
            Assert.AreEqual(AppDomain.CurrentDomain, processor.Parse(typeof(AppDomain), TypedValue.Void, new SlimLeaf("$symbol")).Value);
        }

        [Test] public void TreeIsParsedForList() {
            var list =
                Parse(new ParseList{ Processor = processor }, typeof (List<int>), new SlimTree().AddBranchValue("5").AddBranchValue("4")) as List<int>;
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(5, list[0]);
            Assert.AreEqual(4, list[1]);
        }

        [Test] public void TreeIsParsedForArray() {
            var list =
                Parse(new ParseList{ Processor = processor }, typeof (int[]), new SlimTree().AddBranchValue("5").AddBranchValue("4")) as int[];
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(5, list[0]);
            Assert.AreEqual(4, list[1]);
        }

        [Test] public void ParsesEnumType() {
            Assert.AreEqual(BindingFlags.Public,
                            processor.Parse(typeof (BindingFlags), TypedValue.Void,
                                                       new SlimLeaf("Public")).Value);
        }

        [Test] public void ParsesIntegerForNullableInt() {
            Assert.AreEqual(1, processor.Parse(typeof (int?), TypedValue.Void, new SlimLeaf("1")).Value);
        }

        [Test] public void ParsesDictionary() {
            var dictionary = processor.Parse(typeof (Dictionary<string, string>), TypedValue.Void,
                    new SlimLeaf("<table><tr><td>key</td><td>value</td></tr></table>")).GetValue<Dictionary<string,string>>();
            Assert.AreEqual("value", dictionary["key"]);
        }

        [Test] public void ParsesWithInvariantCulture() {
            CultureInfo current = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES", false);
            Assert.AreEqual(1.001, processor.Parse(typeof (double), TypedValue.Void, new SlimLeaf("1.001")).Value);
            Thread.CurrentThread.CurrentCulture = current;
        }

        static object Parse(ParseOperator<string> parseOperator, Type type, Tree<string> parameters) {
            Assert.IsTrue(parseOperator.CanParse(type, TypedValue.Void, parameters));
            TypedValue result = parseOperator.Parse(type, TypedValue.Void, parameters);
            return result.Value;
        }
    }
}
