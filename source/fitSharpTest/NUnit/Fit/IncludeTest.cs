// Copyright © 2019 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (https://opensource.org/licenses/cpl1.0.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

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
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture] public class IncludeTest {
        [SetUp] public void SetUp() {
            processor = Builder.CellProcessor();
            includeAction = new IncludeAction(processor);
            parsedInput = new CellTree(new CellBase(result), "something");
        }

        [Test] public void ParsesAndExecutesIncludedText() {
            processor.ItemOf<FitEnvironment>().RunTest = new MockRunTest();
            processor.AddOperator(new MockComposeStoryTestString());
            var includeTable = new CellTree(new CellTree("include", "string", input));
            new Include().Interpret(processor, includeTable);
            Assert.IsTrue(includeTable.ValueAt(0, 0).HasAttribute(CellAttribute.Folded));
            Assert.AreEqual(result, includeTable.ValueAt(0, 0).GetAttribute(CellAttribute.Folded));
            Assert.AreEqual(1, processor.TestStatus.Counts.GetCount(TestStatus.Right));
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

        Context Context => processor.Get<Context>();
        CellProcessorBase processor;
        IncludeAction includeAction;

        const string currentPage = "currentPage";
        const string currentPath = "currentPath";
        const string pageContent = "pageContent";
        const string pageName = "pageName";
        const string input = "stuff";
        const string result = "more stuff";
        static Tree<Cell> parsedInput;

        class MockRunTest: RunTest {
            public void Run(CellProcessor processor, Tree<Cell> testTables, StoryTestWriter writer) {
                Assert.AreSame(parsedInput, testTables);
                parsedInput.Value.SetAttribute(CellAttribute.Body, result);
                writer.WriteTable(parsedInput);
                processor.TestStatus.MarkRight(testTables.Branches[0].Value);
            }
        }

        class MockComposeStoryTestString: CellOperator, ComposeOperator<Cell> {
            public bool CanCompose(TypedValue instance) {
                return instance.Type == typeof(HtmlStoryTestSource);
            }

            public Tree<Cell> Compose(TypedValue instance) {
                Assert.AreEqual(input, instance.ValueString);
                return parsedInput;
            }
        }
    }
}
