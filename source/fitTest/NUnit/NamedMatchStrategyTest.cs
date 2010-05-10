// Copyright © 2010 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Data;
using fit.Operators;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture] public class NamedMatchStrategyTest {

        [Test] public void GetsDataRowActuals() {
            var table = new DataTable();
            table.Columns.Add("test", typeof (bool));
            table.Columns.Add("other_test", typeof (string));
            DataRow row = table.NewRow();
            row["test"] = true;
            row["other_test"] = "hi";
            var strategy = new TestStrategy(new Parse("tr", string.Empty,
                new Parse("td", "test", null,
                    new Parse("td", "\"other_test\"", null, null)), null));
            TypedValue[] values = strategy.ActualValues(row);
            Assert.AreEqual(2, values.Length);
            Assert.AreEqual(typeof(bool), values[0].Type);
            Assert.AreEqual(true, values[0].Value);
            Assert.AreEqual(typeof(string), values[1].Type);
            Assert.AreEqual("hi", values[1].Value);
        }

        class TestStrategy: NamedMatchStrategy {
            public TestStrategy(Parse headerRow): base(new Service.Service(), headerRow) {}
            public override bool SurplusAllowed { get { return true; } }
            public override bool IsOrdered { get { return true; } }
        }
    }
}
