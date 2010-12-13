// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class LoggingTest {
        Logging logging;

        [SetUp] public void SetUp() {
            logging = new Logging();
        }

        [Test] public void LoggingIsDisabledWhenCreated() {
            logging.WriteItem("stuff");
            Assert.AreEqual(string.Empty, logging.Show);
        }

        [Test] public void LoggingIsStarted() {
            logging.Start();
            logging.WriteItem("stuff");
            Assert.AreEqual("<ul><li>stuff</li></ul>", logging.Show);
        }

        [Test] public void MultipleMessagesAreLogged() {
            logging.Start();
            logging.WriteItem("stuff");
            logging.WriteItem("nonsense");
            Assert.AreEqual("<ul><li>stuff</li><li>nonsense</li></ul>", logging.Show);
        }

        [Test] public void LoggingIsStopped() {
            logging.Start();
            logging.WriteItem("stuff");
            logging.Stop();
            logging.WriteItem("stuff");
            Assert.AreEqual("<ul><li>stuff</li></ul>", logging.Show);
        }

        [Test] public void SubMessageIsLogged() {
            logging.Start();
            logging.StartWrite("stuff");
            logging.WriteItem("sub");
            logging.EndWrite("nonsense");
            Assert.AreEqual("<ul><li>stuffnonsense<ul><li>sub</li></ul></li></ul>", logging.Show);
        }

        [Test] public void SubMessageIsNotLoggedWhenStopped() {
            logging.StartWrite("stuff");
            logging.Start();
            logging.WriteItem("sub");
            logging.Stop();
            logging.EndWrite("nonsense");
            Assert.AreEqual("<ul><li>sub</li></ul>", logging.Show);
        }
    }
}
