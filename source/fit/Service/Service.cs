// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Reflection;
using fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Service {
    public class Service: CellProcessorBase, Copyable {
        public Service() {
            AddOperator(new RuntimeFlow());

            AddOperator(new ComposeDefault());

            AddOperator(new ComposeStoryTestString());
            AddOperator(new ParseStoryTestString());

            AddOperator(new ComposeTable());
            AddOperator(new ExecuteList());
            AddOperator(new ParseTable());
            AddOperator(new ParseTree());
            AddOperator(new ParseInterpreter());

            ApplicationUnderTest = Context.Configuration.GetItem<ApplicationUnderTest>();
            ApplicationUnderTest.AddNamespace("fit");
            ApplicationUnderTest.AddNamespace("fitnesse.handlers");
            ApplicationUnderTest.AddNamespace("fit.Operators");
            ApplicationUnderTest.AddNamespace("fitSharp.Fit.Fixtures");
            ApplicationUnderTest.AddNamespace("fitSharp.Fit.Operators");
            ApplicationUnderTest.AddAssembly(Assembly.GetExecutingAssembly().CodeBase);
        }

        public Service(Service other): base(other) {}

        Copyable Copyable.Copy() {
            return new Service(this);
        }
    }
}