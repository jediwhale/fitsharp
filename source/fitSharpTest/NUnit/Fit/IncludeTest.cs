// Copyright © 2012 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Fit.Engine;
using fitSharp.Fit.Fixtures;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.IO;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Samples;
using fitSharp.Samples.Fit;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class IncludeTest {
        [SetUp] public void SetUp() {
            processor = Builder.CellProcessor();
            includeAction = new IncludeAction(processor);
        }

        [Test] public void ParsesAndExecutesIncludedText() {
            processor.AddOperator(new MockRunTestOperator());
            processor.AddOperator(new MockComposeStoryTestString());
            processor.AddOperator(new MockParseStoryTestString());
            var includeTable = new CellTree(new CellTree("include", "string", input));
            new Include().Interpret(processor, includeTable);
            Assert.IsTrue(includeTable.ValueAt(0, 0).HasAttribute(CellAttribute.Folded));
            Assert.AreEqual(result, includeTable.ValueAt(0, 0).GetAttribute(CellAttribute.Folded));
        }

        [Test] public void IncludesPage() {
            MakePage(pageName);
            includeAction.Page(pageName);
            Assert.AreEqual(pageContent, includeAction.Result);
        }

        [Test] public void IncludesPageRelativeToCurrent() {
            MakePage(System.IO.Path.Combine(currentPath, pageName));
            Context.TestPagePath = new FilePath(System.IO.Path.Combine(currentPath, currentPage));
            includeAction.PageFromCurrent(pageName);
            Assert.AreEqual(pageContent, includeAction.Result);
        }

        [Test] public void IncludesPageRelativeToSuite() {
            MakePage(System.IO.Path.Combine(currentPath, pageName));
            Context.SuitePath = new DirectoryPath(currentPath);
            includeAction.PageFromSuite(pageName);
            Assert.AreEqual(pageContent, includeAction.Result);
        }

        void MakePage(string pagePath) {
            var pageSource = new FolderTestModel();
            pageSource.MakeFile(pagePath, pageContent);
            processor.Get<Context>().PageSource = pageSource;
        }

        Context Context { get { return processor.Get<Context>(); } }
        CellProcessorBase processor;
        IncludeAction includeAction;

        const string currentPage = "currentPage";
        const string currentPath = "currentPath";
        const string pageContent = "pageContent";
        const string pageName = "pageName";
        const string input = "stuff";
        const string result = "more stuff";
        static readonly Tree<Cell> parsedInput = new CellTree("something");

        class MockRunTestOperator: CellOperator, RunTestOperator {
            public bool CanRunTest(Tree<Cell> testTables, StoryTestWriter writer) {
                return true;
            }

            public TypedValue RunTest(Tree<Cell> testTables, StoryTestWriter writer) {
                Assert.AreSame(parsedInput, testTables);
                writer.WriteTable(parsedInput);
                return TypedValue.Void;
            }
        }

        class MockComposeStoryTestString: CellOperator, ComposeOperator<Cell> {
            public bool CanCompose(TypedValue instance) {
                return instance.Type == typeof(StoryTestString);
            }

            public Tree<Cell> Compose(TypedValue instance) {
                Assert.AreEqual(input, instance.ValueString);
                return parsedInput;
            }
        }

        class MockParseStoryTestString: CellOperator, ParseOperator<Cell> {
            public bool CanParse(Type type, TypedValue instance, Tree<Cell> parameters) {
                return type == typeof(StoryTableString);
            }

            public TypedValue Parse(Type type, TypedValue instance, Tree<Cell> parameters) {
                return new TypedValue(new StoryTableString(result));
            }
        }
    }
}
