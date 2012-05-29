// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;

namespace fitSharp.Parser {
    public struct Substring {
        
        public static Substring Parse(string theInput) {
            return new Substring(theInput, 0, theInput.Length);
        }
        
        public static readonly Substring Empty = new Substring(string.Empty, 0, 0);
	        
        public Substring(string source) : this() {
            this.source = source;
            start = 0;
            length = source.Length;
        }
        
        public Substring(string source, int theStart, int theLength) : this() {
            this.source = source;
            start = theStart;
            length = theLength;
        }
        
        public char this[int theIndex] {
            get {
                if (theIndex >= length) throw new IndexOutOfRangeException();
                return source[start + theIndex];
            }
        }
        
        public bool ContainsAt(int theIndex, string theMatch) {
            if (theIndex + theMatch.Length > length) return false;
            return string.Compare(source, start + theIndex, theMatch, 0, theMatch.Length) == 0;
        }

        public bool Contains(string match) {
            return source.IndexOf(match, start, length, StringComparison.Ordinal) >= start;
        }

        public bool StartsWith(string prefix) {
            return StartsWith(prefix, StringComparison.Ordinal);
        }

        public bool StartsWith(string prefix, StringComparison comparisonType) {
            if (length < prefix.Length) return false;
            return string.Compare(prefix, 0, source, start, prefix.Length, comparisonType) == 0;
        }

        public Substring TruncateAfter(string match) {
            return new Substring(source, start, source.IndexOf(match, start, length, StringComparison.Ordinal) - start + match.Length);
        }

        public Substring After { get { return new Substring(source, start + length, source.Length - start - length); } }
        public Substring Truncate(int newLength) { return new Substring(source, start, newLength); }
        public Substring Truncate(Substring next) { return new Substring(source, start, next.start - start); }
        public Substring Skip(int skip) { return new Substring(source, start + skip, length - skip); }
        public bool AtEnd { get { return start >= source.Length; } }
        public bool IsEmpty { get { return length == 0; } }

        public override string ToString() { return length == 0 ? string.Empty : source.Substring(start, length); }
        public override int GetHashCode() { return source.GetHashCode(); }
        
        public override bool Equals(object theOther) {
            if (theOther is Substring) return Equals((Substring) theOther);
            var otherString = theOther as string;
            return otherString != null && Equals(otherString);
        }
        
        public bool Equals(Substring theOther) {
            return
                (length == theOther.length
                     ? string.Compare(source, start, theOther.source, theOther.start, length) == 0
                     : false);
        }
        
        public bool Equals(string theOther) {
            return
                (length == theOther.Length
                     ? string.Compare(source, start, theOther, 0, length) == 0
                     : false);
        }

        readonly string source;
        readonly int start;
        readonly int length;
    }
}
