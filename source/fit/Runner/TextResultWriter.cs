// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.IO;
using fitSharp.Fit.Model;
using fitSharp.Fit.Service;
using fitSharp.IO;

namespace fitnesse.fitserver
{
    public class TextResultWriter: ResultWriter
    {
        private readonly TextWriter _writer;

        public TextResultWriter(string outputFileName, FolderModel theFolderModel) {
            _writer = "stdout".Equals(outputFileName) ? Console.Out : theFolderModel.MakeWriter(outputFileName);
        }

        public void Close()
        {
            _writer.Flush();
            _writer.Close();
        }

        public void WritePageResult(PageResult results)
        {
            _writer.Write(Protocol.FormatDocument(string.Format("{0}\n{1}\n{2}\n", results.Title, results.TestCounts.Description, results.Content)));
        }

        public void WriteFinalCount(TestCounts summary)
        {
            _writer.Write(Protocol.FormatCounts(summary));
        }
    }
}
