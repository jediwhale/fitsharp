// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.IO;
using System.Text;
using System.Xml;
using fitSharp.Fit.Model;
using fitSharp.IO;

namespace fitSharp.Fit.Service {
    public class XmlResultWriter: ResultWriter {

        readonly XmlWriter writer;
        readonly FolderModel folderModel;
        readonly TextWriter file;

        public XmlResultWriter(string outputFileName, FolderModel theFolderModel) {
            folderModel = theFolderModel;

            var settings = new XmlWriterSettings {Indent = true};

            if ("stdout".Equals(outputFileName))
                writer = XmlWriter.Create(Console.Out, settings);
            else
            {
                file = folderModel.MakeWriter(outputFileName);
                writer = XmlWriter.Create(file, settings);
            }

            writer.WriteStartDocument();
            writer.WriteStartElement("testResults");
        }

        public void Close() {
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
            if (file != null) file.Close();
        }


        public void WritePageResult(PageResult results) {
            writer.WriteStartElement("result");
            writer.WriteElementString("relativePageName", results.Title);
            writer.WriteStartElement("content");
            writer.WriteCData(EncodeForXml(results.Content));
            writer.WriteEndElement();
            WriteCounts(results.TestCounts, "counts");
            writer.WriteEndElement();
        }

        public void WriteFinalCount(TestCounts summary) {
            WriteCounts(summary, "finalCounts");
        }

        static string EncodeForXml(string input) {
            var output = new StringBuilder(input.Length);
            foreach (char c in input) {
                if (c < 32 && !char.IsWhiteSpace(c)) {
                    output.AppendFormat("&#{0};", (int) c);
                }
                else {
                    output.Append(c);
                }
            }
            return output.ToString();
        }

        void WriteCounts(TestCounts summary, string tag) {
            writer.WriteStartElement(tag);
            writer.WriteElementString("right", summary.GetCount(TestStatus.Right).ToString());
            writer.WriteElementString("wrong", summary.GetCount(TestStatus.Wrong).ToString());
            writer.WriteElementString("ignores", summary.GetCount(TestStatus.Ignore).ToString());
            writer.WriteElementString("exceptions", summary.GetCount(TestStatus.Exception).ToString());
            writer.WriteEndElement();
        }
    }
}
