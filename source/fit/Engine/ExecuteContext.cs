// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Machine.Model;

namespace fit.Engine {
    public class ExecuteContext {
        public Fixture Fixture { get; private set; }
        public TypedValue? Target { get; set; }

        public static TypedValue Make(TypedValue target) { return new TypedValue(new ExecuteContext(target)); }
        public static TypedValue Make(Fixture fixture) { return new TypedValue(new ExecuteContext(fixture)); }
        public static TypedValue Make(Fixture fixture, TypedValue target) { return new TypedValue(new ExecuteContext(fixture, target)); }

        public ExecuteContext(Fixture fixture) {
            Fixture = fixture;
        }

        public ExecuteContext(TypedValue target) {
            Target = target;
        }

        public ExecuteContext(Fixture fixture, TypedValue target) {
            Fixture = fixture;
            Target = target;
        }
    }
}
