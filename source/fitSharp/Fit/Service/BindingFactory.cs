﻿// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Service {
    public class BindingFactory {
	    static bool CheckIsImpliedBy(string name) {
		    return name.EndsWith("?") || name.EndsWith("!") || name.EndsWith("()");
		}

        public BindingFactory(CellProcessor processor, MutableDomainAdapter adapter, TargetObjectProvider targetProvider) {
            this.processor = processor;
            this.adapter = adapter;
            this.targetProvider = targetProvider;
        }

		public BindingOperation Make(Tree<Cell> nameCell) {
		    var name = nameCell.Value.Text.Trim();

			if (NoOperationIsImpliedBy(name))
			    return new NoBinding();

			if (CheckIsImpliedBy(name))
			    return new CheckBinding(processor, targetProvider, nameCell);

            var memberName =  processor.ParseTree<Cell, MemberName>(nameCell);
            var member = MemberQuery.FindInstance(processor.FindMember, targetProvider, 
                    new MemberSpecification(memberName, 1));

		    if (!member.IsPresent && name.StartsWith(newIdentifier, StringComparison.OrdinalIgnoreCase)) {
		        return new CreateBinding(processor, adapter, name.Substring(4));
		    }

		    return new InputBinding(processor, targetProvider, nameCell);
		}

	    const string newIdentifier = "new ";

		static bool NoOperationIsImpliedBy(string name) { return name.Length == 0; }

        readonly CellProcessor processor;
        readonly TargetObjectProvider targetProvider;
        readonly MutableDomainAdapter adapter;
    }
}
