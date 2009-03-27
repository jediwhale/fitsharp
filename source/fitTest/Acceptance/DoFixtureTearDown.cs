// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class DoFixtureTearDown: DoFixture {
        public string Message = "TearDown Worked.";
        protected void tearDown() {
            throw new ApplicationException(Message);
        }
        public void anException() {
            throw new ApplicationException("ex");
        }

    }
}