// Copyright © 2011 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using fitSharp.Parser;

namespace fit
{
	public class Parse: CellBase, Tree<Cell>
	{
		public static int FootnoteFiles;

        public static Parse ParseFrom(string input) {
            return ParseRootFrom(input).Parts;
        }

	    public static Parse ParseRootFrom(string input) {
	        return CopyFrom(new HtmlTables().Parse(input));
	    }

	    public Parse More { get; set; }
	    public Parse Parts { get; set; }

	    string body {
	        get { return GetAttribute(CellAttribute.Body); }
            set { SetAttribute(CellAttribute.Body, value); }
	    }

	    string tag {
	        get { return GetAttribute(CellAttribute.StartTag); }
            set { SetAttribute(CellAttribute.StartTag, value); }
	    }

	    public string End {
	        get { return GetAttribute(CellAttribute.EndTag); }
            private set { SetAttribute(CellAttribute.EndTag, value); }
	    }

	    public string Leader {
	        get { return GetAttribute(CellAttribute.Leader); }
            private set { SetAttribute(CellAttribute.Leader, value); }
	    }

	    public string Trailer {
	        get { return GetAttribute(CellAttribute.Trailer); }
            set { SetAttribute(CellAttribute.Trailer, value); }
	    }

	    public string Tag {
            get {
	            int space = tag.IndexOf(' ');
	            if (space < 0) space = tag.Length - 1;
	            return !HasAttribute(CellAttribute.Status)
                    ? tag
                    : string.Format("{0} class=\"{1}\"{2}", tag.Substring(0, space), GetAttribute(CellAttribute.Status), tag.Substring(space));
            }
        }

	    public string Body { get {
	        return ParseStoryTestString.Body(this);
            }
        }

		public void SetBody(string val)
		{
			body = val;
		}

		public virtual void AddToBody(string text)
		{
		    AddToAttribute(CellAttribute.InformationSuffix, text);
		}

        Parse(CellBase source): base(source) {}

        Parse(Parse other)
            : base(other) {
            Parts = other.Parts;
        }

        public Parse(string text, string theTag, string theEnd, string theLeader, string theBody, Parse theParts): base(text) {
            tag = theTag;
            End = theEnd;
            Leader = theLeader;
            body = theBody;
            Parts = theParts;
        }

		public Parse(string tag, string body, Parse parts, Parse more): base(string.IsNullOrEmpty(body) ? string.Empty : body.Trim())
		{
			Leader = "\n";
			this.tag = "<" + tag + ">";
			this.body = body;
			End = "</" + tag + ">";
			Trailer = "";
			Parts = parts;
			More = more;
		}

        public Parse(string input): base(ParseFrom(input)) {
            Parse other = ParseFrom(input);
            Parts = other.Parts;
            More = other.More;
        }

	    static string Label(string text) {
			return " <span class=\"fit_label\">" + text + "</span>";
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

		public virtual void Print(TextWriter output)
		{
			output.Write(ToString());
		}

		public override string ToString()
		{
			return BuildString(new StringBuilder(), More).ToString();
		}

		StringBuilder BuildString(StringBuilder builder, Parse moreNodes)
		{
			builder.Append(Leader);
			builder.Append(Tag);
			if (Parts != null) builder.Append(Parts.BuildString(new StringBuilder(), Parts.More));
            builder.Append(Body);
			builder.Append(End);
			if (moreNodes != null)
			{
				return builder.Append(moreNodes.BuildString(new StringBuilder(), moreNodes.More));
			}
			builder.Append(Trailer);
			return builder;
		}

		public virtual string Footnote
		{
			get
			{
				if (FootnoteFiles >= 25)
					return "[-]";
				try
				{
					int thisFootnote = ++FootnoteFiles;
					string html = "footnotes/" + thisFootnote + ".html";
					var file = new FileInfo("Reports/" + html);

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

        public Parse DeepCopy() {
            return DeepCopy(s => null, s => s.More, s => s.Parts);
        }

        public Parse DeepCopy(Func<Parse, Parse> substitute, Func<Parse, Parse> more, Func<Parse, Parse> parts) {
            Parse sub = substitute(this);
            if (sub != null) {
                sub.More = more(this) == null ? null : more(this).DeepCopy(substitute, more, parts);
                return sub;
            }
            return new Parse(Text, tag, End, Leader, body, (parts(this) == null ? null : parts(this).DeepCopy(substitute, more, parts))) {
                Trailer = Trailer,
                More = more(this) == null ? null : more(this).DeepCopy(substitute, more, parts)
            };
        }

        public Parse Copy() {
            return new Parse(this);
        }

        public static Parse CopyFrom(Tree<CellBase> source) {
            var result = new Parse(source.Value);
            foreach (Tree<CellBase> branch in source.Branches) {
                Parse newBranch = CopyFrom(branch);
                if (result.Parts == null) {
                    result.Parts = newBranch; 
                }
                else {
                    result.Parts.Last.More = newBranch;
                }
            }
            return result;
        }

        public IEnumerable<Parse> Siblings {
            get {
                for (var sibling = this; sibling != null; sibling = sibling.More) yield return sibling;
            }
        }

        public IEnumerable<Tree<Cell>> SiblingTrees {
            get {
                for (var sibling = this; sibling != null; sibling = sibling.More) yield return sibling;
            }
        }

	    public new Cell Value { get { return this; } }
	    public bool IsLeaf { get { return Parts == null; } }
	    public ReadList<Tree<Cell>> Branches { get { return new ParseList(this); } }
        public Parse ParseCell { get { return this; }}
	}

    public class ParseList: ReadList<Tree<Cell>> {
        private readonly Parse parse;

        public ParseList(Parse parse) { this.parse = parse; }
        public IEnumerator<Tree<Cell>> GetEnumerator() {
            for (Parse part = parse.Parts; part !=null; part = part.More) yield return part;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public Tree<Cell> this[int index] {
            get { return parse.Parts.At(index); }
        }

        public int Count {
            get { return parse.Parts == null ? 0 : parse.Parts.Size; }
        }
    }
}
