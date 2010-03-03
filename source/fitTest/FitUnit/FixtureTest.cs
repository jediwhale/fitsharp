// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Threading;
using fitlibrary;
using fitSharp.Machine.Application;

namespace fit.Test.FitUnit {
    public class FixtureTest: DoFixture {

        public void RunTestFixture(string theRows) {
            string html = string.Format("<table><tr><td>fit.Test.FitUnit.TestFixtureFixture</td></tr>{0}</table>", theRows);
            Parse tables = Parse.ParseFrom(html);
            new StoryTest(tables).Execute(Processor.Configuration);
        }

        public ApartmentState ExecutionApartmentState { get { return TestFixtureFixture.State; }
        }

    }

    public class TestFixtureFixture: DoFixture {

        public static ApartmentState State { get { return myState; }}

        public void SaveState() {
            myState = Thread.CurrentThread.GetApartmentState();
        }

        public void SetState(string theValue) {
            Processor.Configuration.GetItem<Settings>().ApartmentState = theValue;    
        }

        private static ApartmentState myState;
    }
}