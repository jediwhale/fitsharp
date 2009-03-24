// FitNesse.NET
// Copyright © 2007 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

namespace fit {

    public struct Substring {
        
        public static Substring Parse(string theInput) {
            return new Substring(theInput, 0, theInput.Length);
        }
        
        public static readonly Substring Empty = new Substring(string.Empty, 0, 0);
	        
        public Substring(string theBase) {
            myBase = theBase;
            myStart = 0;
            myLength = theBase.Length;
        }
        
        public Substring(string theBase, int theStart, int theLength) {
            myBase = theBase;
            myStart = theStart;
            myLength = theLength;
        }
        
        public char this[int theIndex] {
            get {
                if (theIndex >= myLength) throw new System.IndexOutOfRangeException();
                return myBase[myStart + theIndex];
            }
        }
        
        public bool Contains(int theIndex, string theMatch) {
            if (theIndex + theMatch.Length > myLength) return false;
            return string.Compare(myBase, myStart + theIndex, theMatch, 0, theMatch.Length) == 0;
        }
	        
        public string Base {get { return myBase; }}
        public int Start {get { return myStart; }}
        public int End {get { return myStart + myLength - 1; }}
        public int Length {get { return myLength; }}
        public override string ToString() { return myBase.Substring(myStart, myLength); }
        public override int GetHashCode() { return myBase.GetHashCode(); }
        
        public override bool Equals(object theOther) {
            if (theOther is Substring) return Equals((Substring) theOther);
            string otherString = theOther as string;
            if (otherString != null) return Equals(otherString);
            return false;
        }
        
        public bool Equals(Substring theOther) {
            return
                (myLength == theOther.Length
                ? string.Compare(myBase, myStart, theOther.myBase, theOther.myStart, myLength) == 0
                : false);
        }
        
        public bool Equals(string theOther) {
            return
                (myLength == theOther.Length
                ? string.Compare(myBase, myStart, theOther, 0, myLength) == 0
                : false);
        }
        
        private string myBase;
        private int myStart;
        private int myLength;
    }
}
