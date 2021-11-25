// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class TypeDictionary: Configuration {

        public T ItemOf<T>() {
            return (T)items[typeof(T)];
        }

        public void Add(object item) {
            items[item.GetType()] = item;
        }

        public Memory Copy() {
            var copy = new TypeDictionary();
            foreach (var key in items.Keys) {
                var item = items[key];
                var copyableItem = item as Copyable;
                copy.SetItem(key, copyableItem != null ? copyableItem.Copy() : item);
            }
            return copy;
        }

        public void Apply(Action<object> action) {
            foreach (var item in items.Values) action(item);
        }

        public bool HasItem<T>() {
            return items.ContainsKey(typeof(T));
        }

        public T GetItem<T>() where T: new() {
            if (!HasItem<T>()) {
                items[typeof(T)] = new T();
            }
            return ItemOf<T>();
        }

        public Maybe<T> Item<T>() {
            return HasItem<T>() ? new Maybe<T>(ItemOf<T>()) : Maybe<T>.Nothing;
        }

        public object GetItem(string typeName) {
            return GetItem(new ApplicationUnderTest().FindType(new IdentifierName(typeName)));
        }

        public object GetItem(Type type) {
            if (!items.ContainsKey(type)) {
                items[type] = new BasicProcessor().Create(type.AssemblyQualifiedName).GetValue<object>();
            }
            return items[type];
        }

        void SetItem(Type type, object value) { items[type] = value; }

        readonly Dictionary<Type, object> items = new Dictionary<Type, object>();
    }
}
