// Copyright � 2011 Syterra Software Inc. Includes work by Object Mentor, Inc., � 2002 Cunningham & Cunningham, Inc.
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
using NUnit.Framework.Legacy;

namespace fit.Test.NUnit {
    [TestFixture]
    public class FixtureParametersTest{

        [Test]
        public void NoParameterCellsShouldResultInNoArguments()
        {
            const string tableString = "<table><tr><td>StringFixture</td></tr><tr><td>field</td><td>field</td></tr></table>";
            Fixture stringFixture = new StringFixture();
            stringFixture.Prepare(null, Parse.ParseFrom(tableString).Parts);
            ClassicAssert.AreEqual(0, stringFixture.Args.Length);
        }

        [Test]
        public void OneParameterCellShouldResultInOneArgument()
        {
            const string arg = "I'd like to buy an argument";
            const string tableString = "<table><tr><td>StringFixture</td><td>" + arg + "</td></tr><tr><td>field</td><td>field</td></tr></table>";
            Fixture stringFixture = new StringFixture();
            stringFixture.Prepare(null, Parse.ParseFrom(tableString).Parts);
            ClassicAssert.AreEqual(1, stringFixture.Args.Length);
            ClassicAssert.AreEqual(arg, stringFixture.Args[0]);
        }

        [Test]
        public void TwoParameterCellShouldResultInTwoArguments()
        {
            const string arg1 = "I'd like to buy an argument";
            const string arg2 = "I'd like to buy another argument";
            const string tableString = "<table><tr><td>StringFixture</td><td>" + arg1 + "</td><td>" + arg2 + "</td></tr><tr><td>field</td><td>field</td></tr></table>";
            Fixture stringFixture = new StringFixture();
            stringFixture.Prepare(null, Parse.ParseFrom(tableString).Parts);
            ClassicAssert.AreEqual(2, stringFixture.Args.Length);
            ClassicAssert.AreEqual(arg1, stringFixture.Args[0]);
            ClassicAssert.AreEqual(arg2, stringFixture.Args[1]);
        }
    }

    [TestFixture]
    public class EscapeTest
    {
        [Test]
        public void TestEscape()
        {
            const string junk = "!@#$%^*()_-+={}|[]\\:\";',./?`";
            ClassicAssert.AreEqual(junk, Fixture.Escape(junk));
            ClassicAssert.AreEqual("", Fixture.Escape(""));
            ClassicAssert.AreEqual("&lt;", Fixture.Escape("<"));
            ClassicAssert.AreEqual("&lt;&lt;", Fixture.Escape("<<"));
            ClassicAssert.AreEqual("x&lt;", Fixture.Escape("x<"));
            ClassicAssert.AreEqual("&amp;", Fixture.Escape("&"));
            ClassicAssert.AreEqual("&lt;&amp;&lt;", Fixture.Escape("<&<"));
            ClassicAssert.AreEqual("&amp;&lt;&amp;", Fixture.Escape("&<&"));
            ClassicAssert.AreEqual("a &lt; b &amp;&amp; c &lt; d", Fixture.Escape("a < b && c < d"));
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
            ClassicAssert.AreEqual(@"\some\path.html", fixture.Processor.Get<Context>().TestPagePath.ToString());
        }
    }

// Disable the Obsolete warning, we want to run these
// tests until we remove the Save/Recall methods entirely.
#pragma warning disable 612, 618

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
            ClassicAssert.IsNull(fixture.Symbols.GetValueOrDefault(key, null));
            fixture.Symbols.Save(key, value);
            ClassicAssert.AreEqual(value, fixture.Symbols.GetValueOrDefault(key, null));
        }

        [Test]
        public void TestSaveAndRecallTwoValues()
        {
            const string key = "aVariable";
            const string value = "aValue";
            const string otherKey = "anotherVariable";
            const string otherValue = "anotherValue";
            ClassicAssert.IsNull(fixture.Symbols.GetValueOrDefault(key, null));
            fixture.Symbols.Save(key, value);
            fixture.Symbols.Save(otherKey, otherValue);
            ClassicAssert.AreEqual(value, fixture.Symbols.GetValueOrDefault(key, null));
            ClassicAssert.AreEqual(otherValue, fixture.Symbols.GetValueOrDefault(otherKey, null));
        }

        [Test]
        public void TestSaveAndRecallChangedValue()
        {
            const string key = "aVariable";
            const string value = "aValue";
            const string otherValue = "anotherValue";
            fixture.Symbols.Save(key, value);
            fixture.Symbols.Save(key, otherValue);
            ClassicAssert.AreEqual(otherValue, fixture.Symbols.GetValueOrDefault(key, null));
        }

        [Test]
        public void TestGetTargetType()
        {
            ClassicAssert.AreSame(fixture, fixture.GetTargetObject());
        }
    }

#pragma warning restore 612, 618
}