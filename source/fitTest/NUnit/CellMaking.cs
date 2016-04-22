// Copyright © 2016 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitlibrary.table;
using fitSharp.Machine.Engine;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class CellMaking {
        [Test] public void CellIsMadeWithSimpleString() {
            var service = new Service.Service();
            var cell = (Parse) service.Compose("something");
            Assert.AreEqual("<td><span class=\"fit_grey\">something</span></td>", cell.ToString());
        }

        [Test] public void CellIsMadeWithArray() {
            var service = new Service.Service();
            var cell = (Parse) service.Compose(new [] {"something", "else"});
            Assert.AreEqual("<td><span class=\"fit_grey\">something, else</span></td>", cell.ToString());
        }

        [Test] public void CellIsMadeWithEmbeddedTable() {
            var service = new Service.Service();
            Parse table =
                Parse.ParseFrom("<table><tr><td>11</td><td>12</td></tr><tr><td>21</td><td>22</td></tr></table>");
            var cell = (Parse) service.Compose(new ParseTable(table));
            Assert.AreEqual("\n<td>\n<table>\n<tr><td><span class=\"fit_grey\">11</span></td><td><span class=\"fit_grey\">12</span></td></tr>" +
            "\n<tr><td><span class=\"fit_grey\">21</span></td><td><span class=\"fit_grey\">22</span></td></tr></table></td>", cell.ToString());
        }
    }
}
