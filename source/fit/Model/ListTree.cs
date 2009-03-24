// FitLibrary for FitNesse .NET.
// Copyright (c) 2006 Syterra Software Inc. Released under the terms of the GNU General Public License version 2 or later.
// Based on designs from Fit (c) 2002 Cunningham & Cunningham, Inc., FitNesse by Object Mentor Inc., FitLibrary (c) 2003-2006 Rick Mugridge, University of Auckland, New Zealand.

using System.Collections;

namespace fitlibrary.tree {

	public class ListTree: Tree {

        public ListTree(string theTitle) {
            myTitle = theTitle;
            myChildren = new ArrayList();
        }

        public ListTree(string theTitle, IEnumerable theChildren) {
            myTitle = theTitle;
            myChildren = new ArrayList();
            foreach (Tree child in theChildren) {
                AddChild(child);
            }
        }

        public void AddChild(Tree theChild) {
            myChildren.Add(theChild);
        }

        public override string ToString() {
            return ParseTree.ToParseString(this);
        }

        public string Title {get {return myTitle;}}
        public IEnumerable GetChildren() {return myChildren;}

        private string myTitle;
        private ArrayList myChildren;
    }
}
