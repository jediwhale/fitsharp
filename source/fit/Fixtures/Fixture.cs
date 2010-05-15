// Copyright © 2010 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Model;

namespace fit
{
	public class Fixture: MutableDomainAdapter, TargetObjectProvider, Interpreter
	{
	    static CellProcessor symbolProcessor; // backwards compatibility with static symbol methods

	    CellOperation cellOperation;
	    CellProcessor processor;

	    protected object mySystemUnderTest;
	    protected Fixture myParentFixture;

	    public CellProcessor Processor {
	        get { return processor; }
	        set {
	            processor = value;
	            symbolProcessor = value;
	        }
	    }

	    public TestStatus TestStatus { get { return Processor.TestStatus; } }

	    public CellOperation CellOperation {
	        get {
                if (cellOperation == null) cellOperation = new CellOperationImpl(Processor);
	            return cellOperation;
	        }
            set { cellOperation = value;}
	    }

        public Fixture() {}

        public Fixture(object systemUnderTest): this() { mySystemUnderTest = systemUnderTest; }

        public virtual bool IsVisible { get { return true; } }

	    public void Prepare(Interpreter parent, Tree<Cell> table) {
	        var parentFixture = (Fixture) parent;
	        Processor = parentFixture.Processor;
	        myParentFixture = parentFixture;
	        GetArgsForRow(table.Branches[0]);
	    }

        public void Interpret(Tree<Cell> table) { DoTable((Parse)table.Value); }

	    public virtual void DoTable(Parse table)
		{
			DoRows(table.Parts.More);
		}

		public virtual void DoRows(Parse rows)
		{
			while (rows != null)
			{
				Parse more = rows.More;
				DoRow(rows);
				rows = more;
			}
		}

		public virtual void DoRow(Parse row)
		{
			DoCells(row.Parts);
		}

		public virtual void DoCells(Parse cells)
		{
			for (int i = 0; cells != null; i++)
			{
				try
				{
					DoCell(cells, i);
				}
				catch (Exception e)
				{
					TestStatus.MarkException(cells, e);
				}
				cells = cells.More;
			}
		}

		public virtual void DoCell(Parse cell, int columnNumber)
		{
			TestStatus.MarkIgnore(cell);
		}

		// Annotation ///////////////////////////////

		public virtual void Right(Parse cell) { TestStatus.MarkRight(cell); }
		public virtual void Wrong(Parse cell) { TestStatus.MarkWrong(cell); }
		public virtual void Wrong(Parse cell, string actual) { TestStatus.MarkWrong(cell, actual); }
		public virtual void Ignore(Parse cell) { TestStatus.MarkIgnore(cell); }
		public virtual void Exception(Parse cell, Exception exception) { TestStatus.MarkException(cell, exception); }

		// Utility //////////////////////////////////

		public static string Label(string text)
		{
			return " <span class=\"fit_label\">" + text + "</span>";
		}

		public static string Gray(string text)
		{
			return " <span class=\"fit_grey\">" + text + "</span>";
		}

		public static string Escape(string text)
		{
			return Escape(Escape(text, '&', "&amp;"), '<', "&lt;");
		}

		public static string Escape(string text, char from, string to)
		{
			int i = -1;
			while ((i = text.IndexOf(from, i + 1)) >= 0)
			{
				if (i == 0)
					text = to + text.Substring(1);
				else if (i == text.Length)
					text = text.Substring(0, i) + to;
				else
					text = text.Substring(0, i) + to + text.Substring(i + 1);
			}
			return text;
		}

		public static object Recall(string key)
		{
		    return symbolProcessor.Contains(new Symbol(key))
		               ? symbolProcessor.Load(new Symbol(key)).Instance
		               : null;
		}

		public static void Save(string key, object value)
		{
			symbolProcessor.Store(new Symbol(key, value));
		}

		public static void ClearSaved()
		{
		    symbolProcessor.Clear<Symbol>();
		}

		public virtual object GetTargetObject()
		{
			return this;
		}

	    public string[] Args { get; private set; }

	    public void GetArgsForRow(Tree<Cell> row) {
	        Args = row.Branches.Skip(1).Aggregate(new List<string>(),
                (list, cell) => {
                    list.Add(cell.Value.Text);
                    return list;
                }).ToArray();
		}

        public object GetArgumentInput(int theIndex, Type theType) {
            return Processor.Parse(theType, new TypedValue(this), new CellTreeLeaf(Args[theIndex])).Value;
        }

	    public object SystemUnderTest {
	        get { return mySystemUnderTest;}
	    }

        public void SetSystemUnderTest(object theSystemUnderTest) {
            mySystemUnderTest = theSystemUnderTest;
        }
	}
}