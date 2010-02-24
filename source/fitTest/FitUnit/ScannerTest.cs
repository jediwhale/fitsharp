// Copyright © 2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Machine.Model;
using fitSharp.Parser;

namespace fit.Test.FitUnit {
    public class ScannerTest: DomainAdapter {
	    
        public void Input(string theInput) {
            myScanner = new Scanner(theInput);
        }
	    
        public void FindTokenPairFilter(string theFirstToken, string theSecondToken, string theFilterString) {
            ourFilterString = new Substring(theFilterString);
            myScanner.FindTokenPair(theFirstToken, theSecondToken, ourFilter);
        }
	    
        static bool FilterString(Substring theTokenBody) {
            return !ourFilterString.Equals(theTokenBody);
        }

        Scanner myScanner;
	    
        static Substring ourFilterString;
        static readonly TokenBodyFilter ourFilter = FilterString;

        public object SystemUnderTest {
            get { return myScanner; }
        }
    }
}