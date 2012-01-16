// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Machine.Exception;

namespace fitSharp.Machine.Model {
    public class StringObjectMemory {
        public void Save(string id, object item) { items[id.Trim()] = item; }
        public void SaveValue<T>(string id, T value) { Save(id, value); }
        public void Clear() { items.Clear(); }
        public bool HasValue(string id) { return items.ContainsKey(id.Trim()); }

        public object GetValue(string id) {
            if (HasValue(id)) return items[id.Trim()];
            throw new MemoryMissingException<string>(id);
        }

        public object GetValueOrDefault(string id, object defaultValue) {
            return HasValue(id) ? items[id.Trim()] : defaultValue;
        }

        readonly Dictionary<string, object> items = new Dictionary<string, object>();
    }
}
