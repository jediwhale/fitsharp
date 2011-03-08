using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using fitSharp.Machine.Application;

namespace fitSharp.Test.NUnit.Application
{
    [TestFixture]
    public class ArgumentParserTest {
        ArgumentParser parser;

        [SetUp]
        public void SetUp() {
            parser = new ArgumentParser();
        }

        [Test]
        public void ParseInvokesHandlerForProvidedSwitchOnly() {
            bool dWasInvoked = false;

            parser.AddArgumentHandler("d", (value) => dWasInvoked = true);
            parser.AddArgumentHandler("e", (value) => Assert.Fail("Argument handler should only be called if switch was on the command-line"));

            parser.Parse(new string[] { "-d" });

            Assert.IsTrue(dWasInvoked);
        }

        [Test]
        public void ParseSilentlyIgnoresSwitchForWhichThereIsNoHandler() {
            parser.AddArgumentHandler("d", (value) => Assert.Fail("Argument handler should only be called if switch was on the command-line."));

            parser.Parse(new string[] { "-ebbot" });
        }

        [Test]
        public void ParseSilentlyIgnoresSwitchWithoutHyphen() {
            parser.AddArgumentHandler("d", (value) => Assert.Fail("Argument handler should only be called if switch is hyphen-prefixed.") );

            parser.Parse(new string[] { "?d" });
        }

        [Test]
        public void ParseSendsArgumentValueToHandler() {
            string argValue = string.Empty;

            parser.AddArgumentHandler("r", (value) => { argValue = value; });

            parser.Parse(new string[] { "-r", "folderrunner" });

            Assert.AreEqual("folderrunner", argValue);
        }

        [Test]
        public void ParseArgumentsWithAndWithoutValues() {
            string argValue = string.Empty;
            string argValue2 = string.Empty;
            bool dWasCalled = false;

            parser.AddArgumentHandler("r", (value) => argValue = value);
            parser.AddArgumentHandler("c", (value) => argValue2 = value);
            parser.AddArgumentHandler("d", (value) => dWasCalled = true);

            parser.Parse(new string[] {"-r", "folderrunner", "-d", "-c", "appconfig.config"});

            Assert.AreEqual("folderrunner", argValue);
            Assert.AreEqual("appconfig.config", argValue2);
            Assert.IsTrue(dWasCalled, "Expected callback for -d switch");
        }

        [Test]
        public void ParseCallsHandlerWithEmptyStringIfThereIsNoValue() {
            string collectedValue;

            parser.AddArgumentHandler("d", (value) => collectedValue = value);

            collectedValue = "something";   
            parser.Parse(new string[] { "-d" });
            Assert.AreEqual(string.Empty, collectedValue);

            collectedValue = "something";
            parser.Parse(new string[] { "-d", "-r", "rvalue" });
            Assert.AreEqual(string.Empty, collectedValue);

            collectedValue = "something";
            parser.Parse(new string[] { "-r", "rvalue", "-d" });
            Assert.AreEqual(string.Empty, collectedValue);
        }
    }
}
