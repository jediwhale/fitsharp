// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public enum ExecuteCommand { Check, Invoke }

    public class ExecuteContext {

        public const string CheckCommand = "Check";
        public const string InvokeCommand = "Invoke";

        public TypedValue SystemUnderTest { get; private set; }
        public TypedValue? Target { get; set; }
        public ExecuteCommand Command { get; private set; }

        public static TypedValue Make(ExecuteCommand command, object systemUnderTest) {
            return new TypedValue(new ExecuteContext(command, systemUnderTest));
        }

        public static TypedValue Make(ExecuteCommand command, object systemUnderTest, TypedValue target) {
            return new TypedValue(new ExecuteContext(command, systemUnderTest, target));
        }

        public static TypedValue Make(ExecuteCommand command, TypedValue target) {
            return new TypedValue(new ExecuteContext(command, target));
        }

        public ExecuteContext(ExecuteCommand command, TypedValue target) {
            Command = command;
            Target = target;
        }

        public ExecuteContext(ExecuteCommand command, object systemUnderTest) {
            SystemUnderTest = new TypedValue(systemUnderTest);
            Command = command;
        }

        public ExecuteContext(ExecuteCommand command, object systemUnderTest, TypedValue target) {
            SystemUnderTest = new TypedValue(systemUnderTest);
            Target = target;
            Command = command;
        }
    }
}