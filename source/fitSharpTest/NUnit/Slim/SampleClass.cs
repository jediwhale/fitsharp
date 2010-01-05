// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Model;

namespace fitSharp.Test.NUnit.Slim {
    public class SampleClass: DomainAdapter {

        private readonly SampleDomain systemUnderTest = new SampleDomain();

        public object SystemUnderTest { get { return systemUnderTest; } }

        public static int Count;
        public static int MethodCount;

        public SampleClass() {
            Count++;
        }

        public string SampleMethod() {
            MethodCount++;
            return "testresult";
        }

        public string SampleMethodWithParm(string parameter) {
            return "with " + parameter;
        }

        public void SampleVoidMethod() {}

        public void AbortTest() {
            throw new SampleStopTest();
        }

        private class SampleDomain {
            public string DomainMethod() { return "domainstuff"; }
        }
    }

    public class SampleStopTest: ApplicationException {}
}