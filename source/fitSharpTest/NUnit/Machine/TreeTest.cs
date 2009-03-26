// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class TreeTest {
        [Test] public void LeafHasOneLeaf() {
            var result = new StringBuilder();
            foreach (string leaf in new TreeLeaf<string>("a").Leaves) result.Append(leaf);
            Assert.AreEqual("a", result.ToString());
        }

        [Test] public void EmptyTreeHasNoLeaves() {
            var result = new StringBuilder();
            foreach (string leaf in new TreeList<string>().Leaves) result.Append(leaf);
            Assert.AreEqual(string.Empty, result.ToString());
        }

        [Test] public void LeafyTreeHasManyLeaves() {
            var result = new StringBuilder();
            var tree = new TreeList<string>()
                .AddBranchValue("a")
                .AddBranch(new TreeList<string>()
                               .AddBranchValue("b")
                               .AddBranchValue("c"))
                .AddBranchValue("d");
            foreach (string leaf in tree.Leaves) result.Append(leaf);
            Assert.AreEqual("abcd", result.ToString());
        }
    }
}