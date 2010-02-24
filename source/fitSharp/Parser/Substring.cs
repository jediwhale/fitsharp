// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

namespace fitSharp.Parser {
    public struct Substring {
        
        public static Substring Parse(string theInput) {
            return new Substring(theInput, 0, theInput.Length);
        }
        
        public static readonly Substring Empty = new Substring(string.Empty, 0, 0);
	        
        public Substring(string theBase) : this() {
            Base = theBase;
            Start = 0;
            Length = theBase.Length;
        }
        
        public Substring(string theBase, int theStart, int theLength) : this() {
            Base = theBase;
            Start = theStart;
            Length = theLength;
        }
        
        public char this[int theIndex] {
            get {
                if (theIndex >= Length) throw new System.IndexOutOfRangeException();
                return Base[Start + theIndex];
            }
        }
        
        public bool Contains(int theIndex, string theMatch) {
            if (theIndex + theMatch.Length > Length) return false;
            return string.Compare(Base, Start + theIndex, theMatch, 0, theMatch.Length) == 0;
        }

        public string Base { get; private set; }
        public int Start { get; private set; }
        public int End {get { return Start + Length - 1; }}
        public int Length { get; private set; }
        public override string ToString() { return Base.Substring(Start, Length); }
        public override int GetHashCode() { return Base.GetHashCode(); }
        
        public override bool Equals(object theOther) {
            if (theOther is Substring) return Equals((Substring) theOther);
            var otherString = theOther as string;
            return otherString != null && Equals(otherString);
        }
        
        public bool Equals(Substring theOther) {
            return
                (Length == theOther.Length
                     ? string.Compare(Base, Start, theOther.Base, theOther.Start, Length) == 0
                     : false);
        }
        
        public bool Equals(string theOther) {
            return
                (Length == theOther.Length
                     ? string.Compare(Base, Start, theOther, 0, Length) == 0
                     : false);
        }
    }
}
