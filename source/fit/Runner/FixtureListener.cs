// Modified or written by Object Mentor, Inc. for inclusion with FitNesse.
// Copyright (c) 2002 Cunningham & Cunningham, Inc.
// Released under the terms of the GNU General Public License version 2 or later.

using fitSharp.Fit.Model;

namespace fit
{
	public interface FixtureListener 
	{
		void TableFinished(Parse finishedTable);
		void TablesFinished(Parse theTables, TestStatus status);
	} 
}