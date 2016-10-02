// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;
using fitSharp.Slim.Exception;
using fitSharp.Slim.Model;

namespace fitSharp.Slim.Operators {
    public class ComposeException: SlimOperator, ComposeOperator<string> {
        public const string ExceptionResult = "__EXCEPTION__:{0}";

        public static bool WasAborted(string result) {
            return result != null &&
                (result.StartsWith("__EXCEPTION__:ABORT_SLIM_TEST:") || result.StartsWith("__EXCEPTION__:ABORT_SLIM_SUITE:"));
        }

        public bool CanCompose(TypedValue instance) {
            return typeof(System.Exception).IsAssignableFrom(instance.Type);
        }

        public Tree<string> Compose(TypedValue instance) {
            if (ExceptionIs<MemberMissingException>(instance))
                return FormatException<MemberMissingException>(instance, e => string.Format("NO_METHOD_IN_CLASS {0} {1}", e.MemberName, e.Type));

            if (ExceptionIs<ConstructorMissingException>(instance))
                return FormatException<ConstructorMissingException>(instance, e => string.Format("NO_CONSTRUCTOR {0}", e.Type));

            if (ExceptionIs<MemoryMissingException<string>>(instance))
                return FormatException<MemoryMissingException<string>>(instance, e => string.Format("NO_INSTANCE {0}", e.Key));

            if (ExceptionIs<TypeMissingException>(instance))
                return FormatException<TypeMissingException>(instance, e => string.Format("NO_CLASS {0}", e.TypeName));

            if (ExceptionIs<InstructionException>(instance))
                return FormatException<InstructionException>(instance, e => string.Format("MALFORMED_INSTRUCTION {0}", List(e.Instruction)));

            if (IsStopException(instance, "StopTest")) return MakeResult(string.Format("ABORT_SLIM_TEST: {0}", instance.Value));

            if (IsStopException(instance, "StopSuite")) return MakeResult(string.Format("ABORT_SLIM_SUITE: {0}", instance.Value));

            return MakeResult(instance.Value.ToString());
        }

        static bool IsStopException(TypedValue instance, string name) {
            for (var exception = instance.GetValue<System.Exception>();
                 exception != null;
                 exception = exception.InnerException) {
                if (exception.GetType().Name.Contains(name)) return true;
            }
            return false;
        }

        static string List(Tree<string> list) {
            return list.Branches.Aggregate(new StringBuilder(), (result, branch) => {
                if (result.Length > 0) result.Append(",");
                return result.Append(branch.Value ?? "null");
            }).ToString();
        }

        static bool ExceptionIs<T>(TypedValue exception) where T: class {
            return ExceptionAs<T>(exception) != null;
        }

        static Tree<string> FormatException<T>(TypedValue exception, Func<T, string> format) where T: class {
            var candidateException = ExceptionAs<T>(exception);
            return MakeResult(string.Format("message:<<{0}>> {1}", format(candidateException), candidateException));
        }

        static T ExceptionAs<T>(TypedValue exception) where T: class {
            var baseException = exception.GetValueAs<System.Exception>();
            if (baseException is TargetInvocationException) baseException = baseException.InnerException;
            return baseException as T;
        }

        static Tree<string> MakeResult(string message) {
            return new SlimLeaf(string.Format(ExceptionResult, message));
        }
    }
}
