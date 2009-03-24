// FitNesse.NET
// Copyright © 2007 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

namespace fit {
    
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
            myLeader = Substring.Empty;
	        myBody = Substring.Empty;
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
                if (second > body) {
                    myBody = new Substring(myInput, body, second - body);
                    if (theFilter(myBody)) break;
                }
            }
            if (first > myPrevious) {
                myLeader = new Substring(myInput, myPrevious, first - myPrevious);
            }
            myPrevious = second + theSecondToken.Length;
        }
	    
        private int Find(string theInput, string theSearch, int theStart) {
            return Find(theInput, theSearch, theStart, theInput.Length);
        }
	    
        private int Find(string theInput, string theSearch, int theStart, int theEnd) {
            if (theStart >= theEnd) return theEnd;
            int result = theInput.IndexOf(theSearch, theStart, theEnd - theStart);
            if (result < 0) return theEnd;
            return result;
        }
        public Substring Body {get { return myBody; }}
        public Substring Leader {get { return myLeader; }}
	    
	    private string myInput;
        private Substring myLeader;
        private Substring myBody;
        private int myPrevious;
	    
	    private static bool NullTokenBodyFilter(Substring theTokenBody) { return true; }
	    private static TokenBodyFilter ourNullFilter = new TokenBodyFilter(NullTokenBodyFilter);
	}
    
}
