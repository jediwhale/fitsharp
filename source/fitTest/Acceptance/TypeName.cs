// FitNesse.NET
// Copyright © 2007,2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Engine;
using fitSharp.Machine.Application;

namespace fit.Test.Acceptance {
    public class TypeName {
        private readonly Type type;

        public TypeName(string name) {
            try {
                type = Context.Configuration.GetItem<Service>().Create(name).Type;
            }
            catch (Exception) {}
        }

        public string FullName { get { return type.FullName; }}
    }
}