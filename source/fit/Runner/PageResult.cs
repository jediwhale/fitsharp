// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Text;
using fit;

namespace fitnesse.fitserver
{
	public class PageResult
	{
		private StringBuilder contentBuffer = new StringBuilder();
		private Counts counts;
		private string title;

		public PageResult(String title)
		{
			this.title = title;
		}

		public String Content
		{
			get { return contentBuffer.ToString(); }
		}

		public void Append(String data)
		{
			contentBuffer.Append(data);
		}

		public String Title
		{
			get { return title; }
		}

		public Counts Counts
		{
			get {return counts;}
			set {counts = value;}
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append(title).Append("\n");
			buffer.Append(counts.ToString()).Append("\n");
			buffer.Append(contentBuffer);
			return buffer.ToString();
		}
	}
}
