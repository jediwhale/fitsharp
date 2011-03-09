using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using fitSharp.Machine.Application;

namespace fitSharp.Test.NUnit.Application
{
    [TestFixture]
    public class ArgumentParserTest
    {
        [Test]
        public void ParseInvokesOneRegisteredSwitchHandler()
        {
            string matched = string.Empty;

           var parser = new ArgumentParser();
           parser.AddSwitchHandler("d", () => matched = "-d");

           parser.Parse(new string[] { "-d"});

           Assert.AreEqual("-d", matched);
        }

        [Test]
        public void ParseInvokesSwitchHandlerForProvidedSwitchOnly()
        {
            bool dWasInvoked = false;
            bool eWasInvoked = false;

            var parser = new ArgumentParser();
            parser.AddSwitchHandler("d", () => dWasInvoked = true);
            parser.AddSwitchHandler("e", () => eWasInvoked = true);

            parser.Parse(new string[] { "-d" });

            Assert.IsTrue(dWasInvoked);
            Assert.IsFalse(eWasInvoked);
        }

        [Test]
        public void ParseSilentlyIgnoresSwitchForWhichThereIsNoHandler()
        {
            bool dWasInvoked = false;

            var parser = new ArgumentParser();
            parser.AddSwitchHandler("d", () => dWasInvoked = true);

            parser.Parse(new string[] { "-ebbot" });

            Assert.IsFalse(dWasInvoked);
        }

        [Test]
        public void ParseSilentlyIgnoresSwitchWithoutHyphen()
        {
            string matched = string.Empty;

            var parser = new ArgumentParser();
            parser.AddSwitchHandler("d", () => matched = "?d");

            parser.Parse(new string[] { "?d" });

            Assert.AreEqual(string.Empty, matched);
        }

        [Test]
        public void ParseInvokesOneRegisteredArgumentHandler()
        {
            string argValue = string.Empty;
            

            var parser = new ArgumentParser();
            parser.AddArgumentHandler("r", (value) => { argValue = value; });

            parser.Parse(new string[] { "-r", "folderrunner" });

            Assert.AreEqual("folderrunner", argValue);
        }

        [Test]
        public void ParseBothArgumentsAndSwitch()
        {
            string argValue = string.Empty;
            string argValue2 = string.Empty;
            string argValue3 = string.Empty;


            var parser = new ArgumentParser();
            parser.AddArgumentHandler("r", (value) => argValue = value);
            parser.AddArgumentHandler("c", (value) => argValue2 = value);
            parser.AddSwitchHandler("d", () => argValue3 = "-d");

            parser.Parse(new string[] {"-r", "folderrunner", "-d" , "-c", "appconfig.config"});

            Assert.AreEqual("folderrunner", argValue);
            Assert.AreEqual("appconfig.config", argValue2);
            Assert.AreEqual("-d", argValue3);
        }
        [Test]
        public void ParseArgumentHandlerAsSwitch()
        {
            string argValue = string.Empty;
            string argValue2 = string.Empty;
            string argValue3 = string.Empty;


            var parser = new ArgumentParser();
            parser.AddArgumentHandler("r", (value) => argValue = value);
            parser.AddArgumentHandler("c", (value) => argValue2 = value);
            parser.AddSwitchHandler("d", () => argValue3 = "-d");

            parser.Parse(new string[] { "-r", "-folderrunner", "-d" });

            Assert.AreEqual(string.Empty, argValue);
            Assert.AreEqual(string.Empty, argValue2);
            Assert.AreEqual("-d", argValue3);
        }

        [Test]
        public void ParseSwitchHandlerAsArgument()
        {
            string argValue = string.Empty;
            string argValue2 = string.Empty;
            string argValue3 = string.Empty;


            var parser = new ArgumentParser();
            parser.AddSwitchHandler("r", () => argValue = "-r");            

            parser.Parse(new string[] { "-r", "folderrunner"});

            Assert.AreEqual(string.Empty, argValue);
            
        }

    }
}
