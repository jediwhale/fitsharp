// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class Memory: Copyable {
        private readonly Dictionary<Type, object> memoryBanks;

        public Memory() {
            memoryBanks = new Dictionary<Type, object>();
        }

        public Memory(Memory other) {
            memoryBanks = new Dictionary<Type, object>(other.memoryBanks);
        }

        public void Add<V>() {
            if (!memoryBanks.ContainsKey(typeof(V))) {
                memoryBanks[typeof (V)] = new List<V>();
            }
        }

        public void Store<V>(V newItem) {
            List<V> memory = GetMemory<V>();
            foreach (V item in memory) {
                if (!newItem.Equals(item)) continue;
                memory.Remove(item);
                break;
            }
            memory.Add(newItem);
        }

        public V Load<V>(V matchItem) {
            foreach (V item in GetMemory<V>()) {
                if (matchItem.Equals(item)) return item;
            }
            throw new MemoryMissingException<V>(matchItem);
        }

        public bool Contains<V>(V matchItem) {
            foreach (V item in GetMemory<V>()) {
                if (matchItem.Equals(item)) return true;
            }
            return false;
        }

        public void Clear<V>() {
            GetMemory<V>().Clear();
        }

        private List<V> GetMemory<V>() { return (List<V>) memoryBanks[typeof (V)];}

        public Copyable Copy() {
            return new Memory(this);
        }
    }
}
