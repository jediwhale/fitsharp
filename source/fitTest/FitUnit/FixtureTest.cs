// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Threading;
using fitlibrary;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fit.Test.FitUnit {
    public class FixtureTest: DoFixture {

        public static Parse Tables;

        public void RunTestFixture(string theRows) {
            string html = string.Format("<table><tr><td>fit.Test.FitUnit.TestFixtureFixture</td></tr>{0}</table>", theRows);
            Tables = Parse.ParseFrom(html);
            var configuration = new Configuration(Processor.Configuration);
            configuration.GetItem<Settings>().Runner = "fit.Test.FitUnit.MyRunner";
            new fitSharp.Machine.Application.Runner(new string[] {}, Processor.Configuration, new NullReporter());
        }

        public ApartmentState ExecutionApartmentState { get { return TestFixtureFixture.State; }
        }

        private class MyRunner: Runnable {
            public int Run(string[] commandLineArguments, Configuration configuration, ProgressReporter reporter)
            {
                new StoryTest(Tables).Execute(configuration);
                return 0;
            }
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