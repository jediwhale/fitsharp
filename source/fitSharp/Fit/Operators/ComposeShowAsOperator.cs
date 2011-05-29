// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Operators
{
    public class ComposeShowAsOperator: CellOperator, ComposeOperator<Cell> {
        private readonly object source;
        private readonly List<CellAttribute> attributes;

        public ComposeShowAsOperator() {}

        public ComposeShowAsOperator(IEnumerable<CellAttribute> attributes, object source) {
            this.attributes = new List<CellAttribute>(attributes);
            this.source = source;
        }

        public bool CanCompose(TypedValue instance) {
            return GetType().IsAssignableFrom(instance.Type);
        }

        public Tree<Cell> Compose(TypedValue instance) {
            var showAs = instance.GetValue<ComposeShowAsOperator>();
            var result = Processor.Compose(showAs.source);
            foreach (var cellAttribute in showAs.attributes) {
                result.Value.SetAttribute(cellAttribute, string.Empty);
            }
            return result;
        }

        public override string ToString() {
            return attributes.Aggregate(new StringBuilder(), (a, s) => a.Append(s)) + source.ToString();
        }
    }
}
