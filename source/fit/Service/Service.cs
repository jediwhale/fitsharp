// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Reflection;
using fit.Operators;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Service {
    public class Service: Processor<Cell>, Copyable {
        public Service() {
            AddOperator(new RuntimeFlow());

            AddOperator(new ComposeDefault());
            AddOperator(new ComposeHtml());
            AddOperator(new ParseDefault());
            AddOperator(new ExecuteDefault());
            AddOperator(new CompareDefault());

            AddOperator(new ExecuteSymbolSave());
            AddOperator(new CompareEmpty());
            AddOperator(new ExecuteEmpty());
            AddOperator(new ExecuteList());
            AddOperator(new CompareNumeric());
            AddOperator(new ParseMemberName());
            AddOperator(new ParseArray());
            AddOperator(new ParseEnum());
            AddOperator(new ParseNullable());
            AddOperator(new ParseBoolean());
            AddOperator(new ParseType());
            AddOperator(new ParseTable());
            AddOperator(new ParseTree());
            AddOperator(new ParseBlank());
            AddOperator(new ParseNull());
            AddOperator(new ParseSymbol());

            AddOperator(new ExecuteError(), 1);
            AddOperator(new ExecuteException(), 1);
            AddOperator(new CompareFail(), 1);

            ApplicationUnderTest = Context.Configuration.GetItem<ApplicationUnderTest>();
            ApplicationUnderTest.AddNamespace("fit");
            ApplicationUnderTest.AddNamespace("fitnesse.handlers");
            ApplicationUnderTest.AddNamespace("fit.Operators");
            ApplicationUnderTest.AddNamespace("fitSharp.Fit.Operators");
            ApplicationUnderTest.AddAssembly(Assembly.GetExecutingAssembly().CodeBase);

            AddMemory<Symbol>();
        }

        public Service(Service other): base(other) {}

        Copyable Copyable.Copy() {
            return new Service(this);
        }
    }
}