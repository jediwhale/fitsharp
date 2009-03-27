// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class GridStory {
        public GridFixture MakeGrid(System.Collections.IList theList) {
            List<string[]> result = new List<string[]>();
            foreach (List<string> list in theList) {
                result.Add(list.ToArray());
            }
            return new GridFixture(result.ToArray());
        }

        public List<string> Zero12(string theFirst, string theSecond, string theThird) {
            List<string> result = new List<string>();
            result.Add(theFirst);
            result.Add(theSecond);
            result.Add(theThird);
            return result;
        }
    }
}