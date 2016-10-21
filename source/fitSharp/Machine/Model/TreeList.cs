// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections;
using System.Collections.Generic;

namespace fitSharp.Machine.Model {
    public class TreeList<T>: Tree<T> {
        public TreeList() {}

        public TreeList(T value) { aValue = value; }

        public T Value { get { return aValue; } }
        public bool IsLeaf { get { return list.Count == 0; } }
        public ReadList<Tree<T>> Branches { get { return list; } }
        public void Add(Tree<T> branch) { list.Add(branch); }
        public void Add(T branchValue) { Add(new TreeList<T>(branchValue)); }

        public TreeList<T> AddBranchValue(object value) {
            Add(value as Tree<T> ?? new TreeList<T>((T)value));
            return this;
        }

        public TreeList<T> AddBranch(Tree<T> value) {
            Add(value);
            return this;
        }

        readonly BranchList<T> list = new BranchList<T>();
        readonly T aValue;
    }

    public class BranchList<T>: ReadList<Tree<T>> {
        private readonly List<Tree<T>> branches = new List<Tree<T>>();
        public void Add(Tree<T> item) { branches.Add(item); }
        public IEnumerator<Tree<T>> GetEnumerator() { return branches.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return branches.GetEnumerator(); }
        public Tree<T> this[int index] { get { return branches[index]; } }
        public int Count { get { return branches.Count; } }
    }
}
