// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Application;
using fitSharp.Machine.Model;

namespace fit
{
	public class Fixture: MutableDomainAdapter, TargetObjectProvider, Interpreter
	{
	    public CellProcessor Processor { get; set; }

	    public TestStatus TestStatus { get; set; }

        public CellOperation CellOperation { get { return new CellOperation(Processor); }}

        public Fixture() { TestStatus = new TestStatus(); }
        public Fixture(object systemUnderTest): this() { mySystemUnderTest = systemUnderTest; }

        public virtual bool IsInFlow(int tableCount) { return false; }
        public virtual bool IsVisible { get { return true; } }

	    public void Prepare(Interpreter parent, Tree<Cell> table) {
	        var parentFixture = (Fixture) parent;
	        Processor = parentFixture.Processor;
	        myParentFixture = parentFixture;
	        TestStatus = parentFixture.TestStatus;
	        GetArgsForTable((Parse)table.Value);
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

        public static object LoadClass(string theClassName) {
            return Context.Configuration.GetItem<Service.Service>().Create(theClassName.Trim(), new TreeList<Cell>()).Value;
        }

		public static Fixture LoadFixture(string theClassName)
		{
			try
			{
				return (Fixture) LoadClass(theClassName);
			}
			catch (InvalidCastException e)
			{
				throw new ApplicationException("Couldn't cast " + theClassName + " to Fixture.  Did you remember to extend Fixture?", e);
			}
		}

		public static object Recall(string key)
		{
		    return Context.Configuration.GetItem<Service.Service>().Contains(new Symbol(key))
		               ? Context.Configuration.GetItem<Service.Service>().Load(new Symbol(key)).Instance
		               : null;
		}

		public static void Save(string key, object value)
		{
			Context.Configuration.GetItem<Service.Service>().Store(new Symbol(key, value));
		}

		public static void ClearSaved()
		{
		    Context.Configuration.GetItem<Service.Service>().Clear<Symbol>();
		}

		public virtual object GetTargetObject()
		{
			return this;
		}

	    public string[] Args { get; private set; }

	    public void GetArgsForTable(Parse table)
		{
			var list = new ArrayList();
			list.Clear();
			Parse parameters = table.Parts.Parts.More;
			for (; parameters != null; parameters = parameters.More)
				list.Add(parameters.Text);
			Args = new string[list.Count];
			for (int i = 0; i < list.Count; i++)
				Args[i] = (string) list[i];
		}

        public object GetArgumentInput(int theIndex, Type theType) {
            return Processor.Parse(theType, new TypedValue(this), new StringCell(Args[theIndex])).Value;
        }

	    public object SystemUnderTest {
	        get { return mySystemUnderTest;}
	    }

        public void SetSystemUnderTest(object theSystemUnderTest) {
            mySystemUnderTest = theSystemUnderTest;
        }

	    protected object mySystemUnderTest;
	    protected Fixture myParentFixture;
	}
}