// Copyright © 2013 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Reflection;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;
using fitSharp.Slim.Operators;
using fitSharp.Slim.Service;
using fitSharp.Test.Double.Slim;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Slim {

    public interface IObject {}
    public class ConcreteObject : IObject {
        public string Name { get; set; }
        public string Description { get; set; }

        public static IObject NewInstance() {
            return new ConcreteObject {Name = "testname", Description = "testdescription"};
        }
    }
    
    [TestFixture] public class ParseOperatorsTest {
        Service processor;

        [SetUp] public void SetUp() {
            processor = Builder.Service();
        }

        [Test]
        public void ParseSymbolReplacesWithValueAsImplementation() {
            var testvalue = ConcreteObject.NewInstance();
            processor.Get<Symbols>().Save("symbol", testvalue);
            ClassicAssert.AreEqual(testvalue, Parse(new ParseSymbol { Processor = processor }, typeof(ConcreteObject), new SlimLeaf("$symbol")));
        }

        [Test]
        public void ParseSymbolReplacesWithValueAsInterface() {
            var testvalue = ConcreteObject.NewInstance();
            processor.Get<Symbols>().Save("symbol", testvalue);
            ClassicAssert.AreEqual(testvalue, Parse(new ParseSymbol { Processor = processor }, typeof(IObject), new SlimLeaf("$symbol")));
        }

        [Test] public void ParseSymbolReplacesWithValue() {
            processor.Get<Symbols>().Save("symbol", "testvalue");
            ClassicAssert.AreEqual("testvalue", Parse(new ParseSymbol { Processor = processor }, typeof(object), new SlimLeaf("$symbol")));
        }

        [Test] public void ParseSymbolReplacesEmbeddedValues() {
            processor.Get<Symbols>().Save("symbol1", "test");
            processor.Get<Symbols>().Save("symbol2", "value");
            ClassicAssert.AreEqual("-testvalue-", Parse(new ParseSymbol { Processor = processor }, typeof(object), new SlimLeaf("-$symbol1$symbol2-")));
        }

        [Test] public void ParseSymbolIgnoresUndefinedSymbols() {
            ClassicAssert.AreEqual("$symbol", processor.Parse(typeof(object), TypedValue.Void, new SlimLeaf("$symbol")).ValueString);
        }

        [Test] public void ParseSymbolIgnoresEmbeddedUndefinedSymbols() {
            ClassicAssert.AreEqual("-$symbol-", processor.Parse(typeof(object), TypedValue.Void, new SlimLeaf("-$symbol-")).ValueString);
        }

        [Test] public void ParseSymbolWithDoubleDollar() {
            processor.Get<Symbols>().Save("symbol", "testvalue");
            ClassicAssert.AreEqual("$testvalue", processor.Parse(typeof(object), TypedValue.Void, new SlimLeaf("$$symbol")).ValueString);
        }

        [Test] public void ParseSymbolEmbeddedWithDoubleDollar() {
            processor.Get<Symbols>().Save("symbol", "testvalue");
            ClassicAssert.AreEqual("-$testvaluetestvalue-", processor.Parse(typeof(object), TypedValue.Void, new SlimLeaf("-$$symbol$symbol-")).ValueString);
        }

        [Test] public void ParseSymbolMatchingRequestedType() {
            processor.Get<Symbols>().Save("symbol", AppDomain.CurrentDomain);
            ClassicAssert.AreEqual(AppDomain.CurrentDomain, processor.Parse(typeof(AppDomain), TypedValue.Void, new SlimLeaf("$symbol")).Value);
        }

        [Test] public void LeafIsParsedForList() {
            var list =
                Parse(new ParseList{ Processor = processor }, typeof (List<int>), new SlimLeaf("[5, 4]")) as List<int>;
            ClassicAssert.IsNotNull(list);
            ClassicAssert.AreEqual(2, list.Count);
            ClassicAssert.AreEqual(5, list[0]);
            ClassicAssert.AreEqual(4, list[1]);
        }

        [Test] public void TreeIsParsedForList() {
            var list =
                Parse(new ParseList{ Processor = processor }, typeof (List<int>), new SlimTree().AddBranchValue("5").AddBranchValue("4")) as List<int>;
            ClassicAssert.IsNotNull(list);
            ClassicAssert.AreEqual(2, list.Count);
            ClassicAssert.AreEqual(5, list[0]);
            ClassicAssert.AreEqual(4, list[1]);
        }

        [Test] public void LeafIsParsedForArray() {
            var list =
                Parse(new ParseList{ Processor = processor }, typeof (int[]), new SlimLeaf("[5, 4]")) as int[];
            ClassicAssert.IsNotNull(list);
            ClassicAssert.AreEqual(2, list.Length);
            ClassicAssert.AreEqual(5, list[0]);
            ClassicAssert.AreEqual(4, list[1]);
        }

        [Test] public void ParsesEnumType() {
            ClassicAssert.AreEqual(BindingFlags.Public,
                            processor.Parse(typeof (BindingFlags), TypedValue.Void,
                                                       new SlimLeaf("Public")).Value);
        }

        [Test] public void ParsesIntegerForNullableInt() {
            ClassicAssert.AreEqual(1, processor.Parse(typeof (int?), TypedValue.Void, new SlimLeaf("1")).Value);
        }

        [Test] public void ParsesDictionary() {
            var dictionary = processor.Parse(typeof (Dictionary<string, string>), TypedValue.Void,
                    new SlimLeaf("<table><tr><td>key</td><td>value</td></tr></table>")).GetValue<Dictionary<string,string>>();
            ClassicAssert.AreEqual("value", dictionary["key"]);
        }

        [Test] [SetCulture("es-ES")] public void ParsesWithCurrentCulture() {
            ClassicAssert.AreEqual(1.001, processor.Parse(typeof (double), TypedValue.Void, new SlimLeaf("1,001")).Value);
        }

        static object Parse(ParseOperator<string> parseOperator, Type type, Tree<string> parameters) {
            ClassicAssert.IsTrue(parseOperator.CanParse(type, TypedValue.Void, parameters));
            TypedValue result = parseOperator.Parse(type, TypedValue.Void, parameters);
            return result.Value;
        }
    }
}
