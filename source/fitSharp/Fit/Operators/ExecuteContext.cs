// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public enum ExecuteCommand { Check, Compare, Input, Invoke }

    public class ExecuteContext {
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