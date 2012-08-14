// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;
using fitSharp.Samples.Fit;
using fitSharp.Fit.Operators;

namespace fitSharp.Test.NUnit.Fit
{
    public class ParseOperatorTest<ParseOperatorType>
        where ParseOperatorType : CellOperator, ParseOperator<Cell>, new() {

        public ParseOperatorType Parser { get; private set; }

        [SetUp]
        public void SetUp() {
            // Parse operators are stateless, but the processor may be mutated
            // by Parse(), so recreate for every test
            Parser = new ParseOperatorType { Processor = Builder.CellProcessor() };
        }

        protected bool CanParse<T>(string cellContent) {
            return Parser.CanParse(typeof(T), TypedValue.Void, new CellTreeLeaf(cellContent));
        }

        protected T Parse<T>(string cellContent) where T : class {
            return Parse<T>(cellContent, TypedValue.Void);
        }

        protected T Parse<T>(string cellContent, TypedValue instance) where T : class {
            TypedValue result = Parser.Parse(typeof(string), TypedValue.Void, new CellTreeLeaf(cellContent));
            return result.GetValueAs<T>();
        }
    }
}
