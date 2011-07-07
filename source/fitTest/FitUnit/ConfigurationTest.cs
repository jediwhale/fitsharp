// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fitlibrary;
using fitSharp.Fit.Application;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;

namespace fit.Test.FitUnit {
    public class ConfigurationTest: DoFixture {
        
        public ConfigurationTest() {
            myConfiguration = new TypeDictionary();
            myConfiguration.GetItem<TestList>();
            SetSystemUnderTest(myConfiguration);
        }

        public void LoadXml(string xml) { new SuiteConfiguration(myConfiguration).LoadXml(xml); }

        public DoFixture Settings() {
            return new DoFixture(myConfiguration.GetItem<Settings>());
        }

        public void SaveCopy() {
            mySavedCopy = myConfiguration.Copy();
        }

        public void RestoreCopy() {
            myConfiguration = mySavedCopy;
            SetSystemUnderTest(myConfiguration);
        }

        public IEnumerable<string> TestList(string listName) {
            return (TestList)myConfiguration.GetItem(listName);
        }

        public DoFixture GetItem(string type) { return new DoFixture(Processor.Configuration.GetItem(type));}

        private Configuration mySavedCopy;
        private Configuration myConfiguration;
    }

    public class TestList: ConfigurationList<string> {
        public override string Parse(string theValue) {
            return theValue;
        }

        public override ConfigurationList<string> Make() {
            return new TestList();
        }
    }
}