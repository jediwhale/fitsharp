// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Document;
using fitSharp.Swim;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Swim {
    [TestFixture] public class ResultPageTest {

        private ResultPage page;
        private const string name1 = "test1";
        private const string name2 = "test2";
        private const string name3 = "test3";
        private const string columnName1 = "colname1";
        private const string columnName2 = "colname2";
        private const string rowName1 = "rowname1";
        private const string rowName2 = "rowname2";

        [Test] public void OneStepAppearsInCell() {
            LoadSteps(new [] {new TestStep {Name = name1, ColumnName = columnName1, RowName = rowName1}});
            AssertCellContainNames(0, 0, new [] {name1});
        }

        [Test] public void StepWithoutColumnDoesNotAppear() {
            LoadSteps(new [] {new TestStep {Name = name1, RowName = rowName1}});
            Assert.AreEqual(0, page.RowCount);
        }

        [Test] public void StepWithoutRowDoesNotAppear() {
            LoadSteps(new [] {new TestStep {Name = name1, ColumnName = columnName1}});
            Assert.AreEqual(0, page.RowCount);
        }

        [Test] public void TwoStepsAppearInCell() {
            LoadSteps(new [] {
                new TestStep {Name = name1, ColumnName = columnName1, RowName = rowName1},
                new TestStep {Name = name2, ColumnName = columnName1, RowName = rowName1}
            });
            AssertCellContainNames(0, 0, new [] {name1, name2});
        }

        [Test] public void StepProvidesColumnHeader() {
            LoadSteps(new [] {new TestStep {Name = name1, ColumnName = columnName1, RowName = rowName1}});
            AssertColumnHeaders(new[] {columnName1});
        }

        [Test] public void StepProvidesRowHeader() {
            LoadSteps(new [] {new TestStep {Name = name1, ColumnName = columnName1, RowName = rowName1}});
            AssertRowHeaders(new[] {rowName1});
        }

        [Test] public void StepsAppearInSeparateColumns() {
            LoadSteps(new [] {
                new TestStep {Name = name1, ColumnName = columnName1, RowName = rowName1},
                new TestStep {Name = name2, ColumnName = columnName2, RowName = rowName1},
                new TestStep {Name = name3, ColumnName = columnName1, RowName = rowName1}
            });
            AssertColumnHeaders(new[] {columnName1, columnName2});
            AssertCellContainNames(0, 0, new [] {name1, name3});
            AssertCellContainNames(0, 1, new [] {name2});
        }

        [Test] public void StepsAppearInSeparateRows() {
            LoadSteps(new [] {
                new TestStep {Name = name1, ColumnName = columnName1, RowName = rowName1},
                new TestStep {Name = name2, ColumnName = columnName1, RowName = rowName2},
                new TestStep {Name = name3, ColumnName = columnName1, RowName = rowName1}
            });
            AssertRowHeaders(new[] {rowName1, rowName2});
            AssertCellContainNames(0, 0, new [] {name1, name3});
            AssertCellContainNames(1, 0, new [] {name2});
        }

        private void AssertCellContainNames(int row, int column, IList<string> names) {
            Assert.IsTrue(row < page.RowCount);
            Assert.IsTrue(column < page.ColumnCount);
            ResultCell cell = page.GetResultCell(row, column);
            Assert.AreEqual(names.Count, cell.NameCount);
            for (int i = 0; i < names.Count; i++) {
                Assert.AreEqual(names[i], cell.GetName(i));
            }
        }

        private void AssertColumnHeaders(IList<string> names) {
            Assert.AreEqual(names.Count, page.ColumnCount);
            for (int i = 0; i < names.Count; i++) {
                Assert.AreEqual(names[i], page.GetColumnHeader(i));
            }
        }

        private void AssertRowHeaders(IList<string> names) {
            Assert.AreEqual(names.Count, page.RowCount);
            for (int i = 0; i < names.Count; i++) {
                Assert.AreEqual(names[i], page.GetRowHeader(i));
            }
        }

        private void LoadSteps(IEnumerable<Step> inputCells) {
            page = new ResultPage(inputCells);
        }

        // step may have 0-n of {title, row, col, name}
        // hasattribute(x) and getattribute(x)
        // can be 'current'


        private class TestStep: Step {
            private readonly Dictionary<StepAttribute, string> attributes = new Dictionary<StepAttribute, string>();

            public string Name { set { attributes[StepAttribute.Name] = value; } }
            public string ColumnName { set { attributes[StepAttribute.Column] = value; } }
            public string RowName { set { attributes[StepAttribute.Row] = value; }  }
            public bool HasAttribute(StepAttribute key) { return attributes.ContainsKey(key); }
            public string GetAttribute(StepAttribute key) { return attributes[key]; }
        }

    }
}
