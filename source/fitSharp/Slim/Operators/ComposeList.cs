// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections;
using System.Collections.Generic;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;

namespace fitSharp.Slim.Operators {
    public class ComposeList: SlimOperator, ComposeOperator<string> {
        public bool CanCompose(TypedValue instance) {
            //return instance.Type == typeof (List<object>);
            return typeof (IList).IsAssignableFrom(instance.Type);
        }

        public Tree<string> Compose(TypedValue instance) {
            var list = instance.Value as IList ?? new List<object>();
            var tree = new SlimTree();
            foreach (object value in list) {
                tree.AddBranch(Processor.Compose(value));
            }
            return tree;
        }
    }
}