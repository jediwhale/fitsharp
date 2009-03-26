// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

namespace fit
{
	public abstract class BoundFixture : Fixture
	{
		public bool HasExecuted;

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
