// FitNesse.NET
// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;

namespace fit.Engine {
    public class TestStatus { //todo: name??
        public Counts Counts { get; private set; }
        public bool IsAbandoned { get; set; }
        public CellOperation CellOperation { get; private set; }
        public Hashtable Summary { get; private set; }

        public TestStatus() {
            Counts = new Counts();
            CellOperation = new CellOperation();
            Summary = new Hashtable();
        }
    }
}