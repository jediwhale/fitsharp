﻿// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Test.Double;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class DefaultParseTest {
        private DefaultParse<string, BasicProcessor> parse;
        private readonly BasicProcessor processor = new BasicProcessor();

        [SetUp] public void SetUp() {
            parse = new DefaultParse<string, BasicProcessor> {Processor = processor};
        }

        [Test] public void StringIsParsed() {
            TypedValue result =  parse.Parse(typeof (string), TypedValue.Void, new TreeList<string>("stuff"));
            ClassicAssert.AreEqual("stuff", result.Value);
        }

        [Test] public void DateIsParsed() {
            TypedValue result = parse.Parse(typeof(DateTime), TypedValue.Void, new TreeList<string>("03 Jan 2008"));
            ClassicAssert.AreEqual(new DateTime(2008, 1, 3), result.Value);
        }

        [Test] public void ClassIsParsed() {
            TypedValue result = parse.Parse(typeof(SampleClass), TypedValue.Void, new TreeList<string>("stuff"));
            ClassicAssert.IsTrue(result.Value is SampleClass);
        }

        [Test] public void ClassWithStringConstructorIsParsed() {
            TypedValue result = parse.Parse(typeof(ClassFromString), TypedValue.Void, new TreeList<string>("stuff"));
            ClassicAssert.IsTrue(result.Value is ClassFromString);
        }

        [Test] public void StructWithParseAndConstructorIsParsed() {
            TypedValue result = parse.Parse(typeof(StructWithParseAndConstructor), TypedValue.Void, new TreeList<string>("stuff"));
            ClassicAssert.IsTrue(result.Value is StructWithParseAndConstructor);
            ClassicAssert.AreEqual("stuffparsector", result.GetValue<StructWithParseAndConstructor>().stuff);
        }

        private class ClassFromString {
            public ClassFromString(string stuff) {}
        }

        private struct StructWithParseAndConstructor {
            public readonly string stuff;
            public static StructWithParseAndConstructor Parse(string stuff) { return new StructWithParseAndConstructor(stuff + "parse"); }
            public StructWithParseAndConstructor(string stuff) { this.stuff = stuff + "ctor"; }
        }
    }
}