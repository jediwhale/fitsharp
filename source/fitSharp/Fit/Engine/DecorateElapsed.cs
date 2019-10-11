// Copyright © 2019 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (https://opensource.org/licenses/cpl1.0.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.IO;
using fitSharp.Machine.Model;

namespace fitSharp.Fit.Engine {
    public class DecorateElapsed: DecorateElement {
        public TypedValue Decorate(CellProcessor processor, Tree<Cell> table, Func<TypedValue> action) {
            var elapsed = new ElapsedTime();
            var result = action();
            var time = elapsed.ToString();
            if (!table.Branches.Last().Branches.Last().Value.Text.StartsWith("elapsed: ")) {
                table.Branches.Last()
                    .Add(processor.Compose(new TypedValue($"elapsed: {time}")));
            }
            return result;
        }
    }
}
