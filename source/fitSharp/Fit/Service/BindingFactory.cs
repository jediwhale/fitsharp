// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {
    public class BindingFactory {
	    private static readonly IdentifierName newIdentifier = new IdentifierName("new ");

        private readonly CellProcessor processor;
        private readonly object target;
        private readonly MutableDomainAdapter adapter;

        public BindingFactory(CellProcessor processor, MutableDomainAdapter adapter, object target) {
            this.processor = processor;
            this.adapter = adapter;
            this.target = target;
        }

		public BindingOperation Make(Tree<Cell> nameCell) {
		    string name = nameCell.Value.Text.Trim();

			if (NoOperationIsImpliedBy(name))
			    return new NoBinding();

		    var cellOperation = new CellOperationImpl(processor);

			if (CheckIsImpliedBy(name))
			    return new CheckBinding(cellOperation, target, nameCell);

            string memberName =  processor.ParseTree<Cell, MemberName>(nameCell).ToString();

            RuntimeMember member = RuntimeType.FindInstance(target, memberName, 1);

		    if (member == null && newIdentifier.IsStartOf(name)) {
		        string newMemberName = name.Substring(4);
                return new CreateBinding(cellOperation, adapter, newMemberName);
		    }

		    return new InputBinding(cellOperation, target, nameCell);
		}

		private static bool NoOperationIsImpliedBy(string name)
		{
		    return name.Length == 0;
		}

		private static bool CheckIsImpliedBy(string name)
		{
		    return name.EndsWith("?") || name.EndsWith("!") || name.EndsWith("()");
		}
    }
}
