// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace fitSharp.Machine.Model {
    public class EnumeratedTree<T>: Tree<T> {
        private readonly EnumeratedList<T> list;
        public T Value { get; private set; }

        public EnumeratedTree(T value, IEnumerable<Tree<T>> branches): this(branches) {
            Value = value;
        }

        public EnumeratedTree(IEnumerable<Tree<T>> branches) {
            list = new EnumeratedList<T>(branches);
        }

        public bool IsLeaf { get { return list.Count == 0; } }
        public ReadList<Tree<T>> Branches { get { return list; } }
        public void Add(Tree<T> branch) { throw new InvalidOperationException("Enumerated tree is read only."); }
    }

    public class EnumeratedList<T>: ReadList<Tree<T>> {
        private readonly IEnumerable<Tree<T>> baseList;

        public EnumeratedList(IEnumerable<Tree<T>> baseList) { this.baseList = baseList; }

        public IEnumerator<Tree<T>> GetEnumerator() { return baseList.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public Tree<T> this[int index] {
            get {
                return baseList.ElementAt(index);
            }
        }

        public int Count {
            get {
                return baseList.Count();
            }
        }
    }
}
