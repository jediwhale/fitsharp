// Copyright © 2019 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (https://opensource.org/licenses/cpl1.0.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.IO.Compression;
using System.Xml.Linq;
using fitSharp.Machine.Application;
using fitSharp.Machine.Model;
using fitSharp.Parser;

namespace fitSharp.Fit.Model {
    public class ExcelStoryTestSource: StoryTestSource {

        public ExcelStoryTestSource(string fileName) {
            this.fileName = fileName;
        }

        public override Tree<Cell> Parse(CellFactory factory, Settings settings) {
            using (var archive = ZipFile.OpenRead(fileName)) {
                var sheet = new ExcelSheet(factory.MakeEmptyCell);
                foreach (var entry in archive.Entries) {
                    if (entry.FullName == "xl/sharedStrings.xml") {
                        sheet.LoadStrings(XDocument.Load(entry.Open()));
                    }
                }
                foreach (var entry in archive.Entries) {
                    if (entry.FullName.StartsWith("xl/worksheets/") && entry.FullName.EndsWith(".xml")) {
                        return sheet.Parse(XDocument.Load(entry.Open()));
                    }
                }
            }
            return factory.MakeEmptyCell(string.Empty);
        }

        readonly string fileName;
    }
}
