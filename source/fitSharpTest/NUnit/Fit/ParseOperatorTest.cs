using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;
using fitSharp.Samples.Fit;
using fitSharp.Fit.Operators;

namespace fitSharp.Test.NUnit.Fit
{
    public class ParseOperatorTest<ParseOperatorType>
        where ParseOperatorType : CellOperator, ParseOperator<Cell>, new() {
        ParseOperatorType parser;

        public ParseOperatorTest() {
            parser = new ParseOperatorType { Processor = Builder.CellProcessor() };
        }

        protected bool CanParse<T>(string cellContent) {
            return parser.CanParse(typeof(T), TypedValue.Void, new CellTreeLeaf(cellContent));
        }

        protected T Parse<T>(string cellContent) where T : class {
            return Parse<T>(cellContent, TypedValue.Void);
        }

        protected T Parse<T>(string cellContent, TypedValue instance) where T : class {
            TypedValue result = parser.Parse(typeof(string), TypedValue.Void, new CellTreeLeaf(cellContent));
            return result.GetValueAs<T>();
        }
    }
}
