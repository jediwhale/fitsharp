// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class DefaultParseTest {
        private DefaultParse<string> parse;
        private readonly Processor<string> processor = new Processor<string>(new ApplicationUnderTest());

        [SetUp] public void SetUp() {
            parse = new DefaultParse<string>();
        }

        [Test] public void StringIsParsed() {
            TypedValue result =  parse.Parse(processor, typeof (string), TypedValue.Void, new TreeLeaf<string>("stuff"));
            Assert.AreEqual("stuff", result.Value);
        }

        [Test] public void DateIsParsed() {
            TypedValue result = parse.Parse(processor, typeof(DateTime), TypedValue.Void, new TreeLeaf<string>("03 Jan 2008"));
            Assert.AreEqual(new DateTime(2008, 1, 3), result.Value);
        }

        [Test] public void ClassIsParsed() {
            TypedValue result = parse.Parse(processor, typeof(SampleClass), TypedValue.Void, new TreeLeaf<string>("stuff"));
            Assert.IsTrue(result.Value is SampleClass);
        }

        [Test] public void ClassWithStringConstructorIsParsed() {
            TypedValue result = parse.Parse(processor, typeof(ClassFromString), TypedValue.Void, new TreeLeaf<string>("stuff"));
            Assert.IsTrue(result.Value is ClassFromString);
        }

        private class ClassFromString {
            public ClassFromString(string stuff) {}
        }
    }
}