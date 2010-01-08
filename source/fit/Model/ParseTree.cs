// Copyright © 2006,2008,2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Text;
using fit;
using fitSharp.Machine.Model;

namespace fitlibrary.tree {

    [Serializable] public class ParseTree: Tree {

        public static string ToParseString(Tree theTree) {
            var result = new StringBuilder();
            if (theTree.Title != null) result.Append(theTree.Title);
            int childCount = 0;
            foreach (Tree child in theTree.GetChildren()) {
                if (childCount == 0) result.Append("<ul>");
                result.AppendFormat("<li>{0}</li>", ToParseString(child));
                childCount++;
            }
            if (childCount > 0) result.Append("</ul>");
            return result.ToString();
        }

        private readonly ListTree tree;

        public ParseTree(Parse theParse) {
            tree = new ListTree(GetTitle(theParse));
            for (Parse child = Root(theParse).Parts; child != null; child = child.More) {
                tree.AddChild(new ParseTree(child));
            }
            myHashCode = theParse.ToString().GetHashCode();
        }

        public string Title { get { return tree.Title; } }
        public IEnumerable GetChildren() { return tree.GetChildren(); }

        public override int GetHashCode() {return myHashCode;}
        public override string ToString() {return tree.ToString();}

        public override bool Equals(object theOther) {
            var other = theOther as Tree;
            if (other == null) return false;
            return Equals(tree, other);
        }

        private static bool Equals(Tree thisTree, Tree otherTree) {
            if (thisTree == null && otherTree == null) return true;
            if (thisTree == null || otherTree == null) return false;
            if (thisTree.Title != otherTree.Title) return false;

            IEnumerator theseChildren = thisTree.GetChildren().GetEnumerator();
            foreach (Tree treeChild in otherTree.GetChildren()) {
                theseChildren.MoveNext();
                if (!Equals(theseChildren.Current, treeChild)) return false;
            }
            return !theseChildren.MoveNext();
        }

        private static string GetTitle(Parse theParse) {
            if (theParse.Parts == null) return theParse.Text;
            return Root(theParse).Leader;
        }

        private static Parse Root(Parse theParse) {
            return (theParse.Parts != null && ourListIdentifier.IsStartOf(theParse.Tag) ? theParse.Parts : theParse);
        }

        private static readonly IdentifierName ourListIdentifier = new IdentifierName("<li");

        private readonly int myHashCode;
    }
}
