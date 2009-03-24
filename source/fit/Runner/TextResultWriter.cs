// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.IO;
using fit;

namespace fitnesse.fitserver
{
    public class TextResultWriter: ResultWriter
    {
        private readonly TextWriter _writer;

        public TextResultWriter(string outputFileName, FolderModel theFolderModel)
        {
            if ("stdout".Equals(outputFileName))
                _writer = Console.Out;
            else
                _writer = theFolderModel.MakeWriter(outputFileName);
        }

        public void Close()
        {
            _writer.Flush();
            _writer.Close();
        }

        public void WritePageResult(PageResult results)
        {
            _writer.Write(Protocol.FormatDocument(results + "\n"));
        }

        public void WriteFinalCount(Counts counts)
        {
            _writer.Write(Protocol.FormatCounts(counts));
        }
    }
}
