// Copyright © 2015 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;
using fitSharp.Machine.Model;
using fitSharp.Slim.Model;

namespace fitSharp.Slim.Service {
    public class Document {
        public Document(Tree<string> content) {
            Content = content;
        }

        public Tree<string> Content { get; private set; }

        public override string ToString() { return Content.Serialize(new SlimWriter()).ToString(); }

        class SlimWriter: TreeWriter<string> {
            readonly StringBuilder output = new StringBuilder();

            public void WritePrefix(Tree<string> tree) {
                if (!tree.IsLeaf) output.AppendFormat("[{0:000000}:", tree.Branches.Count);
                else output.Append(tree.Value ?? "null");
            }

            public void WriteBranch(Tree<string> tree, int index) {
                string item = tree.Serialize(new SlimWriter()).ToString();
                output.AppendFormat("{0:000000}:{1}:", item.Length, item);
            }

            public void WriteSuffix(Tree<string> tree) {
                if (!tree.IsLeaf) output.Append(']');
            }

            public override string ToString() { return output.ToString(); }
        }

        public static Document Parse(string input) { return new Document(Read(input)); }

        static Tree<string> Read(string input) {
            if (IsList(input)) return ReadList(new DelimitedString(input.Substring(1, input.Length - 2)));
            return new SlimLeaf(input);
        }

        static bool IsList(string input) {
            int result;
            return input.StartsWith("[")
                && input.Length > 8
                && input.Substring(7, 1) == ":"
                && input.EndsWith("]")
                && int.TryParse(input.Substring(1, 6), out result);
        }

        static SlimTree ReadList(DelimitedString input) {
            var count = int.Parse(input.ReadTo(":"));
            var result = new SlimTree();
            for (var i = 0; i < count; i++) {
                var itemLength = int.Parse(input.ReadTo(":"));
                result.AddBranch(Read(input.Read(itemLength, 1)));
            }
            return result;
        }

        class DelimitedString {
            public DelimitedString(string content) {
                this.content = content;
                position = 0;
            }

            public string ReadTo(string terminator) {
                var end = content.IndexOf(terminator, position, StringComparison.Ordinal);
                if (end < 0) throw new InvalidOperationException("no terminator");
                var result = content.Substring(position, end - position);
                position = end + terminator.Length;
                return result;
            }

            public string Read(int length, int skip) {
                var result = content.Substring(position, length);
                position += length + skip;
                return result;
            }

            readonly string content;
            int position;
        }
    }
}
