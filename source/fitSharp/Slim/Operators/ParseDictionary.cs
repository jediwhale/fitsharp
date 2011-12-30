// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Parser;

namespace fitSharp.Slim.Operators {
    public class ParseDictionary: SlimOperator, ParseOperator<string> {
        public bool CanParse(Type type, TypedValue instance, Tree<string> parameters) {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Dictionary<,>);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<string> parameters) {
            var table = new HtmlTables().Parse(parameters.Value);
            return new TypedValue(table.Branches[0].Branches.Aggregate(
                                      (IDictionary) Activator.CreateInstance(type),
                                      (dictionary, row) => {
                                          dictionary.Add(
                                              Processor.Parse(type.GetGenericArguments()[0], row.Branches[0].Value.Text)
                                                  .Value,
                                              Processor.Parse(type.GetGenericArguments()[1], row.Branches[1].Value.Text)
                                                  .Value);
                                          return dictionary;
                                      }));
        }
    }
}
