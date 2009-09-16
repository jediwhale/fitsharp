// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;

namespace fit
{
	public class ActionFixture : Fixture
	{
		protected Parse cells;
		protected static Fixture actor;
		protected object targetObject;

		// Traversal ////////////////////////////////

		public override void DoCells(Parse cells)
		{
			this.cells = cells;
			try
			{
				targetObject = this;
			    CellOperation.Invoke(this, cells);
				targetObject = actor;
			}
			catch (Exception e)
			{
				TestStatus.MarkException(cells, e);
			}
		}

		// Actions //////////////////////////////////

		public virtual void Start()
		{
			actor = LoadFixture(cells.More.Text);
		}

		public virtual void Enter()
		{
			CellOperation.Input(actor.GetTargetObject(), cells.More, cells.More.More);
		}

		public virtual void Press()
		{
			CellOperation.Invoke(actor, cells.More);
		}

		public virtual void Check()
		{
			CellOperation.Check(actor.GetTargetObject(), cells.More, cells.More.More);
		}

		public override object GetTargetObject() {
			return targetObject;
		}
	}
}