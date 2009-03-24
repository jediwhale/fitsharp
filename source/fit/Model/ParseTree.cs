// FitNesse.NET
// Copyright © 2006, 2008 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using System.Text;
using fit;
using fitSharp.Machine.Model;

namespace fitlibrary.tree {

    public class ParseTree: Tree {

        public static string ToParseString(Tree theTree) {
            StringBuilder result = new StringBuilder();
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

        public ParseTree(Parse theParse) {
            myParse = theParse;
            myHashCode = myParse.ToString().GetHashCode();
        }

        public string Title {get {return GetTitle(myParse);}}
        public IEnumerable GetChildren() {
            return new ChildEnumerator(Root(myParse).Parts);
        }

        public override int GetHashCode() {return myHashCode;}
        public override string ToString() {return myParse.ToString();}

        public override bool Equals(object theOther) {
            Tree other = theOther as Tree;
            if (other == null) return false;
            return Equals(myParse, other);
        }

        private bool Equals(Parse theParse, Tree theTree) {
            if (theParse == null && theTree == null) return true;
            if (theParse == null || theTree == null) return false;
            if (GetTitle(theParse) != theTree.Title) return false;
            Parse parseChild = Root(theParse).Parts;
            foreach (Tree treeChild in theTree.GetChildren()) {
                if (!Equals(parseChild, treeChild)) return false;
                parseChild = parseChild.More;
            }
            return (parseChild == null);
        }

        private static string GetTitle(Parse theParse) {
            if (theParse.Parts == null) return theParse.Text;
            return Root(theParse).Leader;
        }

        private static Parse Root(Parse theParse) {
            return (theParse.Parts != null && ourListIdentifier.IsStartOf(theParse.Tag) ? theParse.Parts : theParse);
        }

        private static readonly IdentifierName ourListIdentifier = new IdentifierName("<li");

        private Parse myParse;
        private int myHashCode;

        private class ChildEnumerator: IEnumerable, IEnumerator {

            public ChildEnumerator(Parse theChildren) {
                myChildren = theChildren;
            }

            public IEnumerator GetEnumerator() {return this;}

            public bool MoveNext() {
                if (myCurrent == null) {
                    myCurrent = myChildren;
                }
                else {
                    myCurrent = myCurrent.More;
                }
                return (myCurrent != null);
            }

            public void Reset() {myCurrent = null;}
            public object Current {get {return (myCurrent == null ? null : new ParseTree(myCurrent));}}

            private Parse myChildren;
            private Parse myCurrent;
        }
    }

}
