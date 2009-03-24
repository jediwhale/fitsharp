// FitNesse.NET
// Copyright © 2007,2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fit.Test.NUnit;
using fitlibrary;
using fitSharp.Fit.Application;
using fitSharp.Machine.Application;
using fitSharp.Machine.Model;

namespace fit.Test.FitUnit {
    public class ConfigurationTest: DomainAdapter {
        
        public ConfigurationTest() {
            myFolderTestModel = new FolderTestModel();
            myFileTestFixture = new DoFixture(myFolderTestModel);
            myConfiguration = new Configuration();
            myConfiguration.SetItem(typeof(TestList), new TestList());
        }

        public DoFixture FileSetup() {
            return myFileTestFixture;
        }

        public DoFixture Settings() {
            return new DoFixture(myConfiguration.GetItem<Settings>());
        }

        public void SaveCopy() {
            mySavedCopy = new Configuration(myConfiguration);
        }

        public void RestoreCopy() {
            myConfiguration = mySavedCopy;
        }

        public IEnumerable<string> TestList() {
            return myConfiguration.GetItem<TestList>();
        }

        private readonly DoFixture myFileTestFixture;
        private readonly FolderTestModel myFolderTestModel;
        private Configuration mySavedCopy;
        private Configuration myConfiguration;

        public object SystemUnderTest {
            get { return myConfiguration; }
        }

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