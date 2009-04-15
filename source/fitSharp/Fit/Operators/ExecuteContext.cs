// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class ExecuteContext {
        public TypedValue SystemUnderTest { get; private set; }
        public TestStatus TestStatus { get; private set; }
        public TypedValue? Target { get; set; }

        public static TypedValue Make(TypedValue target) { return new TypedValue(new ExecuteContext(target)); }
        public static TypedValue Make(TestStatus testStatus, object systemUnderTest) { return new TypedValue(new ExecuteContext(testStatus, systemUnderTest)); }
        public static TypedValue Make(TestStatus testStatus, object systemUnderTest, TypedValue target) { return new TypedValue(new ExecuteContext(testStatus, systemUnderTest, target)); }

        public ExecuteContext(TestStatus testStatus, object systemUnderTest) {
            TestStatus = testStatus;
            SystemUnderTest = new TypedValue(systemUnderTest);
        }

        public ExecuteContext(TypedValue target) {
            Target = target;
        }

        public ExecuteContext(TestStatus testStatus, object systemUnderTest, TypedValue target) {
            TestStatus = testStatus;
            SystemUnderTest = new TypedValue(systemUnderTest);
            Target = target;
        }
    }
}