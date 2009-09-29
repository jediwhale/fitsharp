// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Text.RegularExpressions;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fit
{
	public class ColumnFixture : BoundFixture
	{
		public Binding[] ColumnBindings;
	    private static readonly IdentifierName newIdentifier = new IdentifierName("new ");

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
				binding.HandleCell(this, cell);
			} 
			catch(Exception e) {
				TestStatus.MarkException(cell, e);
			}
		}

		protected void Bind(Parse headerCells)
		{
			ColumnBindings = new Binding[headerCells.Size];
			for (int i = 0; headerCells != null; i++, headerCells = headerCells.More)
			{
				ColumnBindings[i] = CreateBinding(this, headerCells.Text.Trim(), headerCells, GetType());
			}
		}

		protected Binding CreateBinding(Fixture fixture, string name, Parse nameCell, Type targetType)
		{
			return CreateBinding(fixture, name, nameCell, targetType, GetOperationType(name));
		}

		protected Binding CreateBinding(Fixture fixture, string name, Parse nameCell, Type targetType, OperationType operationType)
		{
		    if (!newIdentifier.IsStartOf(name)) {
		        return new Binding(name, nameCell, operationType);
		    }
		    string memberName = name.Substring(4);
		    return new Binding(memberName, new StringCellLeaf(memberName), OperationType.Create);
		}

		private OperationType GetOperationType(string name)
		{
			if (NoOperationIsImpliedBy(name))
				return OperationType.None;
			if (CheckIsImpliedBy(name))
				return OperationType.Check;

			return OperationType.Input;
		}

		private bool NoOperationIsImpliedBy(string name)
		{
			return "".Equals(name.Trim());
		}

		private static Regex checkIsImpliedByRegex = new Regex("(\\?|!|\\(\\))$");
		public virtual bool CheckIsImpliedBy(string name)
		{
			return checkIsImpliedByRegex.IsMatch(name);
		}
	}
}