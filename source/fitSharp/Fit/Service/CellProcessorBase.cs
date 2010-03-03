// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {

    public class CellProcessorBase: ProcessorBase<Cell, CellProcessor>, CellProcessor {
        protected readonly CellOperators operators;
	    public TestStatus TestStatus { get; set; }

        public CellProcessorBase(): this(new Configuration(), new CellOperators()) {}

        protected CellProcessorBase(Configuration configuration, CellOperators operators): base(configuration) {
            TestStatus = new TestStatus();
	        this.operators = operators;
	        operators.Processor = this;

            AddMemory<Symbol>();
            AddMemory<Procedure>();
        }

        protected override Operators<Cell, CellProcessor> Operators {
            get { return operators; }
        }
    }
}
