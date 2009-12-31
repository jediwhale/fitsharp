// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary;
using fitlibrary.table;
using System;

namespace fit.Test.Acceptance {
    public class SpecifyFixtureTest: DoFixture {

        public void Process(Parse theTable) {
            Parse table = theTable.More.Parts;
            myMessage = string.Empty;
            myTestValue = string.Empty;
            myResult = null;
            try {
                new SpecifyFixture{Processor = new Service.Service(Processor.Configuration)}.DoTable(table);
                myResult = table;
            }
            catch (Exception e) {
                myMessage = e.Message;
            }
        }

        public bool MatchResult(Table theExpectedTable) {
            string result = myResult.ToString();
            string expected = theExpectedTable.ToString();
            return Trim(result) == Trim(expected);
        }
 
        private static string Trim(string theInput) {
            return theInput.Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
        }

        public string Message() {
            return myMessage;
        }

        public string Test() {
            return myTestValue;
        }

        public void SetTest(string theValue) {
            myTestValue = theValue;
        }

        private string myMessage;
        private Parse myResult;
        private static string myTestValue;
    }
}