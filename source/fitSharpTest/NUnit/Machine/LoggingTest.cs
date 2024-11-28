// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fitSharp.Test.NUnit.Machine {
    [TestFixture] public class LoggingTest {
        Logging logging;

        [SetUp] public void SetUp() {
            logging = new Logging();
        }

        [Test] public void LoggingIsDisabledWhenCreated() {
            logging.WriteItem("stuff");
            ClassicAssert.AreEqual(string.Empty, logging.Show);
        }

        [Test] public void LoggingIsStarted() {
            logging.Start();
            logging.WriteItem("stuff");
            ClassicAssert.AreEqual("<ul><li>stuff</li></ul>", logging.Show);
        }

        [Test] public void MultipleMessagesAreLogged() {
            logging.Start();
            logging.WriteItem("stuff");
            logging.WriteItem("nonsense");
            ClassicAssert.AreEqual("<ul><li>stuff</li><li>nonsense</li></ul>", logging.Show);
        }

        [Test] public void LoggingIsStopped() {
            logging.Start();
            logging.WriteItem("stuff");
            logging.Stop();
            logging.WriteItem("stuff");
            ClassicAssert.AreEqual("<ul><li>stuff</li></ul>", logging.Show);
        }

        [Test] public void SubMessageIsLogged() {
            logging.Start();
            logging.StartWrite("stuff");
            logging.WriteItem("sub");
            logging.EndWrite("nonsense");
            ClassicAssert.AreEqual("<ul><li>stuffnonsense<ul><li>sub</li></ul></li></ul>", logging.Show);
        }

        [Test] public void SubMessageIsNotLoggedWhenStopped() {
            logging.StartWrite("stuff");
            logging.Start();
            logging.WriteItem("sub");
            logging.Stop();
            logging.EndWrite("nonsense");
            ClassicAssert.AreEqual("<ul><li>sub</li></ul>", logging.Show);
        }

        [Test] public void LoggerCanRegisterWhenAdded() {
            logging.Add(new TestLogger());
            ClassicAssert.IsTrue(TestLogger.IsRegistered);
        }

        [Test] public void LoggerReceivesStartEvent() {
            logging.Add(new TestLogger());
            logging.Start();
            ClassicAssert.IsTrue(TestLogger.IsStarted);
        }

        [Test] public void LoggerReceivesStopEvent() {
            logging.Add(new TestLogger());
            logging.Start();
            logging.Stop();
            ClassicAssert.IsFalse(TestLogger.IsStarted);
        }

        [Test] public void LoggerDoesNotReceiveEventsWhenStopped() {
            logging.Add(new TestLogger());
            logging.BeginCell(new CellBase("stuff"));
            ClassicAssert.AreEqual(null, TestLogger.ActiveCell);
        }

        [Test] public void LoggerReceivesBeginCellEvent() {
            logging.Add(new TestLogger());
            logging.Start();
            logging.BeginCell(new CellBase("stuff"));
            ClassicAssert.AreEqual("stuff", TestLogger.ActiveCell.Text);
        }

        [Test] public void LoggerReceivesEndCellEvent() {
            logging.Add(new TestLogger());
            logging.Start();
            var cell = new CellBase("stuff");
            logging.BeginCell(cell);
            logging.EndCell(cell);
            ClassicAssert.AreEqual(null, TestLogger.ActiveCell);
        }

        class TestLogger: Logger {
            public static bool IsRegistered;
            public static bool IsStarted;
            public static Cell ActiveCell;

            public void Register(Logging loggingFacility) {
                IsRegistered = true;
                loggingFacility.StartEvent += OnStart;
                loggingFacility.StopEvent += OnStop;
                loggingFacility.BeginCellEvent += OnBeginCell;
                loggingFacility.EndCellEvent += OnEndCell;
            }

            static void OnStart() { 
                IsStarted = true;
            }

            static void OnStop() {
                IsStarted = false;
            }

            static void OnBeginCell(Cell cell) {
                ActiveCell = cell;
            }

            static void OnEndCell(Cell cell) {
                if (ActiveCell == cell) ActiveCell = null;
            }
        }
    }
}
