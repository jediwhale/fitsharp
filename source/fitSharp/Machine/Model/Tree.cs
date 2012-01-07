// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.Linq;

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

    public interface Tree<T> {
        T Value { get; }
        bool IsLeaf { get; }
        ReadList<Tree<T>> Branches { get; }
    }

    public static class TreeExtension {
        public static IEnumerable<T> Leaves<T>(this Tree<T> tree) {
            if (tree.IsLeaf) yield return tree.Value;
            else foreach (Tree<T> branch in tree.Branches) foreach (T leaf in branch.Leaves()) yield return leaf;
        }

        public static TreeWriter<T> Serialize<T>(this Tree<T> tree, TreeWriter<T> serializer) {
            serializer.WritePrefix(tree);
            if (!tree.IsLeaf) {
                for (int i = 0; i < tree.Branches.Count; i++) {
                    serializer.WriteBranch(tree.Branches[i], i);
                }
            }
            serializer.WriteSuffix(tree);
            return serializer;
        }

        public static T Last<T>(this ReadList<T> list) {
            return list[list.Count - 1];
        }

        public static T ValueAt<T>(this Tree<T> tree, int index) {
            return tree.Branches[index].Value;
        }

        public static T ValueAt<T>(this Tree<T> tree, int index1, int index2) {
            return tree.Branches[index1].ValueAt(index2);
        }

        public static Tree<T> Skip<T>(this Tree<T> tree, int skipCount) {
            return new EnumeratedTree<T>(tree.Value, tree.Branches.Skip(skipCount));
        }
    }
}
