// Copyright © 2011 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Data;
using fit.Operators;
using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fit.Test.NUnit {
    [TestFixture] public class NamedMatchStrategyTest {

        [Test] public void GetsDataRowActualsWithSimpleName() {
            AssertGetsActuals("test", "\"test\"");
        }

        [Test] public void GetsDataRowActualsWithQuotedName() {
            AssertGetsActuals("my_test", "\"my_test\"");
        }

        [Test] public void GetsDataRowActualsWithUnderscoredName() {
            AssertGetsActuals("my_test", "my_test");
        }

        private static void AssertGetsActuals(string columnName, string headerName) {
            var table = new DataTable();
            table.Columns.Add(columnName, typeof (string));
            DataRow row = table.NewRow();
            row[columnName] = "hi";
            var strategy = new TestStrategy(new Parse("tr", string.Empty,
                                                      new Parse("td", headerName, null, null), null));
            TypedValue[] values = strategy.ActualValues(row);
            ClassicAssert.AreEqual(1, values.Length);
            ClassicAssert.AreEqual(typeof(string), values[0].Type);
            ClassicAssert.AreEqual("hi", values[0].Value);
        }

        class TestStrategy: NamedMatchStrategy {
            public TestStrategy(Parse headerRow): base(new Service.Service(), headerRow) {}
            public override bool SurplusAllowed { get { return true; } }
            public override bool IsOrdered { get { return true; } }
        }
    }
}
