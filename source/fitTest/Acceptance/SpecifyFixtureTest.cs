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
                new SpecifyFixture().DoTable(table);
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
 
        private string Trim(string theInput) {
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