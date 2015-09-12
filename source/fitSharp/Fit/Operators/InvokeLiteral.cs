// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators {
    public class InvokeLiteral: CellOperator, InvokeOperator<Cell> {
        public bool CanInvoke(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            return parameters.Branches.Count == 1 && literals.ContainsKey(memberName.OriginalName.Trim());
        }

        public TypedValue Invoke(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            return Processor.ParseTree(literals[memberName.OriginalName.Trim()], parameters.Branches[0]);
        }

        static readonly Dictionary<string, Type> literals =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase) {
            {"boolean:", typeof(bool)},
            {"byte:", typeof(byte)},
            {"character:", typeof(char)},
            {"decimal:", typeof(decimal)},
            {"double:", typeof(double)},
            {"float:", typeof(float)},
            {"integer:", typeof(int)},
            {"long:", typeof(long)},
            {"short:", typeof(short)},
            {"string:", typeof(string)}
        };
    }
}
