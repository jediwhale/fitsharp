// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fit.exception;
using fitSharp.Fit.Exception;
using fitSharp.Machine.Model;

namespace fit.Test.Double {
    public class SampleDomainAdapter: DomainAdapter {
        public SampleDomainAdapter() {
            myDomain = new SampleDomain();
        }

        public object SystemUnderTest { get { return myDomain; } }

        public string LowerCaseName {get { return myDomain.Name.ToLower(); }}

        public SampleDomain MakeSampleDomain(string theName) {
            return new SampleDomain(theName);
        }

        public string ThrowAbandon() {
            throw new AbandonStoryTestException();
        }

        public string ThrowAbandonSuite() {
            throw new AbandonSuiteException();
        }

        public string ThrowAbandon(string message) {
            throw new AbandonStoryTestException(message);
        }

        private readonly SampleDomain myDomain;
    }
}
