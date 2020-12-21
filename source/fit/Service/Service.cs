// Copyright © 2020 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using System.Reflection;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Service {
    public class Service: CellProcessorBase {
        public Service(): this(new TypeDictionary()) {} // todo: test only -> factory

        public Service(Memory memory): base(memory, memory.GetItem<Operators>()) {
            ApplicationUnderTest.AddNamespace("fit");
            ApplicationUnderTest.AddAssembly(TargetFramework.Location(Assembly.GetExecutingAssembly()));
            memory.GetItem<Operators>().AddNamespaces(ApplicationUnderTest);
        }

        public Service(Processor<Cell> other): this(other.Memory.Copy()) {}

        public void AddCellHandler(string handlerName) {
            ((CellOperators)Operators).AddCellHandler(handlerName);
        }

        public void RemoveCellHandler(string handlerName) {
            ((CellOperators)Operators).RemoveCellHandler(handlerName);
        }

        public override Tree<Cell> MakeCell(string text, string tag, IEnumerable<Tree<Cell>> branches) {
            var result = new Parse(new CellBase(text));
            if (!string.IsNullOrEmpty(tag)) result.Value.SetTag(tag);
            if (!string.IsNullOrEmpty(text)) result.Value.SetAttribute(CellAttribute.Body, text);
            foreach (var branch in branches) {
                result.Add(branch);
            }
            return result;
        }
    }
}
