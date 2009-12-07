// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections;
using System.Collections.Generic;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Extension;
using fitSharp.Machine.Model;
using fitSharp.Parser;

namespace fitSharp.Slim.Operators {
    public class ParseDictionary: SlimOperator, ParseOperator<string> {
        public bool CanParse(Type type, TypedValue instance, Tree<string> parameters) {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Dictionary<,>);
        }

        public TypedValue Parse(Type type, TypedValue instance, Tree<string> parameters) {
            StringNode table = new HtmlTables<StringNode>(new StringNodeFactory()).Parse(parameters.Value);
            return new TypedValue(table.Node.Branches.AggregateTo(
                (IDictionary) Activator.CreateInstance(type),
                (dictionary, row) => dictionary.Add(
                    Processor.ParseTree(type.GetGenericArguments()[0], row.Branches[0]).Value,
                    Processor.ParseTree(type.GetGenericArguments()[1], row.Branches[1]).Value)));
        }

        private class StringNodeFactory: ParseTreeNodeFactory<StringNode> {
            public StringNode MakeNode(string startTag, string endTag, string leader, string body, StringNode firstChild) {
                return new StringNode(body, firstChild);
            }

            public void AddTrailer(StringNode node, string trailer) {}

            public void AddSibling(StringNode node, StringNode sibling) {
                node.AddSibling(sibling);
            }
        }

        private class StringNode {
            private readonly List<StringNode> siblings = new List<StringNode>();

            public TreeList<string> Node { get; private set; }

            public StringNode(string body, StringNode firstChild) {
                Node = new TreeList<string>(body ?? string.Empty);
                if (firstChild != null) foreach (StringNode child in firstChild.siblings) Node.AddBranch(child.Node);
                siblings.Add(this);
            }

            public void AddSibling(StringNode sibling) {
                if (sibling != null) siblings.Add(sibling);
            }
        }
    }
}
