// Copyright © 2020 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Reflection;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Samples;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using TestStatus=fitSharp.Fit.Model.TestStatus;

namespace fit.Test.NUnit {
    public class TestUtils
    {
        public static Memory InitAssembliesAndNamespaces()
        {
            var memory = new TypeDictionary();
            memory.GetItem<ApplicationUnderTest>().AddAssembly(TargetFramework.Location(Assembly.GetAssembly(typeof (TestUtils))));
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

        public static void DoInput(Fixture fixture, Tree<Cell> range, Parse cell) {
            new InputBinding(fixture.Processor, fixture, range).Do(cell);
        }

        public static void DoCheck(Fixture fixture, Tree<Cell> range, Parse cell) {
            fixture.Processor.Check(fixture.GetTargetObject(), range, cell);
        }

        public static void CheckCounts(TestCounts counts, int right, int wrong, int ignore, int exception) {
            ClassicAssert.AreEqual(right, counts.GetCount(TestStatus.Right));
            ClassicAssert.AreEqual(wrong, counts.GetCount(TestStatus.Wrong));
            ClassicAssert.AreEqual(ignore, counts.GetCount(TestStatus.Ignore));
            ClassicAssert.AreEqual(exception, counts.GetCount(TestStatus.Exception));
        }

        public static void VerifyCounts(Fixture fixture, int right, int wrong, int ignores, int exceptions) {
            ClassicAssert.AreEqual(right, fixture.TestStatus.Counts.GetCount(TestStatus.Right));
            ClassicAssert.AreEqual(wrong, fixture.TestStatus.Counts.GetCount(TestStatus.Wrong));
            ClassicAssert.AreEqual(ignores, fixture.TestStatus.Counts.GetCount(TestStatus.Ignore));
            ClassicAssert.AreEqual(exceptions, fixture.TestStatus.Counts.GetCount(TestStatus.Exception));
        }
    }

    [TestFixture]
    public class PersonTest
    {
        [Test]
        public void TestConstructor() {
            var person = new Person("john", "doe");
            ClassicAssert.AreEqual("john doe", person.ToString());
        }

        [Test]
        public void TestConstructorWithId() {
            var person = new Person(1, "jane", "roe");
            ClassicAssert.AreEqual("jane roe", person.ToString());
            ClassicAssert.AreEqual("jane", person.FirstName);
            ClassicAssert.AreEqual("roe", person.LastName);
            ClassicAssert.AreEqual(1, person.Id);
        }

        [Test]
        public void TestIsTalented()
        {
            var person = new Person("Scott", "Henderson");
            person.SetTalented(true);
            ClassicAssert.IsTrue(person.IsTalented);
        }

        [Test]
        public void TestParse()
        {
            const string name = "joe schmoe";
            Person person = Person.Parse(name);
            ClassicAssert.AreEqual(name, person.ToString());
        }

        [Test]
        public void TestEquals()
        {
            var original = new Person("Wes", "Montgomery");
            var copy = new Person("Wes", "Montgomery");
            ClassicAssert.IsTrue(original.Equals(copy));
        }
    }
}