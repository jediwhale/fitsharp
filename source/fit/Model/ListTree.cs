// Copyright © 2006,2010 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Collections.Generic;

namespace fitlibrary.tree {

	[Serializable] public class ListTree: Tree {

        public ListTree(string theTitle) {
            Title = theTitle;
            children = new List<Tree>();
        }

        public ListTree(string theTitle, IEnumerable<Tree> theChildren) {
            Title = theTitle;
            children = new List<Tree>();
            foreach (Tree child in theChildren) {
                AddChild(child);
            }
        }

        public void AddChild(Tree theChild) {
            children.Add(theChild);
        }

        public override string ToString() {
            return ParseTree.ToParseString(this);
        }

	    public string Title { get; private set; }
	    public IEnumerable GetChildren() { return children; }

	    private readonly List<Tree> children;
    }
}
