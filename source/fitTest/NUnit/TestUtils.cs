// FitNesse.NET
// Copyright © 2008,2009 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Reflection;
using System.Text;
using fit.Engine;
using fitlibrary;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    public class TestUtils
    {
        public static void InitAssembliesAndNamespaces()
        {
            Context.Configuration.GetItem<ApplicationUnderTest>().AddAssembly(Assembly.GetAssembly(typeof (TestUtils)).CodeBase);
            Context.Configuration.GetItem<ApplicationUnderTest>().AddNamespace("fit.Test.NUnit");
            Context.Configuration.GetItem<ApplicationUnderTest>().AddNamespace("fit.Test.Acceptance");
        }

        public static Parse CreateCell(string value)
        {
            return new Parse("td", value, null, null);
        }

        public static CellRange CreateCellRange(string value)
		{
            return new CellRange(CreateCell(value));
		}

        public static bool IsMatch(CompareOperator<Cell> compareOperator, object instance, Type type, string value) {
            var processor = new Processor<Cell>();
            processor.AddOperator(new CompareDefault());
            bool result = true;
            return compareOperator.TryCompare(processor, new TypedValue(instance, type), CreateCell(value), ref result);
        }

        public static bool IsMatch(ExecuteOperator<Cell> executor, Tree<Cell> parameters) {
            var processor = new Processor<Cell>();
            processor.AddMemory<Symbol>();
            processor.AddOperator(new ParseMemberName());
            TypedValue result = TypedValue.Void;
            return executor.TryExecute(processor, new TypedValue(new ExecuteContext(new Fixture(), new TypedValue("stuff"))), parameters, ref result);
        }
    }

    public class Person
    {
        private int id;
        private string firstName;
        private string lastName;
        private bool talented;

        public int Id
        {
            get { return id; }
        }

        public string FirstName
        {
            get { return firstName; }
        }

        public string LastName
        {
            get { return lastName; }
        }

        public static Person Parse(string name)
        {
            string[] names = name.Split(' ');
            return new Person(names[0], names[1]);
        }

        public Person(string firstName, string lastName) {
            this.firstName = firstName;
            this.lastName = lastName;
        }

        public Person(int id, string firstName, string lastName) {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(firstName);
            if (builder.Length > 0 && lastName != null && lastName.Length > 0)
            {
                builder.Append(" ");
            }
            return builder.Append(lastName).ToString();
        }

        public override bool Equals(object obj)
        {
            Person that = obj as Person;
            if (that == null)
                return false;
            return this.firstName == that.firstName && this.lastName == that.lastName;
        }

        public override int GetHashCode() {
            return id.GetHashCode() + firstName.GetHashCode() + lastName.GetHashCode();
        }

        public void SetTalented(bool talented)
        {
            this.talented = talented;
        }

        public bool IsTalented
        {
            get { return talented; }
        }
    }

    [TestFixture]
    public class PersonTest
    {
        [Test]
        public void TestConstructor() {
            Person person = new Person("john", "doe");
            Assert.AreEqual("john doe", person.ToString());
        }

        [Test]
        public void TestConstructorWithId() {
            Person person = new Person(1, "jane", "roe");
            Assert.AreEqual("jane roe", person.ToString());
            Assert.AreEqual("jane", person.FirstName);
            Assert.AreEqual("roe", person.LastName);
            Assert.AreEqual(1, person.Id);
        }

        [Test]
        public void TestIsTalented()
        {
            Person person = new Person("Scott", "Henderson");
            person.SetTalented(true);
            Assert.IsTrue(person.IsTalented);
        }

        [Test]
        public void TestParse()
        {
            string name = "joe schmoe";
            Person person = Person.Parse(name);
            Assert.AreEqual(name, person.ToString());
        }

        [Test]
        public void TestEquals()
        {
            Person original = new Person("Wes", "Montgomery");
            Person copy = new Person("Wes", "Montgomery");
            Assert.IsTrue(original.Equals(copy));
        }
    }
}