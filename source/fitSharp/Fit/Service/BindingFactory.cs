// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {
    public class BindingFactory {
	    static readonly IdentifierName newIdentifier = new IdentifierName("new ");

        readonly CellProcessor processor;
        readonly TargetObjectProvider targetProvider;
        readonly MutableDomainAdapter adapter;

        public BindingFactory(CellProcessor processor, MutableDomainAdapter adapter, TargetObjectProvider targetProvider) {
            this.processor = processor;
            this.adapter = adapter;
            this.targetProvider = targetProvider;
        }

		public BindingOperation Make(Tree<Cell> nameCell) {
		    var name = nameCell.Value.Text.Trim();

			if (NoOperationIsImpliedBy(name))
			    return new NoBinding();

		    var cellOperation = new CellOperationImpl(processor);

			if (CheckIsImpliedBy(name))
			    return new CheckBinding(cellOperation, targetProvider, nameCell);

            var memberName =  processor.ParseTree<Cell, MemberName>(nameCell);
            var member = RuntimeType.FindInstance(targetProvider, memberName, 1);

		    if (member == null && newIdentifier.IsStartOf(name)) {
		        string newMemberName = name.Substring(4);
                return new CreateBinding(processor, adapter, newMemberName);
		    }

		    return new InputBinding(processor, targetProvider, nameCell);
		}

		static bool NoOperationIsImpliedBy(string name) { return name.Length == 0; }

		public static bool CheckIsImpliedBy(string name) {
		    return name.EndsWith("?") || name.EndsWith("!") || name.EndsWith("()");
		}
    }
}
