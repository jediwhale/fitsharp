// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.IO;
using System.Xml;
using fit;

namespace fitnesse.fitserver
{
    public class XmlResultWriter: ResultWriter {

        private readonly XmlWriter _writer;
        private readonly FolderModel _folderModel;
        private readonly TextWriter _file;

        public XmlResultWriter(string outputFileName, FolderModel theFolderModel)
        {
            _folderModel = theFolderModel;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            if ("stdout".Equals(outputFileName))
                _writer = XmlWriter.Create(Console.Out, settings);
            else
            {
                _file = _folderModel.MakeWriter(outputFileName);
                _writer = XmlWriter.Create(_file, settings);
            }

            _writer.WriteStartDocument();
            _writer.WriteStartElement("testResults");
        }

        public void Close()
        {
            _writer.WriteEndElement();
            _writer.WriteEndDocument();
            _writer.Flush();
            _writer.Close();
            if (_file != null) _file.Close();
        }


        public void WritePageResult(PageResult results)
        {
            _writer.WriteStartElement("result");
            _writer.WriteElementString("relativePageName", results.Title);
            _writer.WriteStartElement("content");
            _writer.WriteCData(results.Content);
            _writer.WriteEndElement();
            _writer.WriteStartElement("counts");
            _writer.WriteElementString("right", results.Counts.Right.ToString());
            _writer.WriteElementString("wrong", results.Counts.Wrong.ToString());
            _writer.WriteElementString("ignores", results.Counts.Ignores.ToString());
            _writer.WriteElementString("exceptions", results.Counts.Exceptions.ToString());
            _writer.WriteEndElement();
            _writer.WriteEndElement();
        }

        public void WriteFinalCount(Counts counts)
        {
            _writer.WriteStartElement("finalCounts");
            _writer.WriteElementString("right", counts.Right.ToString());
            _writer.WriteElementString("wrong", counts.Wrong.ToString());
            _writer.WriteElementString("ignores", counts.Ignores.ToString());
            _writer.WriteElementString("exceptions", counts.Exceptions.ToString());
            _writer.WriteEndElement();
        }
    }
}
