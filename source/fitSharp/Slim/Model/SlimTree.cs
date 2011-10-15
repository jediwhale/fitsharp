// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Model;

namespace fitSharp.Slim.Model {
    public class SlimTree: Tree<string> {
        public static SlimTree Parse(string input) {
            if (!IsList(input)) throw new FormatException("Array format is [a, b, c]");
            return MakeTree(input);
        }

        public string Value { get { return tree.Value; } }

        public bool IsLeaf { get { return false; } }

        public ReadList<Tree<string>> Branches { get { return tree.Branches; } }

        public SlimTree AddBranchValue(object branch) {
            tree.AddBranch(branch as Tree<string> ?? new SlimLeaf(branch.ToString()));
            return this;
        }

        public SlimTree AddBranch(Tree<string> value) {
            tree.AddBranch(value);
            return this;
        }

        static SlimTree MakeTree(string input) {
            var result = new SlimTree();
            var start = 1;
            while (start < input.Length - 1) {
                var endList = FindEndOfNestedList(input, start);
                if (input[start] == '[' && endList > start) {
                    result.AddBranch(MakeTree(input.Substring(start, endList - start + 1)));
                    start = endList + 3;
                }
                else {
                    var end = input.IndexOf(", ", start);
                    if (end < start) end = input.Length - 1;
                    if (end >= start) result.AddBranch(MakeBranch(input.Substring(start, end - start)));
                    start = end + 2;
                }
            }
            return result;
        }

        static int FindEndOfNestedList(string input, int start) {
            var endList = input.IndexOf("], ", start);
            if (endList < 0 && input[input.Length - 2] == ']') endList = input.Length - 2;
            return endList;
        }

        static Tree<string> MakeBranch(string input) {
            if (IsList(input)) return MakeTree(input);
            return new SlimLeaf(input);
        }

        static bool IsList(string input) {
            return input.StartsWith("[") && input.EndsWith("]");
        }

        readonly TreeList<string> tree =  new TreeList<string>();
    }

    public class SlimLeaf: Tree<string> {
        public string Value { get; private set; }

        public SlimLeaf(string value) { Value = value; }

        public bool IsLeaf { get { return true; } }

        public ReadList<Tree<string>> Branches { get { throw new NotImplementedException("Leaf node has no branches."); } }
    }
}
