// Copyright © 20011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Reflection;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Service {
    public class Service: CellProcessorBase {
        public Service(): this(new Configuration()) {}

        public Service(Configuration configuration): base(configuration, configuration.GetItem<Operators>()) {
            ApplicationUnderTest.AddNamespace("fit");
            ApplicationUnderTest.AddNamespace("fitSharp.Fit.Fixtures");
            ApplicationUnderTest.AddAssembly(Assembly.GetExecutingAssembly().CodeBase);
            configuration.GetItem<Operators>().AddNamespaces(ApplicationUnderTest);
        }

        public void AddCellHandler(string handlerName) {
            ((CellOperators)Operators).AddCellHandler(handlerName);
        }

        public void RemoveCellHandler(string handlerName) {
            ((CellOperators)Operators).RemoveCellHandler(handlerName);
        }

        public override Tree<Cell> MakeCell(string text) {
            return new Parse("td", text, null, null);
        }
    }
}
