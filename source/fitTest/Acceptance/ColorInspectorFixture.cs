// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;

namespace fit.Test.Acceptance
{
    public class ColorInspectorFixture : RowFixture
    {
        public override object[] Query()
        {
            Array colorsArray = Enum.GetValues(typeof (Color));
            ArrayList colorsList = new ArrayList(colorsArray);
            return colorsList.ToArray();
        }

        public override Type GetTargetClass()
        {
            return typeof (Color);
        }
    }

    public enum Color
    {
        Red,
        Blue
    }
}
