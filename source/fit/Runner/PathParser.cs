// Copyright © 2011 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using System.Linq;

namespace fitnesse.fitserver {
	public class PathParser {

		public PathParser(string pathNames) {
			if (!string.IsNullOrEmpty(pathNames))
				foreach (var pathName in pathNames.Split(';').Where(IsValid)) {
				    assemblyPaths.Add(pathName.Replace("\"", string.Empty));
				}
		}

	    public IEnumerable<string> AssemblyPaths {
			get { return assemblyPaths; }
		}

	    static bool IsValid(string pathName) {
	        return !pathName.EndsWith("config") && pathName != "defaultPath";
	    }

		readonly List<string> assemblyPaths = new List<string>();
	}
}
