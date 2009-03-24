// FitNesse.NET
// Copyright © 2007,2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Configuration;
using System.Threading;
using fitlibrary;

namespace fit.Test.Acceptance {
    public class InfoFixture: DoFixture {

        public ApartmentState ApartmentState() {
            return Thread.CurrentThread.GetApartmentState();
        }

        public int Result {
            get { return myResult; }
            set {myResult = value; }
        }

        public string ConfigValue(string theKey) {
            return ConfigurationManager.AppSettings[theKey];
        }

        public string WriteAMessage(string theMessage) {
            Console.WriteLine(theMessage);
            return theMessage;
        }

        public void DoStuff() {}

        public void ShowHtml(Parse theCells) {
            theCells.More = new Parse("td", "hello world", null, null);
        }

        private int myResult;
    }
}