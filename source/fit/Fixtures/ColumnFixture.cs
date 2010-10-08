// Copyright © 2010 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Service;

namespace fit
{
	public class ColumnFixture : BoundFixture
	{
		public Binding[] ColumnBindings;

		public override void DoRows(Parse rows)
		{
			Bind(rows.Parts);
			base.DoRows(rows.More);
		}

		public override void DoRow(Parse row) 
		{
			HasExecuted = false;
			try {
				Reset();
				base.DoRow(row);
				if (!HasExecuted) 
					Execute();
			} 
			catch (Exception e) {
				TestStatus.MarkException(row.Leaf, e);
			}
		}

		public virtual void Reset() {
			// about to process first cell of row
		}

		public override void DoCell(Parse cell, int column) {
			Binding binding = ColumnBindings[column];
			try {
				binding.Do(cell);
			} 
			catch(Exception e) {
				TestStatus.MarkException(cell, e);
			}
		}

		protected void Bind(Parse headerCells)
		{
		    var factory = new BindingFactory(Processor, this, this);
			ColumnBindings = new Binding[headerCells.Size];
			for (int i = 0; headerCells != null; i++, headerCells = headerCells.More)
			{
				ColumnBindings[i] = new Binding(factory.Make(headerCells)).BeforeCheck(CheckCalled);
			}
		}

	}

}