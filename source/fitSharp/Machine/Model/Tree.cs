// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections;
using System.Collections.Generic;

namespace fitSharp.Machine.Model {
    public interface TreeWriter<T> {
        void WritePrefix(Tree<T> tree);
        void WriteBranch(Tree<T> tree, int index);
        void WriteSuffix(Tree<T> tree);
    }

    public interface ReadList<T>: IEnumerable<T> {
        T this[int index] { get; }
        int Count { get; }
    }

    public abstract class Tree<T> {
        public abstract T Value { get; }

        public abstract bool IsLeaf { get; }
        public abstract ReadList<Tree<T>> Branches { get; }

        public IEnumerable<T> Leaves {
            get {
                if (IsLeaf) yield return Value;
                else foreach (Tree<T> branch in Branches) foreach (T leaf in branch.Leaves) yield return leaf;
            }
        }

        public TreeWriter<T> Serialize(TreeWriter<T> serializer) {
            serializer.WritePrefix(this);
            if (!IsLeaf) {
                for (int i = 0; i < Branches.Count; i++) {
                    serializer.WriteBranch(Branches[i], i);
                }
            }
            serializer.WriteSuffix(this);
            return serializer;
        }
    }

    public class TreeList<T>: Tree<T> {
        private readonly BranchList<T> list = new BranchList<T>();
        private readonly T aValue;
        public override T Value { get { return aValue; } }

        public TreeList() {}

        public TreeList(T value) { aValue = value; }

        public override bool IsLeaf { get { return list.Count == 0; } }

        public override ReadList<Tree<T>> Branches { get { return list; } }

        public TreeList<T> AddBranchValue(object value) {
            list.Add(value as Tree<T> ?? new TreeLeaf<T>((T)value));
            return this;
        }

        public TreeList<T> AddBranch(Tree<T> value) {
            list.Add(value);
            return this;
        }

    }

    public class BranchList<T>: ReadList<Tree<T>> {
        private readonly List<Tree<T>> branches = new List<Tree<T>>();
        public void Add(Tree<T> item) { branches.Add(item); }
        public IEnumerator<Tree<T>> GetEnumerator() { return branches.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return branches.GetEnumerator(); }
        public Tree<T> this[int index] { get { return branches[index]; } }
        public int Count { get { return branches.Count; } }
    }

    public class TreeLeaf<T>: Tree<T> {
        private readonly T aValue;
        public override T Value { get { return aValue; } }
        public TreeLeaf(T value) { aValue = value; }
        public override bool IsLeaf { get { return true; } }
        public override ReadList<Tree<T>> Branches { get { throw new InvalidOperationException(); } }
    }
}