// Copyright © 2010 Syterra Software Inc. Includes work Copyright (C) Gojko Adzic 2006-2008 http://gojko.net
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Operators;

namespace dbfit.util
{
    public class Options
    {
        private static bool fixedLengthStringParsing;
        private static bool bindSymbols = true;

        public static void reset()
        {
            fixedLengthStringParsing = false;
            bindSymbols = true;
        }
        public static bool IsFixedLengthStringParsing()
        {
            return fixedLengthStringParsing;
        }
        public static bool ShouldBindSymbols()
        {
            return bindSymbols;
        }

        public static void SetOption(CellProcessor processor, String name, String value)
        {
            String normalname = NameNormaliser.NormaliseName(name);
            if ("fixedlengthstringparsing".Equals(normalname))
            {
                fixedLengthStringParsing = Boolean.Parse(value);
                if (fixedLengthStringParsing)
                    processor.AddOperator(typeof(ParseQuotedString).FullName);
                else
                    processor.RemoveOperator(typeof(ParseQuotedString).FullName);
            }
            else if ("bindsymbols".Equals(normalname))
            {
                bindSymbols = Boolean.Parse(value);
            }
            else throw new ApplicationException("Unsupported option" + name);
        }
    }
}
