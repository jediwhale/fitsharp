// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (https://opensource.org/licenses/cpl1.0.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Engine;
using fitSharp.Machine.Model;
using fitSharp.Parser;

namespace fitSharp.Fit.Model {
    public abstract class StoryTestSource {
        public static StoryTestSource FromString(string content) {
            if (content.Contains(Characters.TextStoryTestBegin)) {
                return new TextStoryTestSource(content);
            }
            return new HtmlStoryTestSource(content);
        }

        public static StoryTestSource FromFile(StoryPageName name, string content) {
            if (name.IsExcelSpreadsheet) {
                return new ExcelStoryTestSource(name.Name);
            }
            return FromString(content);
        }

        public static Tree<Cell> MakeTreeCell(CellProcessor processor, string text) {
            return processor.MakeCell(text, string.Empty, new TreeList<Cell>[] {});
        }

        public abstract Tree<Cell> Parse(CellProcessor processor);
    }
}
