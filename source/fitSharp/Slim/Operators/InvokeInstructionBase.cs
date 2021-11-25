// Copyright © 2021 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;

namespace fitSharp.Slim.Operators {
    public abstract class InvokeInstructionBase: SlimOperator, InvokeOperator<string> {
        public bool CanInvoke(TypedValue instance, MemberName memberName, Tree<string> parameters) {
            return instance.Type == typeof(SlimInstruction) && (
                identifier.IsEmpty ||
                   (parameters.Branches.Count > 1 && identifier.Matches(parameters.ValueAt(1))));
        }

        public TypedValue Invoke(TypedValue instance, MemberName memberName, Tree<string> parameters) {
            try {
                return new TypedValue(ExecuteOperation(parameters));
            }
            catch (System.Exception e) {
                return new TypedValue(Result(parameters, Processor.Compose(e)));
            }
        }

        protected InvokeInstructionBase(string identifierName) {
            identifier = new IdentifierName(identifierName);
        }

        protected abstract Tree<string> ExecuteOperation(Tree<string> parameters);

        protected static Tree<string> DefaultResult(Tree<string> parameters) {
            return Result(parameters, defaultResult);
        }

        protected static Tree<string> Result(Tree<string> parameters, Tree<string> result) {
            return new SlimTree()
                .AddBranchValue(parameters.ValueAt(0))
                .AddBranch(result);
        }

        protected static Tree<string> Result(Tree<string> parameters, string result) {
            return new SlimTree()
                .AddBranchValue(parameters.ValueAt(0))
                .AddBranchValue(result);
        }

        protected static Tree<string> ParameterTree(Tree<string> input, int startingIndex) {
            var result = new SlimTree();
            for (var i = startingIndex; i < input.Branches.Count; i++) {
                result.AddBranch(input.Branches[i]);
            }
            return result;
        }

        protected TypedValue InvokeMember(Tree<string> parameters, int memberIndex) {
            var savedInstances = Processor.Get<SavedInstances>();
            var instance = parameters.ValueAt(memberIndex);
            var target = savedInstances.HasValue(instance) ? savedInstances.GetValue(instance) : new NullInstance();
            var result = Processor.Invoke(
                target,
                new MemberNameBuilder(Processor.ApplicationUnderTest).MakeMemberName(parameters.ValueAt(memberIndex + 1)),
                ParameterTree(parameters, memberIndex + 2));
            result.ThrowExceptionIfNotValid();
            return result;
        }
        
        const string defaultResult = "OK";
        readonly IdentifierName identifier;

        class NullInstance {}
    }
}
