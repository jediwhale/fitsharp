// Modified or written by Object Mentor, Inc. for inclusion with FitNesse.
// Copyright (c) 2002 Cunningham & Cunningham, Inc.
// Released under the terms of the GNU General Public License version 2 or later.
using System;
using System.Text.RegularExpressions;

namespace fit
{
	public abstract class BoundFixture : Fixture
	{
		public bool HasExecuted = false;

		public void CheckCalled()
		{
			if (!HasExecuted) {
				Execute();
				HasExecuted = true;
			}
		}

		public virtual void Execute() {
			// about to process first method call of row
		}

	}
}
