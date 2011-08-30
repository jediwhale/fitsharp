using System;
using fitSharp.Machine.Model;

namespace dbfit.fixture
{
    public class SetParameter : fit.Fixture
    {
        public static void SetParameterValue(Symbols symbols, String name, Object value)
        {
            if (value == null || "null".Equals(value.ToString().ToLower()))
            {
               symbols.Save(name, DBNull.Value);
            }
            else if (value != null && value.ToString().StartsWith("<<"))
            {
                string varname = value.ToString().Substring(2);
                if (!name.Equals(varname))
                {
                    symbols.Save(name, symbols.GetValueOrDefault(varname, null));
                }
            }
            else
                symbols.Save(name, value);
        }
        public override void DoTable(fit.Parse table)
        {
            SetParameterValue(Symbols, Args[0], GetArgumentInput(1, typeof(object)));
        }
    }
}
