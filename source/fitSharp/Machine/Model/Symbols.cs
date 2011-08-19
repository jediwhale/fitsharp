// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Machine.Exception;

namespace fitSharp.Machine.Model {
    public class Symbols {
        public void Save(string id, object instance) { symbols[id.Trim()] = instance; }
        public void Clear() { symbols.Clear(); }
        public bool HasValue(string id) { return symbols.ContainsKey(id.Trim()); }

        public object GetValue(string id) {
            if (HasValue(id)) return symbols[id.Trim()];
            throw new MemoryMissingException<string>(id);
        }

        public object GetValueOrDefault(string id, object defaultValue) {
            return HasValue(id) ? symbols[id.Trim()] : defaultValue;
        }

        readonly Dictionary<string, object> symbols = new Dictionary<string, object>();
    }
}
