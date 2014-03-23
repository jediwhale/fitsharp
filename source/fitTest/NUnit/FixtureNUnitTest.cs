// Copyright © 2011 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Test.Double;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Samples.Fit;
using NUnit.Framework;
using fitSharp.Machine.Application;

namespace fit.Test.NUnit {
    [TestFixture]
    public class FixtureParametersTest{

        [Test]
        public void NoParameterCellsShouldResultInNoArguments()
        {
            const string tableString = "<table><tr><td>StringFixture</td></tr><tr><td>field</td><td>field</td></tr></table>";
            Fixture stringFixture = new StringFixture();
            stringFixture.Prepare(null, Parse.ParseFrom(tableString).Parts);
            Assert.AreEqual(0, stringFixture.Args.Length);
        }

        [Test]
        public void OneParameterCellShouldResultInOneArgument()
        {
            const string arg = "I'd like to buy an argument";
            const string tableString = "<table><tr><td>StringFixture</td><td>" + arg + "</td></tr><tr><td>field</td><td>field</td></tr></table>";
            Fixture stringFixture = new StringFixture();
            stringFixture.Prepare(null, Parse.ParseFrom(tableString).Parts);
            Assert.AreEqual(1, stringFixture.Args.Length);
            Assert.AreEqual(arg, stringFixture.Args[0]);
        }

        [Test]
        public void TwoParameterCellShouldResultInTwoArguments()
        {
            const string arg1 = "I'd like to buy an argument";
            const string arg2 = "I'd like to buy another argument";
            const string tableString = "<table><tr><td>StringFixture</td><td>" + arg1 + "</td><td>" + arg2 + "</td></tr><tr><td>field</td><td>field</td></tr></table>";
            Fixture stringFixture = new StringFixture();
            stringFixture.Prepare(null, Parse.ParseFrom(tableString).Parts);
            Assert.AreEqual(2, stringFixture.Args.Length);
            Assert.AreEqual(arg1, stringFixture.Args[0]);
            Assert.AreEqual(arg2, stringFixture.Args[1]);
        }
    }

    [TestFixture]
    public class EscapeTest
    {
        [Test]
        public void TestEscape()
        {
            const string junk = "!@#$%^*()_-+={}|[]\\:\";',./?`";
            Assert.AreEqual(junk, Fixture.Escape(junk));
            Assert.AreEqual("", Fixture.Escape(""));
            Assert.AreEqual("&lt;", Fixture.Escape("<"));
            Assert.AreEqual("&lt;&lt;", Fixture.Escape("<<"));
            Assert.AreEqual("x&lt;", Fixture.Escape("x<"));
            Assert.AreEqual("&amp;", Fixture.Escape("&"));
            Assert.AreEqual("&lt;&amp;&lt;", Fixture.Escape("<&<"));
            Assert.AreEqual("&amp;&lt;&amp;", Fixture.Escape("&<&"));
            Assert.AreEqual("a &lt; b &amp;&amp; c &lt; d", Fixture.Escape("a < b && c < d"));
        }
    }

    [TestFixture]
    public class ContextTest {
        private Fixture fixture;
        private CellProcessorBase processor;

        [SetUp]
        public void SetUp() {
            processor = Builder.CellProcessor();
            processor.Memory.GetItem<Context>().TestPagePath = new FilePath(@"\some\path.html");

            fixture = new Fixture { Processor = processor };
        }

        [Test]
        public void TestProcessorContextDataIsExposedToFixture() {
            // I admit this test is a little forced, but I have another test verifying that
            // the context is populated by SuiteRunner, so it makes sense to test
            // that the fixture sees the change, too. To me, anyway.
            Assert.AreEqual(@"\some\path.html", fixture.Processor.Get<Context>().TestPagePath.ToString());
        }
    }

    [TestFixture]
    public class SaveAndRecallTest
    {
        private Fixture fixture;

        [SetUp] public void SetUp() {
            fixture = new Fixture { Processor = Builder.CellProcessor() };
        }

        [Test]
        public void TestSaveAndRecallValue()
        {
            const string key = "aVariable";
            const string value = "aValue";
            Assert.IsNull(fixture.Symbols.GetValueOrDefault(key, null));
            fixture.Symbols.Save(key, value);
            Assert.AreEqual(value, fixture.Symbols.GetValueOrDefault(key, null));
        }

        [Test]
        public void TestSaveAndRecallTwoValues()
        {
            const string key = "aVariable";
            const string value = "aValue";
            const string otherKey = "anotherVariable";
            const string otherValue = "anotherValue";
            Assert.IsNull(fixture.Symbols.GetValueOrDefault(key, null));
            fixture.Symbols.Save(key, value);
            fixture.Symbols.Save(otherKey, otherValue);
            Assert.AreEqual(value, fixture.Symbols.GetValueOrDefault(key, null));
            Assert.AreEqual(otherValue, fixture.Symbols.GetValueOrDefault(otherKey, null));
        }

        [Test]
        public void TestSaveAndRecallChangedValue()
        {
            const string key = "aVariable";
            const string value = "aValue";
            const string otherValue = "anotherValue";
            fixture.Symbols.Save(key, value);
            fixture.Symbols.Save(key, otherValue);
            Assert.AreEqual(otherValue, fixture.Symbols.GetValueOrDefault(key, null));
        }

        [Test]
        public void TestGetTargetType()
        {
            Assert.AreSame(fixture, fixture.GetTargetObject());
        }
    }
}