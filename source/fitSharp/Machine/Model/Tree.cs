// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

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
    }
}