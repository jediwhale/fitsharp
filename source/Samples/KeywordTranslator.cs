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
