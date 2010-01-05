// Copyright © 2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using System.Collections.Generic;
using fitlibrary.tree;

namespace fit.Test.Double {
    public class SampleTree: Tree {
        public SampleTree(string title, params string[] children) {
            Title = title;
            foreach (string child in children) {
                this.children.Add(new SampleTree(child));
            }
        }
        public string Title { get; private set; }
        public readonly List<Tree> children = new List<Tree>();

        public IEnumerable GetChildren() {
            return children;
        }
    }
}
