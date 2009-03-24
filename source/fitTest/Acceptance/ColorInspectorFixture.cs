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
