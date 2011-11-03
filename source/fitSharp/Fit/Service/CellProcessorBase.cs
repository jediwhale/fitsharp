// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {

    public class CellProcessorBase: ProcessorBase<Cell, CellProcessor>, CellProcessor {
        protected readonly CellOperators operators;
	    public TestStatus TestStatus { get; set; }
        public CallStack CallStack { get; private set; }

        public virtual Tree<Cell> MakeCell(string text, IEnumerable<Tree<Cell>> branches) {
            var result = new CellTree(text);
            foreach (var branch in branches) result.AddBranch(branch);
            return result;
        }

        public CellProcessorBase(Memory memory, CellOperators operators): base(memory) {
            TestStatus = new TestStatus();
            CallStack = new CallStack();
	        this.operators = operators;
	        operators.Processor = this;

            Memory.GetItem<Symbols>();
            Memory.GetItem<Procedures>();
        }

        public override TypedValue Parse(System.Type type, TypedValue instance, Tree<Cell> parameters) {
            Cell cell = parameters.Value;
            if (cell != null && cell.ParsedValue.Type == type) return cell.ParsedValue;
            TypedValue parsedValue = base.Parse(type, instance, parameters);
            if (cell != null) cell.ParsedValue = parsedValue;
            return parsedValue;
        }

        protected override Operators<Cell, CellProcessor> Operators {
            get { return operators; }
        }
    }
}
