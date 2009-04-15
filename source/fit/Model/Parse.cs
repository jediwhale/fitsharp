// FitNesse.NET
// Copyright © 2007-2009 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.IO;
using System.Text;
using System.Web;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fit
{
	public class Parse: Tree<Cell>, Cell
	{
	    private string tag;
		private string body;
	    private string originalBody;

        private CellAttributes Attributes { get; set; }

	    public Parse More { get; set; }
	    public Parse Parts { get; set; }
	    public string Leader { get; private set; }
	    public string Trailer { get; set; }
	    public string End { get; private set; }

	    public string Tag {
            get {
	            int space = tag.IndexOf(' ');
	            if (space < 0) space = tag.Length - 1;
	            return !Attributes.HasAttribute(CellAttributes.StatusKey) ? tag : string.Format("{0} class=\"{1}\"{2}", tag.Substring(0, space), GetAttribute(CellAttributes.StatusKey), tag.Substring(space));
            }
        }

	    public string Body {
            get {
                string result = body;
                if (Attributes.HasAttribute(CellAttributes.InformationPrefixKey)) {
                    result = string.Format("<span class=\"fit_grey\">{0}</span>{1}", GetAttribute(CellAttributes.InformationPrefixKey), result);
                }
                if (Attributes.HasAttribute(CellAttributes.InformationSuffixKey)) {
                    result = string.Format("{0}<span class=\"fit_grey\">{1}</span>", result, GetAttribute(CellAttributes.InformationSuffixKey));
                }
                if (Attributes.HasAttribute(CellAttributes.ActualKey)) {
                    result += Label("expected") + "<hr />" + HttpUtility.HtmlEncode(GetAttribute(CellAttributes.ActualKey)) + Label("actual");
                }
                if (Attributes.HasAttribute(CellAttributes.ExceptionKey)) {
                    result += "<hr /><pre><div class=\"fit_stacktrace\">" + GetAttribute(CellAttributes.ExceptionKey) + "</div></pre>";
                }
                if (Attributes.HasAttribute(CellAttributes.LabelKey)) {
                    result += Label(GetAttribute(CellAttributes.LabelKey));
                }
                return result;
            }
        }

		public void SetBody(string val)
		{
			body = val;
		}

		public virtual void AddToBody(string text)
		{
		    AddToAttribute(CellAttributes.InformationSuffixKey, text, CellAttributes.SuffixFormat);
		}

	    private Parse() { Attributes = new CellAttributes(); }

        //added
        public Parse(string theTag, string theEnd, string theLeader, string theBody, Parse theParts): this() {
            tag = theTag;
            End = theEnd;
            Leader = theLeader;
            body = theBody;
            originalBody = theBody;
            Parts = theParts;
        }

		public Parse(string tag, string body, Parse parts, Parse more): this()
		{
			this.Leader = "\n";
			this.tag = "<" + tag + ">";
			this.body = body;
			originalBody = body;
			this.End = "</" + tag + ">";
			this.Trailer = "";
			this.Parts = parts;
			this.More = more;
		}

		public static string[] Tags = {"table", "tr", "td"};

		public Parse(string text) : this(text, Tags, 0, 0)
		{}

		public Parse(string text, string[] tags) : this(text, tags, 0, 0)
		{}

	    public void SetAttribute(string key, string value) {
            Attributes.SetAttribute(key, value);
        }

	    public void AddToAttribute(string key, string value, string format) {
	        Attributes.AddToAttribute(key, value, format);
	    }

	    public string GetAttribute(string key) {
            return Attributes.GetAttribute(key);
        }

	    private static string Label(string text) {
			return " <span class=\"fit_label\">" + text + "</span>";
		}

		private static string Substring(string text, int startIndexInclusive, int endIndexExclusive)
		{
			return text.Substring(startIndexInclusive, endIndexExclusive - startIndexInclusive);
		}

		private static int ProtectedIndexOf(string text, string searchValue, int offset, string tag)
		{
			int result = text.IndexOf(searchValue, offset);
			if (result < 0)
				throw new ApplicationException("Can't find tag: " + tag);
			else
				return result;
		}

		public Parse(string text, string[] tags, int level, int offset): this()
		{
			string lc = text.ToLower();
			string target = tags[level].ToLower();

			int startTag = ProtectedIndexOf(lc, "<" + target, 0, target);
			int endTag = ProtectedIndexOf(lc, ">", startTag, target) + 1;
			int startEnd = ProtectedIndexOf(lc, "</" + target, endTag, target);
			int endEnd = ProtectedIndexOf(lc, ">", startEnd, target) + 1;
			int startMore = lc.IndexOf("<" + target, endEnd);

			Leader = Substring(text, 0, startTag);
			tag = Substring(text, startTag, endTag);
			body = Substring(text, endTag, startEnd);
			originalBody = body;
			End = Substring(text, startEnd, endEnd);
			Trailer = text.Substring(endEnd);

			if (level + 1 < tags.Length)
			{
				Parts = new Parse(body, tags, level + 1, offset + endTag);
				body = null;
			    originalBody = null;
			}

			if (startMore >= 0)
			{
				More = new Parse(Trailer, tags, level, offset + endEnd);
				Trailer = null;
			}
		}

		public virtual int Size
		{
			get { return More == null ? 1 : More.Size + 1; }
		}

		public virtual Parse Last
		{
			get { return More == null ? this : More.Last; }
		}

		public virtual Parse Leaf
		{
			get { return Parts == null ? this : Parts.Leaf; }
		}

		public virtual Parse At(int i)
		{
			return i == 0 || More == null ? this : More.At(i - 1);
		}

		public virtual Parse At(int i, int j)
		{
			return At(i).Parts.At(j);
		}

		public virtual Parse At(int i, int j, int k)
		{
			return At(i, j).Parts.At(k);
		}

		public virtual string Text
		{
			get
			{
				return HtmlToText(originalBody);
			}
		}

	    public static string HtmlToText(string theHtml) {
	        return new HtmlString(theHtml).ToPlainText();
        }

        public static string UnFormat(string s)
        {
            return StripMarkup(s);
        }

        private static string StripMarkup(string s)
        {
            int i = 0, j;
            while ((i = s.IndexOf('<', i)) >= 0)
            {
                if ((j = s.IndexOf('>', i + 1)) > 0)
                    s = Substring(s, 0, i) + s.Substring(j + 1);
                else
                    break;
            }
            return s;
        }
	    
	    public static string UnEscape(string s)
		{
			int i = -1, j;
			while ((i = s.IndexOf('&', i + 1)) >= 0)
			{
				if ((j = s.IndexOf(';', i + 1)) > 0)
				{
					string from = Substring(s, i + 1, j).ToLower();
					string to = null;
					if ((to = Replacement(from)) != null)
						s = Substring(s, 0, i) + to + s.Substring(j + 1);
				}
			}
			return s;
		}

		public static string Replacement(string from)
		{
			if (from == "lt")
				return "<";
			else if (from == "gt")
				return ">";
			else if (from == "amp")
				return "&";
            else if (from == "nbsp")
                return " ";
            else
				return null;
		}

		public virtual void Print(TextWriter output)
		{
			output.Write(ToString());
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			return BuildString(builder).ToString();
		}

		private StringBuilder BuildString(StringBuilder builder)
		{
			builder.Append(Leader);
			builder.Append(Tag);
            // change
			if (Parts != null) builder.Append(Parts.BuildString(new StringBuilder()));
            builder.Append(Body);
            // end change
			builder.Append(End);
			if (More != null)
			{
				return builder.Append(More.BuildString(new StringBuilder()));
			}
			else
			{
				builder.Append(Trailer);
				return builder;
			}
		}

		public static int FootnoteFiles = 0;

		public virtual string Footnote
		{
			get
			{
				if (FootnoteFiles >= 25)
					return "[-]";
				else
				{
					try
					{
						int thisFootnote = ++FootnoteFiles;
						string html = "footnotes/" + thisFootnote + ".html";
						FileInfo file = new FileInfo("Reports/" + html);

						// Create the Reports directory if not exists
						string directory = file.DirectoryName;
						if (!Directory.Exists(directory))
							Directory.CreateDirectory(directory);
						else if (file.Exists)
							file.Delete();

						TextWriter output = file.CreateText();
						Print(output);
						output.Close();
						return string.Format("<a href={0}>[{1}]</a>", file.FullName, thisFootnote);
					}
					catch (IOException)
					{
						return "[!]";
					}
				}
			}
		}

        public Parse DeepCopy() {
            return new Parse(tag, End, Leader, body, (Parts == null ? null : Parts.DeepCopy())) {
                Attributes = Attributes,
                Trailer = Trailer,
                More = (More == null ? null : More.DeepCopy())
            };
        }

        public Parse Copy() {
            return new Parse(tag, End, Leader, body, Parts) {Trailer = Trailer, Attributes = Attributes};
        }

	    public override Cell Value { get { return this; } }
	    public override bool IsLeaf { get { return true; } }
	    public override ReadList<Tree<Cell>> Branches { get { throw new InvalidOperationException(); } }
        public Parse ParseCell { get { return this; }}
	}
}