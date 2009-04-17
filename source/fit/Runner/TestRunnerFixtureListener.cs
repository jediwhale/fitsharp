// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit;
using fit.Service;
using fitSharp.Fit.Model;
using fitSharp.Machine.Application;

namespace fitnesse.fitserver
{
	public class TestRunnerFixtureListener : FixtureListener
	{
		public TestStatus TestStatus = new TestStatus();
		private bool atStartOfResult = true;
		private PageResult currentPageResult;
		private readonly TestRunner runner;
	    private readonly Configuration configuration;

		public TestRunnerFixtureListener(TestRunner runner, Configuration configuration)
		{
			this.runner = runner;
		    this.configuration = configuration;
		}

		public void TableFinished(Parse table)
		{
            string data = configuration.GetItem<Service>().Parse<StoryTestString>(table).ToString();
			if(atStartOfResult)
			{
				int indexOfFirstLineBreak = data.IndexOf("\n");
				String pageTitle = data.Substring(0, indexOfFirstLineBreak);
				data = data.Substring(indexOfFirstLineBreak + 1);
				currentPageResult = new PageResult(pageTitle);
				atStartOfResult = false;
			}
			currentPageResult.Append(data);
		}

		public void TablesFinished(Parse theTables, TestStatus status)
		{
			currentPageResult.TestStatus = status;
			runner.AcceptResults(currentPageResult);
			atStartOfResult = true;
			TestStatus.TallyCounts(status);
		}
	}
}
