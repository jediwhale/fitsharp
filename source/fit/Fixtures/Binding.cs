// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.Model;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fit
{
	public enum OperationType
	{
		Input,
		Check,
        Create,
		None
	}
	public class Binding
	{
		public Binding(string columnHeader, Tree<Cell> headerCell, OperationType operationType)
		{
			MemberName = columnHeader;
			this.operationType = operationType;
		    MemberCell = headerCell;
		}

		public virtual void HandleCell(BoundFixture fixture, Parse cell) {
			HandleCell(fixture, cell, operationType);
		}

		public virtual void HandleCell(BoundFixture fixture, Parse cell, OperationType operationType)
		{
			switch (operationType) {
				case OperationType.Input:
					fixture.CellOperation.Input(fixture.GetTargetObject(), MemberCell, cell);
					break;
				case OperationType.Check:
					fixture.CheckCalled();
					fixture.CellOperation.Check(fixture.GetTargetObject(), MemberCell, cell);
					break;
                case OperationType.Create:
			        fixture.CellOperation.Create(fixture, MemberName, new CellRange(cell, 1));
			        break;
			}
		}

	    public string MemberName { get; private set; }

	    private readonly OperationType operationType;
	    public Tree<Cell> MemberCell { get; private set;}
	}
}