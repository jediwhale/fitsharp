// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Fit.Engine;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class CellOperators: Operators<Cell, CellProcessor> {

        public CellOperators() {
            Add(new ComposeDefault(), 0);
            Add(new ExecuteDefault(), 0);
            Add(new InvokeDataRow<Cell, CellProcessor>(), 0);
            Add(new InvokeSetUpTearDown(), 0);
            Add(new ParseDefault(), 0);
            Add(new ParseMemberName(), 0);
            Add(new ParseBoolean(), 0);
            Add(new ParseDate(), 0);
            Add(new ParseType<Cell, CellProcessor>(), 0);
            Add(new ParseBlank(), 0);
            Add(new ParseNull(), 0);
            Add(new ParseInterpreter(), 0);

            Add(new CheckDefault(), 0);
            Add(new RunTestDefault(), 0);
            Add(new WrapDefault(), 0);
            Add(new CheckEmpty(), 0);
            Add(new CheckSymbolSave(), 0);

            Add(new CompareDefault(), 0);
            Add(new CompareEmpty(), 0);
            Add(new CompareNumeric(), 0);

            Add(new CheckVolatile(), 2);
            Add(new FindMemberQueryable(), 2);
            Add(new InvokeSpecialAction(), 2);
            Add(new ParseArray(), 2);
            Add(new ParseByteArray(), 2);
            Add(new ParseNullArray(), 2);
            Add(new ParseSymbol(), 2);
            Add(new CompareError(), 2);
            Add(new CompareException(), 2);
            Add(new CompareFail(), 2);

            AddNamespaces(createMemory.GetItem<ApplicationUnderTest>());
        }

        public void AddNamespaces(ApplicationUnderTest applicationUnderTest) {
            applicationUnderTest.AddNamespace("fitSharp.Fit.Operators");
            applicationUnderTest.AddNamespace("fit.Operators");
        }

        public void AddCellHandler(string handlerName) {
            if (renames.ContainsKey(handlerName.ToLower())) Add(renames[handlerName.ToLower()]);
        }

        public void RemoveCellHandler(string handlerName) {
            if (renames.ContainsKey(handlerName.ToLower())) Remove(renames[handlerName.ToLower()]);
        }

        private static readonly Dictionary<string, string> renames = new Dictionary<string, string> {
                                                                                                        {"boolhandler", typeof(ParseBoolean).FullName},
                                                                                                        {"emptycellhandler", typeof(CheckEmpty).FullName},
                                                                                                        {"exceptionkeywordhandler", typeof(CompareException).FullName},
                                                                                                        {"nullkeywordhandler", typeof(ParseNull).FullName},
                                                                                                        {"blankkeywordhandler", typeof(ParseBlank).FullName},
                                                                                                        {"errorkeywordhandler", typeof(CompareError).FullName},
                                                                                                        {"endswithhandler", typeof(CompareEndsWith).FullName},
                                                                                                        {"failkeywordhandler", typeof(CompareFail).FullName},
                                                                                                        {"startswithhandler", typeof(CompareStartsWith).FullName},
                                                                                                        {"integralrangehandler", typeof(CompareIntegralRange).FullName},
                                                                                                        {"numericcomparehandler", typeof(CompareNumeric).FullName},
                                                                                                        {"stringhandler", typeof(CompareString).FullName},
                                                                                                        {"substringhandler", typeof(CompareSubstring).FullName},
                                                                                                        {"symbolsavehandler", typeof(CheckSymbolSave).FullName},
                                                                                                        {"symbolrecallhandler", typeof(ParseSymbol).FullName},
                                                                                                        {"regexhandler", typeof(CompareRegEx).FullName},
                                                                                                        {"listhandler", "fit.Operators.ExecuteList"},
                                                                                                        {"parsecellhandler", "fit.Operators.ExecuteParse"},
                                                                                                        {"tablehandler", "fit.Operators.ParseTable"}
                                                                                                    };
    }
}