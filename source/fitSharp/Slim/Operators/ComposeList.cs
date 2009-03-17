// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Slim.Operators {
    public class ComposeList: ComposeOperator<string> { //todo: handle any enumerable type
        public bool TryCompose(Processor<string> processor, TypedValue instance, ref Tree<string> result) {
            if (instance.Type != typeof (List<object>)) return false;
            var list = instance.Value as List<object> ?? new List<object>();
            var tree = new TreeList<string>();
            foreach (object value in list) {
                tree.AddBranch(processor.Compose(value));
            }
            result = tree;
            return true;
        }
    }
}