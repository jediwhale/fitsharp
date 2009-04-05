// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Text;
using fitnesse.fixtures;
using fitSharp.Fit.Model;
using NUnit.Framework;

namespace fit.Test.NUnit
{
	[TestFixture]
	public class TableFixtureTest
	{
		private string table;
	    private StoryTest myStoryTest;

		[SetUp]
		public void SetUp()
		{
			TestUtils.InitAssembliesAndNamespaces();
			StringBuilder builder = new StringBuilder();
			builder.Append("<table>");
			builder.Append("<tr><td colspan='5'>ExampleTableFixture</td></tr>");
			builder.Append("<tr><td>0,0</td><td>0,1</td><td>0,2</td><td>37</td><td></td></tr>");
			builder.Append("</table>");
			table = builder.ToString();
		    myStoryTest = new StoryTest(new Parse(table), new SimpleFixtureListener());
		    myStoryTest.Execute();
		}

		[TearDown]
		public void TearDown()
		{
			ExampleTableFixture.ResetStatics();
		}

		private class SimpleFixtureListener : FixtureListener
		{
			private Parse finishedTable;

			public Parse FinishedTable
			{
				get { return finishedTable; }
				set { finishedTable = value; }
			}

			public void TableFinished(Parse finishedTable)
			{
				this.finishedTable = finishedTable;
			}

			public void TablesFinished(Parse theTables, Counts counts)
			{
			}
		}

		private Parse getFinishedTable()
		{
			return ((SimpleFixtureListener)myStoryTest.Listener).FinishedTable;
		}

		[Test]
		public void TestNumRows()
		{
			Assert.AreEqual(1, ExampleTableFixture.numRows);
		}

		[Test]
		public void TestDoStaticTable()
		{
			Assert.AreEqual(1, ExampleTableFixture.timesDoStaticTableCalled);
		}

		[Test]
		public void TestGetCell()
		{
			Assert.AreEqual("0,0", ExampleTableFixture.ZeroZero);
		}

		[Test]
		public void TestGetText()
		{
			Assert.AreEqual("0,1", ExampleTableFixture.ZeroOne);
		}

		[Test]
		public void TestGetInt()
		{
			Assert.AreEqual(37, ExampleTableFixture.resultOfGetInt);
		}

		[Test]
		public void TestRight()
		{
			Assert.AreEqual(1, myStoryTest.Counts.Right);
            Assert.AreEqual(CellAttributes.PassStatus, getFinishedTable().At(0,1,0).GetAttribute(CellAttributes.StatusKey));
		}

		[Test]
		public void TestWrong()
		{
			Assert.AreEqual(2, myStoryTest.Counts.Wrong);
            Assert.AreEqual(CellAttributes.FailStatus, getFinishedTable().At(0,1,1).GetAttribute(CellAttributes.StatusKey));
            Assert.AreEqual(CellAttributes.FailStatus, getFinishedTable().At(0,1,2).GetAttribute(CellAttributes.StatusKey));
			Assert.IsTrue(getFinishedTable().At(0,1,2).Body.IndexOf("actual") > 0);
		}

		[Test]
		public void TestIgnore()
		{
			Assert.AreEqual(1, myStoryTest.Counts.Ignores);
            Assert.AreEqual(CellAttributes.IgnoreStatus, getFinishedTable().At(0,1,3).GetAttribute(CellAttributes.StatusKey));
		}

		[Test]
		public void TestBlank()
		{
			Assert.IsTrue(ExampleTableFixture.blankCell);
		}
	}
}

namespace fit
{
	public class ExampleTableFixture : TableFixture
	{
		public static int timesDoStaticTableCalled;
		public static int numRows;
		public static string ZeroZero;
		public static string ZeroOne;
		public static int resultOfGetInt;
		public static bool blankCell;

		public static void ResetStatics()
		{
			timesDoStaticTableCalled = 0;
			numRows = 0;
			ZeroZero = null;
			ZeroOne = null;
			resultOfGetInt = 0;
			blankCell = false;
		}

		protected override void DoStaticTable(int rows)
		{
			timesDoStaticTableCalled++;
			numRows = rows;
			ZeroZero = GetCell(0,0).Text;
			ZeroOne = GetString(0,1);
			Right(0,0);
			Wrong(0,1);
			Wrong(0,2,"0,3");
			Ignore(0,3);
			resultOfGetInt = GetInt(0,3);
			blankCell = Blank(0,4);
		}
	}
}