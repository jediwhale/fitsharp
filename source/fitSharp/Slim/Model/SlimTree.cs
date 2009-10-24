// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;

namespace fitSharp.Slim.Model {
    public class SlimTree: Tree<string> {
        private readonly TreeList<string> tree =  new TreeList<string>();

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
    }

    public class SlimLeaf: Tree<string> {
        public string Value { get; private set; }

        public SlimLeaf(string value) { Value = value; }

        public bool IsLeaf { get { return true; } }

        public ReadList<Tree<string>> Branches { get { throw new System.NotImplementedException(); } }
    }
}
