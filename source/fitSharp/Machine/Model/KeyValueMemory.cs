// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

namespace fitSharp.Machine.Model {
    public class KeyValueMemory<KeyType, ValueType> {
        public KeyType Id { get; private  set; }
        public ValueType Instance { get; private  set; }

        public KeyValueMemory(KeyType id, ValueType instance): this(id) {
            Instance = instance;
        }

        public KeyValueMemory(KeyType id) {
            Id = id;
        }

        public override bool Equals(object other) { return Equals(other as KeyValueMemory<KeyType, ValueType>); }
        public bool Equals(KeyValueMemory<KeyType, ValueType> other) { return other != null && Id.Equals(other.Id); }
        public override int GetHashCode() { return Id.GetHashCode(); }
    }
}