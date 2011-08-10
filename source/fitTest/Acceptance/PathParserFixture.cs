// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Linq;
using fitnesse.fitserver;

namespace fit.Test.Acceptance {
    public class PathParserFixture : ColumnFixture
    {
        public string PathString;
        public string[] AssemblyPaths;

        public override void Execute()
        {
            PathParser parser = new PathParser(PathString);
            AssemblyPaths = new string[parser.AssemblyPaths.Count()];
            int index = 0;
            foreach(string assemblyPath in parser.AssemblyPaths)
                AssemblyPaths[index++] = assemblyPath;
        }

    }
}