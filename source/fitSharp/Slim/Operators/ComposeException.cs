// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;
using fitSharp.Slim.Exception;

namespace fitSharp.Slim.Operators {
    public class ComposeException: Operator<Service.Service>, ComposeOperator<string> {
        public const string ExceptionResult = "__EXCEPTION__:{0}";

        public bool CanCompose(TypedValue instance) {
            return typeof(System.Exception).IsAssignableFrom(instance.Type);
        }

        public Tree<string> Compose(TypedValue instance) {
            Tree<string> result = null;
            if (TryResult<MemberMissingException>(instance,
                                                  e => string.Format("NO_METHOD_IN_CLASS {0} {1}", e.MemberName, e.Type), ref result)) return result;

            if (TryResult<ConstructorMissingException>(instance,
                                                       e => string.Format("NO_CONSTRUCTOR {0}", e.Type), ref result)) return result;

            if (TryResult<CreateException>(instance,
                                           e => string.Format("COULD_NOT_INVOKE_CONSTRUCTOR {0}", e.Type), ref result)) return result;

            if (TryResult<ParseException<string>>(instance,
                                                  e => string.Format("NO_CONVERTER_FOR_ARGUMENT_NUMBER {0}", e.Type), ref result)) return result;

            if (TryResult<MemoryMissingException<SavedInstance>>(instance,
                                                                 e => string.Format("NO_INSTANCE {0}", e.Key.Id), ref result)) return result;

            if (TryResult<TypeMissingException>(instance,
                                                e => string.Format("NO_CLASS {0}", e.TypeName), ref result)) return result;

            if (TryResult<InstructionException>(instance,
                                                e => string.Format("MALFORMED_INSTRUCTION {0}", List(e.Instruction)), ref result)) return result;

            return IsStopTestException(instance) ? MakeResult(string.Format("ABORT_SLIM_TEST: {0}", instance.Value)) : MakeResult(instance.Value.ToString());
        }

        private static bool IsStopTestException(TypedValue instance) {
            for (var exception = (System.Exception) instance.Value;
                 exception != null;
                 exception = exception.InnerException) {
                if (exception.GetType().Name.Contains("StopTest")) return true;
            }
            return false;
        }

        private static string List(Tree<string> list) {
            var result = new StringBuilder();
            foreach (Tree<string> branch in list.Branches) {
                if (result.Length > 0) result.Append(",");
                result.Append(branch.Value ?? "null");
            }
            return result.ToString();
        }

        private delegate string Format<T>(T exception);

        private static bool TryResult<T>(TypedValue exception, Format<T> formatter, ref Tree<string> result) where T: class {
            var candidateException = exception.Value as T;
            if (candidateException == null) return false;
            result = MakeResult(string.Format("message:<<{0}>> {1}", formatter(candidateException), candidateException));
            return true;
        }

        private static Tree<string> MakeResult(string message) {
            return new TreeLeaf<string>(string.Format(ExceptionResult, message));
        }

        public static bool WasAborted(string result) {
            return result != null && result.StartsWith("__EXCEPTION__:ABORT_SLIM_TEST:");
        }
    }
}