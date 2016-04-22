// Copyright © 2016 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Data;
using dbfit.fixture;
using fit;
using fit.Service;
using fitSharp.Machine.Model;
using NUnit.Framework;
using TestStatus=fitSharp.Fit.Model.TestStatus;

namespace dbfitTest {
    [TestFixture]
    public class QueryTest {
        [Test]
        public void AccessesColumnWithQuotedUnderscore() {
            AssertQuery("some_column", "\"some_column\"");
        }

        [Test]
        public void AccessesColumnWithQuestionMark() {
            AssertQuery("somecolumn", "somecolumn?");
        }

        [Test]
        public void AccessesColumnWithWithQuotedUnderscoreAndQuestionMark() {
            AssertQuery("some_column", "\"some_column\"?");
        }

        private static void AssertQuery(string columnName, string headerCellName) {
            var dataTable = new DataTable();
            dataTable.Columns.Add("somekey", typeof (string));
            dataTable.Columns.Add(columnName, typeof (string));
            var newRow = dataTable.NewRow();
            newRow["somekey"] = "key";
            newRow[columnName] = "value";
            dataTable.Rows.Add(newRow);
            var fixture = new Query(dataTable, false) {Processor = new Service()};

            var testTable = new CellTree(
                new CellTree("query"),
                new CellTree("somekey", headerCellName),
                new CellTree("key", "value")
                );
            Parse parseTable = Parse.CopyFrom(testTable);
            fixture.DoTable(parseTable);
            Assert.AreEqual(TestStatus.Right, parseTable.At(0, 2, 0).GetAttribute(CellAttribute.Status));
            Assert.AreEqual(TestStatus.Right, parseTable.At(0, 2, 1).GetAttribute(CellAttribute.Status));
        }
    }
}
