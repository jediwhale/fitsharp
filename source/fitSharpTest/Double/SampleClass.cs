// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

namespace fitSharp.Test.Double {
    public class SampleClass {
        public static SampleClass Parse(string input) {
            return new SampleClass();
        }

        public void VoidMethod() {}

        public string MethodNoParms() {
            return "samplereturn";
        }

        public string method_with_underscores() {
            return "samplereturn";
        }

        public string MethodWithParms(string input) {
            return "sample" + input;
        }

        public string Property { get; set; }

        public string Field;

        public string this[string input] {
            get { return "index" + input; }
        }

        public string Duplicate;
        private string _duplicate { get { return Duplicate.ToUpper();} }
    }
}