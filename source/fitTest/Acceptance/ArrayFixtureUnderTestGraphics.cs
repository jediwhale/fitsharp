// Copyright (c) 2004 Rick Mugridge, University of Auckland, NZ
// Released under the terms of the GNU General Public License version 2 or later.
// Modified for C# by Mike Stockdale.

using fitlibrary;
using fitlibrary.tree;

namespace fit.Test.Acceptance {
    public class ArrayFixtureUnderTestGraphics: ArrayFixture {
        public ArrayFixtureUnderTestGraphics():base(
            new GraphicElement[] {
                new GraphicElement(1, new ListTree("a")),
                new GraphicElement(1, new ListTree(string.Empty, new ListTree[]{new ListTree("a")})),
                new GraphicElement(2, new ListTree(string.Empty, new ListTree[]{new ListTree("a"), new ListTree("BB")})),
            })
        {}
    }

    public class GraphicElement {

        public GraphicElement(int i, Tree t) {
            myI = i;
            myTree = t;
        }
        public int i {get {return myI;}}
        public Tree tree {get {return myTree;}}
        private int myI;
        private Tree myTree;
        
    }
}