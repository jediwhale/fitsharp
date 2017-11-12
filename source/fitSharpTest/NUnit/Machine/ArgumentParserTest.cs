// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Application;
using NUnit.Framework;
using System.Collections.Generic;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class ArgumentParserTest {
        [SetUp] public void SetUp() {
            parser = new ArgumentParser();
            itemsInvoked = new List<string>();
        }

        [Test] public void ParseInvokesOneRegisteredSwitchHandler() {
            AddSwitchHandler("d");
            AssertParse(new [] {"-d"}, new [] {"d"});
        }

        [Test] public void ParseInvokesSwitchHandlerForProvidedSwitchOnly() {
            AddSwitchHandler("d");
            AddSwitchHandler("e");
            AssertParse(new [] {"-d"}, new [] {"d"});
        }

        [Test] public void ParseSilentlyIgnoresSwitchForWhichThereIsNoHandler() {
            AddSwitchHandler("d");
            AssertParse(new [] {"-ebbot"}, new string[] {});
        }

        [Test] public void ParseSilentlyIgnoresSwitchWithoutHyphen() {
            AddSwitchHandler("d");
            AssertParse(new [] {"?d"}, new string[] {});
        }

        [Test] public void ParseInvokesOneRegisteredArgumentHandler() {
            AddArgumentHandler("r");
            AssertParse(new [] {"-r", "folderrunner"}, new [] {"r,folderrunner"});
        }

        [Test] public void ParseBothArgumentsAndSwitch() {
            AddArgumentHandler("r");
            AddArgumentHandler("c");
            AddSwitchHandler("d");
            AssertParse(
                new [] {"-r", "folderrunner", "-d" , "-c", "appconfig.config"},
                new [] {"r,folderrunner", "d", "c,appconfig.config"});
        }

        [Test] public void ParseArgumentHandlerAsSwitch() {
            AddArgumentHandler("r");
            AddArgumentHandler("c");
            AddSwitchHandler("d");
            AssertParse(new [] {"-r", "-folderrunner", "-d"}, new [] {"d"});
        }

        [Test] public void ParseSwitchHandlerAsArgument() {
            AddSwitchHandler("r");
            AssertParse(new [] {"-r", "folderrunner"}, new string[] {});
        }

        [Test]
        public void CollectsExtras() {
            Assert.AreEqual("x,-a,-b,b", string.Join(",",
                ArgumentParser.Extras(new [] {"x", "-a", "-x", "a", "-x", "-b", "b", "-y"}, new [] {"x", "y"})));
        } 

        ArgumentParser parser;
        List<string> itemsInvoked;

        void AddSwitchHandler(string @switch) {
            parser.AddSwitchHandler(@switch, () => itemsInvoked.Add(@switch));
        }

        void AddArgumentHandler(string @switch) {
            parser.AddArgumentHandler(@switch, argument => itemsInvoked.Add(@switch + "," + argument));
        }

        void AssertParse(string[] input, string[] expectedInvoked) {
            parser.Parse(input);
            Assert.AreEqual(expectedInvoked.Length, itemsInvoked.Count);
            foreach(var expected in expectedInvoked) Assert.IsTrue(itemsInvoked.Contains(expected));
        }
    }
}
