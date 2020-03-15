// Copyright © 2020 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;

namespace fitSharp.Samples {
    public class KeywordTranslator: CellOperator, InvokeSpecialOperator {
        public bool CanInvokeSpecial(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            return translations.ContainsKey(memberName.Name);
        }

        public TypedValue InvokeSpecial(TypedValue instance, MemberName memberName, Tree<Cell> parameters) {
            return new InvokeFlowKeyword {Processor = Processor}
                .InvokeSpecial(instance, new MemberName(translations[memberName.Name]), parameters);
        }

        readonly Dictionary<string, string> translations = new Dictionary<string, string> {
            {"vérifier", "check"}
        };
    }
}
