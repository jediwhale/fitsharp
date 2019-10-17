// Copyright © 2019 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Model;
using fitSharp.Machine.Application;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Fixtures {
    public class ConfigureFixture: Interpreter {
        public void Interpret(CellProcessor processor, Tree<Cell> table) {
            processor.TestStatus.TableCount--;

            table.ValueAt(0, 1).SetAttribute(CellAttribute.Syntax, CellAttributeValue.SyntaxSUT);

            if (table.Branches[0].Branches.Count > 2) {
                var currentRow = table.Branches[0].Skip(2);
                Execute(processor, table.ValueAt(0, 1).Text, currentRow);
            }

            new Traverse<Cell>()
                .Rows.All(row => Execute(processor, table.ValueAt(0, 1).Text, row))
                .VisitTable(table);
        }

        static void Execute(CellProcessor processor, string facilityName, Tree<Cell> currentRow) {
            var facility = processorIdentifier.Matches(facilityName)
                ? processor
                : processor.Memory.GetItem(FindType(processor, facilityName, currentRow.ValueAt(0).Text));

            var result = currentRow.ExecuteMethod(processor, new DoRowSelector(), facility).ThrowExceptionIfNotValid();
            if (result.IsVoid) return;
            currentRow.ValueAt(0).SetAttribute(CellAttribute.Folded, result.ValueString);
        }

        static Type FindType(CellProcessor processor, string facilityName, string action) {
            return processor.ApplicationUnderTest.FindType(new GracefulNameMatcher(ConfigurationNames.TypeName(facilityName, action))).Type;
        }

        static readonly IdentifierName processorIdentifier = new IdentifierName("processor");
    }
}
