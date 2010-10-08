// Copyright © 2010 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Text;
using fitnesse.fixtures;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;
using TestStatus=fitSharp.Fit.Model.TestStatus;

namespace fit.Test.NUnit
{
	[TestFixture]
	public class TableFixtureTest
	{
		string table;
	    StoryTest myStoryTest;
	    Parse finishedTable;
	    TestCounts resultCounts;

		[SetUp]
		public void SetUp()
		{
			TestUtils.InitAssembliesAndNamespaces();
			var builder = new StringBuilder();
			builder.Append("<table>");
			builder.Append("<tr><td colspan='5'>ExampleTableFixture</td></tr>");
			builder.Append("<tr><td>0,0</td><td>0,1</td><td>0,2</td><td>37</td><td></td></tr>");
			builder.Append("</table>");
			table = builder.ToString();
		    myStoryTest = new StoryTest(Parse.ParseFrom(table), SimpleWriter);
		    myStoryTest.Execute(new Configuration());
		}

		[TearDown]
		public void TearDown()
		{
			ExampleTableFixture.ResetStatics();
		}

		void SimpleWriter(Tree<Cell> theTables, TestCounts counts) {
            finishedTable = (Parse) theTables.Value;
		    resultCounts = counts;
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
			Assert.AreEqual(1, resultCounts.GetCount(TestStatus.Right));
            Assert.AreEqual(TestStatus.Right, finishedTable.At(0,1,0).GetAttribute(CellAttribute.Status));
		}

		[Test]
		public void TestWrong()
		{
			Assert.AreEqual(2, resultCounts.GetCount(TestStatus.Wrong));
            Assert.AreEqual(TestStatus.Wrong, finishedTable.At(0,1,1).GetAttribute(CellAttribute.Status));
            Assert.AreEqual(TestStatus.Wrong, finishedTable.At(0,1,2).GetAttribute(CellAttribute.Status));
			Assert.IsTrue(finishedTable.At(0,1,2).Body.IndexOf("actual") > 0);
		}

		[Test]
		public void TestIgnore()
		{
			Assert.AreEqual(1, resultCounts.GetCount(TestStatus.Ignore));
            Assert.AreEqual(TestStatus.Ignore, finishedTable.At(0,1,3).GetAttribute(CellAttribute.Status));
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
