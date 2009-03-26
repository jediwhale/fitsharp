// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections;
using System.Collections.Generic;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Application {
    public abstract class ConfigurationList<T>: Copyable, Configurable, IEnumerable<T> where T: class {

        protected ConfigurationList() {
            myList = new List<T>();
        }

        public abstract T Parse(string theValue);
        public abstract ConfigurationList<T> Make();

        public int Count { get { return myList.Count; }}

        public T this[int index] { get { return myList[index]; }}

        public void Insert(string value, string before) {
            T valueItem = Parse(value);
            if (valueItem == null) return;
            T beforeItem = Parse(before);

            if (HasValue(value)) Remove(value);

            for (int i = 0; i < myList.Count; i++) {
                if (beforeItem != null && beforeItem.ToString() == myList[i].ToString()) {
                    InsertItem(i, valueItem);
                    return;
                }
            }

            throw new ArgumentException(string.Format("Key '{0}' not found.", before));
        }

        public void Add(string theValue) {
            T valueItem = Parse(theValue);
            if (valueItem == null) return;

            AddItem(valueItem);
        }

        public void Remove(string theValue) {
            T valueItem = Parse(theValue);
            if (valueItem == null) return;
            foreach (T existingItem in myList) {
                if (existingItem.ToString() == valueItem.ToString()) {
                    RemoveItem(existingItem);
                    return;
                }
            }
        }

        public bool HasValue(string value) {
            T valueItem = Parse(value);

            foreach (T existingItem in myList) {
                if (valueItem != null && existingItem.ToString() == valueItem.ToString()) {
                    return true;
                }
            }
            return false;
        }

        public Copyable Copy() {
            ConfigurationList<T> result = Make();
            result.myList = new List<T>();
            foreach (T item in myList) {
                result.myList.Add(item);
            }
            return result;
        }

        protected virtual void AddItem(T theNewItem) { myList.Add(theNewItem); }
        protected virtual void RemoveItem(T theExistingItem) { myList.Remove(theExistingItem); }
        protected virtual void InsertItem(int before, T newItem) { myList.Insert(before, newItem); }

        protected List<T> myList;

        public IEnumerator<T> GetEnumerator() {
            return myList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}