// Copyright © 2011 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit
{
	public class Fixture: MutableDomainAdapter, TargetObjectProvider, Interpreter
	{
	    static CellProcessor symbolProcessor; // compatibility with obsolete static symbol methods

	    CellOperation cellOperation;
	    CellProcessor processor;

	    protected object mySystemUnderTest;

	    public CellProcessor Processor {
	        get { return processor; }
	        set {
	            processor = value;
	            symbolProcessor = processor;
	        }
	    }

	    public TestStatus TestStatus { get { return Processor.TestStatus; } }
        public Symbols Symbols { get { return Processor.Get<Symbols>(); } }

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

	    public void Prepare(CellProcessor processor, Tree<Cell> row) {
	        Processor = processor;
	        GetArgsForRow(row);
	    }

	    public void Interpret(CellProcessor processor, Tree<Cell> table) {
	        Prepare(processor, table.Branches[0]);
	        if (!IsVisible) this.processor.TestStatus.TableCount--;
	        table.Value.As<Parse>(DoTable);
	    }

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

		[Obsolete] public static object Recall(string key)
		{
		    return symbolProcessor.Get<Symbols>().GetValueOrDefault(key, null);
		}

		[Obsolete] public static void Save(string key, object value)
		{
			symbolProcessor.Get<Symbols>().Save(key, value);
		}

		[Obsolete] public static void ClearSaved()
		{
		    symbolProcessor.Get<Symbols>().Clear();
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

        public V GetArgumentInput<V>(int theIndex) {
            return Processor.Parse(typeof(V), new TypedValue(this), new CellTreeLeaf(Args[theIndex])).GetValue<V>();
        }

	    public object SystemUnderTest {
	        get { return mySystemUnderTest;}
	    }

        public void SetSystemUnderTest(object theSystemUnderTest) {
            mySystemUnderTest = theSystemUnderTest;
        }
	}
}