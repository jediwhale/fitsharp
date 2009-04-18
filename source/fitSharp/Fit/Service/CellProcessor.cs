// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Fit.Operators;

namespace fitSharp.Fit.Service {
    public class CellProcessor: Processor<Cell> {

        public CellProcessor() {
            AddOperator(new ParseDefault());
            AddOperator(new ExecuteDefault());
            AddOperator(new CompareDefault());

            AddOperator(new ExecuteSymbolSave());
            AddOperator(new CompareNumeric());
            AddOperator(new ParseMemberName());
            AddOperator(new ParseEnum());
            AddOperator(new ParseNullable());
            AddOperator(new ParseBoolean());
            AddOperator(new ParseType());
            AddOperator(new ParseBlank());
            AddOperator(new ParseNull());
            AddOperator(new ParseSymbol());

            AddOperator(new ParseArray(), 1);
            AddOperator(new ExecuteError(), 1);
            AddOperator(new ExecuteException(), 1);

            AddMemory<Symbol>();
        }

        public CellProcessor(Processor<Cell> other): base(other) {}
    }
}
