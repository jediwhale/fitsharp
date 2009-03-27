// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using fit.Test.NUnit;

namespace fit.Test.Acceptance
{
	public class PersonFixture : ColumnFixture
	{
		public Person Field;
		private Person propertyValue;
		private Person methodValue;

		public Person Property
		{
			set { propertyValue = value; }
			get { return propertyValue; }
		}

		public void Set(Person value)
		{
			methodValue = value;
		}

		public Person Get()
		{
			return methodValue;
		}
	}

	public class PeopleLoaderFixture : ColumnFixture
	{
		public static ArrayList people = new ArrayList();
		public string FirstName;
		public string LastName;
		public int id;
		public override void Execute() {
			people.Add(new Person(id, FirstName, LastName));
		}
		public string Clear()
		{
			people.Clear();
			return "cleared";
		}
	}

	public class PeopleRowFixtureCleaner : Fixture
	{
		public PeopleRowFixtureCleaner()
		{
			PeopleLoaderFixture.people.Clear();
		}
	}

    public class PeopleRowFixture : RowFixture
	{
		public override object[] Query()
		{
			return PeopleLoaderFixture.people.ToArray();
		}

		public override Type GetTargetClass()
		{
			return typeof(Person);
		}
	}

	public class ArrayOfPeopleFixture : ColumnFixture
	{
		public Person[] Field;
		private Person[] propertyValue;
		private Person[] methodValue;

		public Person[] Property
		{
			set { propertyValue = value; }
			get { return propertyValue; }
		}

		public void Set(Person[] value)
		{
			methodValue = value;
		}

		public Person[] Get()
		{
			return methodValue;
		}
	}

}
