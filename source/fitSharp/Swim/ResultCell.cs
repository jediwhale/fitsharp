// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Document;

namespace fitSharp.Swim {
    public class ResultCell {
        private readonly List<string> cellNames = new List<string>();
        public int NameCount { get { return cellNames.Count; }}
        public void AddStep(Step step) { cellNames.Add(step.GetAttribute(StepAttribute.Name)); }
        public string GetName(int index) { return cellNames[index]; }
    }
}
