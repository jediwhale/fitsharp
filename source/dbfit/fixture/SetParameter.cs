using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace dbfit.fixture
{
    public class SetParameter : fit.Fixture
    {
        public static void SetParameterValue(CellProcessor processor, String name, Object value)
        {
            if (value == null || "null".Equals(value.ToString().ToLower()))
            {
               processor.Get<Symbols>().Save(name, DBNull.Value);
            }
            else if (value != null && value.ToString().StartsWith("<<"))
            {
                string varname = value.ToString().Substring(2);
                if (!name.Equals(varname))
                {
                    processor.Get<Symbols>().Save(name, processor.Get<Symbols>().GetValueOrDefault(varname, null));
                }
            }
            else
                processor.Get<Symbols>().Save(name, value);
        }
        public override void DoTable(fit.Parse table)
        {
            SetParameterValue(Processor, Args[0], GetArgumentInput(1, typeof(object)));
        }
    }
}
