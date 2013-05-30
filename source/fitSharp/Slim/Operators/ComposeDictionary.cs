using System;
using System.Collections.Generic;
using System.Text;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;

namespace fitSharp.Slim.Operators {
    public class ComposeDictionary: SlimOperator, ComposeOperator<string> {
        public bool CanCompose(TypedValue instance) {
            return instance.Type.IsGenericType && instance.Type.GetGenericTypeDefinition() == typeof (Dictionary<,>);
        }

        public Tree<string> Compose(TypedValue instance) {
            var result = new StringBuilder();
            result.AppendFormat("<table class=\"hash_table\">{0}", Environment.NewLine);
            foreach (var keyValue in instance.GetValueAs<Dictionary<string,string>>()) {
                result.AppendFormat("\t<tr class=\"hash_row\">{2}\t\t<td class=\"hash_key\">{0}</td>{2}\t\t<td class=\"hash_value\">{1}</td>{2}\t</tr>{2}",
                    keyValue.Key, keyValue.Value, Environment.NewLine);
            }
            result.Append("</table>");
            return new SlimLeaf(result.ToString());
        }
    }
}
