// Copyright © 2011 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Reflection;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Samples;
using NUnit.Framework;
using TestStatus=fitSharp.Fit.Model.TestStatus;

namespace fit.Test.NUnit {
    public class TestUtils
    {
        public static Memory InitAssembliesAndNamespaces()
        {
            var memory = new TypeDictionary();
            memory.GetItem<ApplicationUnderTest>().AddAssembly(Assembly.GetAssembly(typeof (TestUtils)).CodeBase);
            memory.GetItem<ApplicationUnderTest>().AddNamespace("fit.Test.NUnit");
            memory.GetItem<ApplicationUnderTest>().AddNamespace("fit.Test.Acceptance");
            return memory;
        }

        public static Parse CreateCell(string value)
        {
            return new Parse("td", value, null, null);
        }

        public static Tree<Cell> CreateCellRange(string value)
		{
            return new CellTree(value);
		}

        public static bool IsMatch(CompareOperator<Cell> compareOperator, object instance, Type type, string value) {
            return compareOperator.CanCompare(new TypedValue(instance, type), CreateCell(value));
        }

        public static void DoInput(Fixture fixture, Tree<Cell> range, Parse cell) {
            new InputBinding(fixture.Processor, fixture, range).Do(cell);
        }

        public static void DoCheck(Fixture fixture, Tree<Cell> range, Parse cell) {
            fixture.Processor.Check(fixture.GetTargetObject(), range, cell);
        }

        public static TestCounts MakeTestCounts() {
            var counts = new TestCounts();
            counts.AddCount(TestStatus.Right);
            counts.AddCount(TestStatus.Wrong);
            counts.AddCount(TestStatus.Wrong);
            counts.AddCount(TestStatus.Ignore);
            counts.AddCount(TestStatus.Ignore);
            counts.AddCount(TestStatus.Ignore);
            counts.AddCount(TestStatus.Exception);
            counts.AddCount(TestStatus.Exception);
            counts.AddCount(TestStatus.Exception);
            counts.AddCount(TestStatus.Exception);
            return counts;
        }

        public static void CheckCounts(TestCounts counts, int right, int wrong, int ignore, int exception) {
            Assert.AreEqual(right, counts.GetCount(TestStatus.Right));
            Assert.AreEqual(wrong, counts.GetCount(TestStatus.Wrong));
            Assert.AreEqual(ignore, counts.GetCount(TestStatus.Ignore));
            Assert.AreEqual(exception, counts.GetCount(TestStatus.Exception));
        }

        public static void VerifyCounts(Fixture fixture, int right, int wrong, int ignores, int exceptions) {
            Assert.AreEqual(right, fixture.TestStatus.Counts.GetCount(TestStatus.Right));
            Assert.AreEqual(wrong, fixture.TestStatus.Counts.GetCount(TestStatus.Wrong));
            Assert.AreEqual(ignores, fixture.TestStatus.Counts.GetCount(TestStatus.Ignore));
            Assert.AreEqual(exceptions, fixture.TestStatus.Counts.GetCount(TestStatus.Exception));
        }
    }

    [TestFixture]
    public class PersonTest
    {
        [Test]
        public void TestConstructor() {
            var person = new Person("john", "doe");
            Assert.AreEqual("john doe", person.ToString());
        }

        [Test]
        public void TestConstructorWithId() {
            var person = new Person(1, "jane", "roe");
            Assert.AreEqual("jane roe", person.ToString());
            Assert.AreEqual("jane", person.FirstName);
            Assert.AreEqual("roe", person.LastName);
            Assert.AreEqual(1, person.Id);
        }

        [Test]
        public void TestIsTalented()
        {
            var person = new Person("Scott", "Henderson");
            person.SetTalented(true);
            Assert.IsTrue(person.IsTalented);
        }

        [Test]
        public void TestParse()
        {
            const string name = "joe schmoe";
            Person person = Person.Parse(name);
            Assert.AreEqual(name, person.ToString());
        }

        [Test]
        public void TestEquals()
        {
            var original = new Person("Wes", "Montgomery");
            var copy = new Person("Wes", "Montgomery");
            Assert.IsTrue(original.Equals(copy));
        }
    }
}