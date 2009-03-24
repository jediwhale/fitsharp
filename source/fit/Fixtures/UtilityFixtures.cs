// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;

namespace fit
{
	public class IntFixture : ColumnFixture
	{
		public int Field;
		private int propertyValue;
		private int methodValue;

		public int Property
		{
			get { return propertyValue; }
			set { propertyValue = value; }
		}

		public void Set(int value)
		{
			methodValue = value;
		}

		public int Get()
		{
			return methodValue;
		}
	}

	public class StringFixture : ColumnFixture
	{
		public string Field;
		private string propertyValue;
		private string methodValue;

		public string Property
		{
			get { return propertyValue; }
			set { propertyValue = value; }
		}

		public void Set(string value)
		{
			methodValue = value;
		}

		public string Get()
		{
			return methodValue;
		}
	}

	public class ArrayOfIntsFixture : ColumnFixture
	{
		public int[] Field;
		private int[] propertyValue;
		private int[] methodValue;

		public int[] Property
		{
			set { propertyValue = value; }
			get { return propertyValue; }
		}

		public void Set(int[] value)
		{
			methodValue = value;
		}

		public int[] Get()
		{
			return methodValue;
		}
	}

	public class DoubleFixture : ColumnFixture
	{
		public double Field;
		private double propertyValue;
		private double methodValue;

		public void Set(double value)
		{
			methodValue = value;
		}

		public double Get()
		{
			return methodValue;
		}

		public double Property
		{
			set { propertyValue = value; }
			get { return propertyValue; }
		}
	}

	public class LongFixture : ColumnFixture
	{
		public long Field;
		private long propertyValue;
		private long methodValue;

		public void Set(long value)
		{
			methodValue = value;
		}

		public long Get()
		{
			return methodValue;
		}

		public long Property
		{
			set { propertyValue = value; }
			get { return propertyValue; }
		}
	}

	public class DecimalFixture : ColumnFixture
	{
		public decimal Field;
		private decimal propertyValue;
		private decimal methodValue;

		public void Set(decimal value)
		{
			methodValue = value;
		}

		public decimal Get()
		{
			return methodValue;
		}

		public decimal Property
		{
			set { propertyValue = value; }
			get { return propertyValue; }
		}
	}

	public class FloatFixture : ColumnFixture
	{
		public float Field;
		private float propertyValue;
		private float methodValue;

		public void Set(float value)
		{
			methodValue = value;
		}

		public float Get()
		{
			return methodValue;
		}

		public float Property
		{
			get { return propertyValue; }
			set { propertyValue = value; }
		}
	}

	public class BoolFixture : ColumnFixture
	{
		public bool Field;
		private bool propertyValue;
		private bool methodValue;

		public bool Property
		{
			get { return propertyValue; }
			set { propertyValue = value; }
		}

		public void Set(bool value)
		{
			methodValue = value;
		}

		public bool Get()
		{
			return methodValue;
		}
	}

	public class ArrayOfBoolsFixture : ColumnFixture
	{
		public bool[] Field;
		private bool[] propertyValues;
		private bool[] methodValues;

		public bool[] Property
		{
			set { propertyValues = value; }
			get { return propertyValues; }
		}

		public void Set(bool[] value)
		{
			methodValues = value;
		}

		public bool[] Get()
		{
			return methodValues;
		}
	}
	public class ArrayOfStringsFixture : ColumnFixture
	{
		public string[] Field;

		public void Set(string[] value)
		{
			Field = value;
		}

		public string[] Get()
		{
			return Field;
		}

		public string[] Property
		{
			set { Field = value; }
			get { return Field; }
		}

		public void Save()
		{
            ArrayOfStringsRowFixture.items.Add(this);
		}
	}

    public class ArrayOfStringsRowFixture : RowFixture
	{
		public static ArrayList items = new ArrayList();

		public override object[] Query()
		{
			return items.ToArray();
		}

		public override Type GetTargetClass()
		{
			return typeof(ArrayOfStringsFixture);
		}
	}

	public class ErrorThrowingFixture : ColumnFixture
	{
		public string ErrorThrowingMethod()
		{
			throw new ApplicationException();
		}

		public string ErrorThrowingProperty
		{
			get { throw new ApplicationException(); }
		}

		public string RedirectToErrorThrowingMethod()
		{
			return ErrorThrowingMethod();
		}
	}

	public class ExceptionThrowingFixture : ColumnFixture
	{
		public string Message;
		public string ThrowNullReferenceException()
		{
			throw new NullReferenceException(Message);
		}
		public string ThrowApplicationException()
		{
			throw new ApplicationException(Message);
		}
	}

}
