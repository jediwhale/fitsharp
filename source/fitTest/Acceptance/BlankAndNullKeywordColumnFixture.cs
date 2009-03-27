// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;

namespace fit.Test.Acceptance {
    public class BlankAndNullKeywordRowFixture : RowFixture
    {
        public override object[] Query()
        {
            StringFixture fixture1 = new StringFixture();
            fixture1.Field = null;
            fixture1.Property = null;
            fixture1.Set(null);
            StringFixture fixture2 = new StringFixture();
            fixture2.Field = "";
            fixture2.Property = "";
            fixture2.Set("");
            StringFixture fixture3 = new StringFixture();
            fixture3.Field = "Joe";
            fixture3.Property = "Joe";
            fixture3.Set("Joe");
            return new object[]{fixture1, fixture2, fixture3};
        }

        public override Type GetTargetClass()
        {
            return typeof(StringFixture);
        }
    }
}