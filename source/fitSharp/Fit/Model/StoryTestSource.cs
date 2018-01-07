// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Machine.Model;
using fitSharp.Parser;

namespace fitSharp.Fit.Model {
    public enum StoryTestType {
        Html,
        Text
    }

    public class StoryTestSource {
        public static StoryTestSource FromString(string content) {
            return new StoryTestSource(
                content.Contains(Characters.TextStoryTestBegin) ? StoryTestType.Text : StoryTestType.Html,
                content);
        }

        public static Tree<Cell> MakeTreeCell(CellProcessor processor, string text) {
            return processor.MakeCell(text, string.Empty, new TreeList<Cell>[] {});
        } 

        public StoryTestSource(StoryTestType type, string content) {
            Type = type;
            this.content = content;
        }

        public StoryTestType Type { get; }
        public bool IsEmpty => string.IsNullOrEmpty(content);
        public override string ToString() { return content; }

        readonly string content;
    }
}
