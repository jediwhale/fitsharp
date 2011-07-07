// Copyright © 2011 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using fitSharp.Machine.Engine;

namespace fitnesse.fitserver
{
	public class PathParser
	{
		private readonly ArrayList assemblyPaths = new ArrayList();
		private readonly string configFilePath;

		public PathParser(string pathNames)
		{
			if (!string.IsNullOrEmpty(pathNames))
				foreach (string pathName in pathNames.Split(';'))
				{
					if (pathName.EndsWith("config"))
					{
						if (configFilePath == null)
							configFilePath = pathName;
						else
							throw new ArgumentException("Please check the path. There should only be one config file on the path and there are at least two.");
					}
					else
						assemblyPaths.Add(pathName);
				}
		}

		public IList AssemblyPaths
		{
			get { return assemblyPaths; }
		}

		public bool HasConfigFilePath()
		{
			return configFilePath != null;
		}

		public string ConfigFilePath
		{
			get { return configFilePath; }
		}

	    public void AddAssemblies(Configuration configuration) {
	        foreach (string assemblyPath in AssemblyPaths) {
	            if (assemblyPath == "defaultPath") continue;
	            configuration.GetItem<ApplicationUnderTest>().AddAssembly(assemblyPath.Replace("\"", string.Empty));
	        }
	        if (HasConfigFilePath())
	            configuration.GetItem<AppDomainSetup>().ConfigurationFile = configFilePath; //todo: needs to be done before shell.runinnewdomain - runnable has parseargs method?
	    }
	}
}
