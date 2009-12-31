// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections;
using fitlibrary;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fit {
    public class ConfigurationSetupFixture: DoFixture {

        public override void DoTable(Parse table) {
            SetSystemUnderTest(Processor.Configuration);
            base.DoTable(table);
        }

        public DoFixture Settings() {
            return new DoFixture(Processor.Configuration.GetItem<Settings>());
        }

        public DoFixture ApplicationUnderTest() { return new DoFixture(Processor.Configuration.GetItem<ApplicationUnderTest>()); }

        //look at
        public DoFixture Service() { return new DoFixture(Processor); }

        public DoFixture GetItem(string type) { return new DoFixture(Processor.Configuration.GetItem(type));}

        public IEnumerable List(string name) {
            return (IEnumerable)Processor.Configuration.GetItem(name);
        }
    }
}
