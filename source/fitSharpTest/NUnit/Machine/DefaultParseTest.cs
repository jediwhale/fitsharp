// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Service;
using fitSharp.Test.Double;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class DefaultParseTest {
        private DefaultParse<string, Service> parse;
        private readonly Service processor = new Service();

        [SetUp] public void SetUp() {
            parse = new DefaultParse<string, Service> {Processor = processor};
        }

        [Test] public void StringIsParsed() {
            TypedValue result =  parse.Parse(typeof (string), TypedValue.Void, new TreeList<string>("stuff"));
            Assert.AreEqual("stuff", result.Value);
        }

        [Test] public void DateIsParsed() {
            TypedValue result = parse.Parse(typeof(DateTime), TypedValue.Void, new TreeList<string>("03 Jan 2008"));
            Assert.AreEqual(new DateTime(2008, 1, 3), result.Value);
        }

        [Test] public void ClassIsParsed() {
            TypedValue result = parse.Parse(typeof(SampleClass), TypedValue.Void, new TreeList<string>("stuff"));
            Assert.IsTrue(result.Value is SampleClass);
        }

        [Test] public void ClassWithStringConstructorIsParsed() {
            TypedValue result = parse.Parse(typeof(ClassFromString), TypedValue.Void, new TreeList<string>("stuff"));
            Assert.IsTrue(result.Value is ClassFromString);
        }

        private class ClassFromString {
            public ClassFromString(string stuff) {}
        }
    }
}