// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;

namespace fit
{
	public class ActionFixture : Fixture
	{
		protected Parse cells;
		protected static object actor;
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
		    actor = CellOperation.Create(cells.More.Text.Trim()).Value;
		    var fixture = actor as Fixture;
            if (fixture != null) fixture.Processor = Processor;
		}

		public virtual void Enter()
		{
			CellOperation.Input(GetTarget(actor), cells.More, cells.More.More);
		}

		public virtual void Press()
		{
			CellOperation.Invoke(actor, cells.More, new CellTree(), cells.More);
		}

		public virtual void Check()
		{
			CellOperation.Check(GetTarget(actor), cells.More, cells.More.More);
		}

		public override object GetTargetObject() {
			return targetObject;
		}

        private static object GetTarget(object actor) {
            var target = actor as TargetObjectProvider;
            return target == null ? actor : target.GetTargetObject();
        }
	}
}