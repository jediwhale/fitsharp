// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Application;
using fitSharp.Machine.Model;

namespace fit.Test.Acceptance {
    public class TypeName {
        private readonly Type type;

        public TypeName(string name) {
            try {
                type = Context.Configuration.GetItem<Service.Service>().Create(name, new TreeList<Cell>()).Type;
            }
            catch (Exception) {}
        }

        public string FullName { get { return type.FullName; }}
    }
}