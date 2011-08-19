// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Model {
    public class Procedures {
        public Tree<Cell> GetValue(string id) { return procedures[id]; }
        public bool HasValue(string id) { return procedures.ContainsKey(id); }
        public void Save(string id, Tree<Cell> instance) { procedures[id] = instance; }
        readonly Dictionary<string, Tree<Cell>> procedures = new Dictionary<string, Tree<Cell>>();
    }
}
