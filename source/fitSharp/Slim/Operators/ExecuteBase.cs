// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Slim.Operators {
    public abstract class ExecuteBase: ExecuteOperator<string> {
        private const string defaultResult = "OK";
        private readonly IdentifierName identifier;

        public bool TryExecute(Processor<string> processor, TypedValue instance, Tree<string> parameters, ref TypedValue result) {
            if (!identifier.IsEmpty && (parameters.Branches.Count < 2 || !identifier.Matches(parameters.Branches[1].Value))) return false;
            try {
                result = new TypedValue(ExecuteOperation(processor, parameters));
            }
            catch (System.Exception e) {
                result = new TypedValue(Result(parameters, processor.Compose(e)));
            }
            return true;
        }

        protected ExecuteBase(string identifierName) {
            identifier = new IdentifierName(identifierName);
        }

        protected abstract Tree<string> ExecuteOperation(Processor<string> processor, Tree<string> parameters);

        protected static Tree<string> DefaultResult(Tree<string> parameters) {
            return Result(parameters, defaultResult);
        }

        protected static Tree<string> Result(Tree<string> parameters, Tree<string> result) {
            return new TreeList<string>()
                .AddBranchValue(parameters.Branches[0].Value)
                .AddBranch(result);
        }

        protected static Tree<string> Result(Tree<string> parameters, string result) {
            return new TreeList<string>()
                .AddBranchValue(parameters.Branches[0].Value)
                .AddBranchValue(result);
        }

        protected static Tree<string> ParameterTree(Tree<string> input, int startingIndex) {
            var result = new TreeList<string>(input.Value);
            for (int i = startingIndex; i < input.Branches.Count; i++) {
                result.AddBranch(input.Branches[i]);
            }
            return result;
        }

        protected static TypedValue InvokeMember(Processor<string> processor, Tree<string> parameters, int memberIndex) {
            object target = processor.Load(new SavedInstance(parameters.Branches[memberIndex].Value)).Instance;
            TypedValue result = processor.TryInvoke(new TypedValue(target), parameters.Branches[memberIndex + 1].Value, ParameterTree(parameters, memberIndex + 2));
            result.ThrowExceptionIfNotValid();
            return result;
        }

        public static bool WasAborted(Tree<string> result) {
            return ComposeException.WasAborted(result.Branches[1].Value);
        }
    }
}