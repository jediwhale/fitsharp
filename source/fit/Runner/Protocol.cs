// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Text;
using fitSharp.Fit.Model;

namespace fitnesse.fitserver
{
	public class Protocol
	{
		public static string FormatInteger(int encodeInteger)
		{
			string numberPartOfString = "" + encodeInteger;
			return new String('0', 10 - numberPartOfString.Length) + numberPartOfString;
		}

		public static string FormatDocument(string document)
		{
			return FormatInteger(Encoding.UTF8.GetBytes(document).Length) + document;
		}

		public static String FormatCounts(TestCounts status)
		{
			var builder = new StringBuilder();
			builder.Append(FormatInteger(0));
			builder.Append(FormatInteger(status.GetCount(CellAttributes.RightStatus)));
			builder.Append(FormatInteger(status.GetCount(CellAttributes.WrongStatus)));
			builder.Append(FormatInteger(status.GetCount(CellAttributes.IgnoreStatus)));
			builder.Append(FormatInteger(status.GetCount(CellAttributes.ExceptionStatus)));
			return builder.ToString();
		}

        public static string FormatRequest(string token) {
			return "GET /?responder=socketCatcher&ticket=" + token + " HTTP/1.1\r\n\r\n";
        }

		public static string FormatRequest(string pageName, bool usingDownloadedPaths, string suiteFilter) {
			string request = "GET /" + pageName + "?responder=fitClient";
			if (usingDownloadedPaths)
				request += "&includePaths=yes";
            if (suiteFilter != null)
                request += "&suiteFilter=" + suiteFilter;
			return request + " HTTP/1.1\r\n\r\n";
		}
	}
}
