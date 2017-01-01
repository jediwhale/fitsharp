// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;

namespace fitSharp.Parser {
    public delegate bool TokenBodyFilter(Substring theTokenBody);

    public class Scanner {
	    
        public Scanner(string theInput) {
            myInput = theInput;
            myPrevious = 0;
        }
	    
        public void FindTokenPair(string theFirstToken, string theSecondToken) {
            FindTokenPair(theFirstToken, theSecondToken, ourNullFilter);
        }
	    
        public void FindTokenPair(string theFirstToken, string theSecondToken, TokenBodyFilter theFilter) {
            Leader = Substring.Empty;
            Body = Substring.Empty;
            if (myPrevious >= myInput.Length) return;
            int first, second = 0;
            for (first = myPrevious; first < myInput.Length; first = second + theSecondToken.Length) {
                first = Find(myInput, theFirstToken, first);
                second = Find(myInput, theSecondToken, first + theFirstToken.Length + 1);
                if (second == myInput.Length) {
                    first = second;
                    break;
                }
                int body = first + theFirstToken.Length;
                if (second <= body) continue;
                Body = new Substring(myInput, body, second - body);
                if (theFilter(Body)) break;
            }
            if (first > myPrevious) {
                Leader = new Substring(myInput, myPrevious, first - myPrevious);
            }
            myPrevious = second + theSecondToken.Length;
        }

        static int Find(string theInput, string theSearch, int theStart) {
            return Find(theInput, theSearch, theStart, theInput.Length);
        }

        static int Find(string theInput, string theSearch, int theStart, int theEnd) {
            if (theStart >= theEnd) return theEnd;
            int result = theInput.IndexOf(theSearch, theStart, theEnd - theStart, StringComparison.OrdinalIgnoreCase);
            return result < 0 ? theEnd : result;
        }

        public Substring Body { get; private set; }
        public Substring Leader { get; private set; }

        readonly string myInput;
        int myPrevious;
	    
        static bool NullTokenBodyFilter(Substring theTokenBody) { return true; }
        static readonly TokenBodyFilter ourNullFilter = NullTokenBodyFilter;
    }
}