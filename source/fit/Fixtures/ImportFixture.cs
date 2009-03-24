// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
using System;
using fit.Engine;
using fitSharp.Machine.Application;

namespace fit
{
	[Obsolete("Use suite configuration file")]
    public class ImportFixture : Fixture
	{
		public override void DoCell(Parse cell, int columnNumber)
		{
            Context.Configuration.GetItem<Service>().AddNamespace(cell.Text);
		}
	}
}

