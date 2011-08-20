// Copyright © 2010 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class CellHandlerLoaderTest
    {
        [Test]
        public void TestLoadHandler()
        {
            var configuration = TestUtils.InitAssembliesAndNamespaces();
            var builder = new TestBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td colspan=\"2\">cell handler loader</td></tr>");
            builder.Append("<tr><td>load</td><td>substring handler</td></tr>");
            builder.Append("</table>");
            Assert.IsFalse(new Service.Service(configuration).Compare(new TypedValue("abc"), TestUtils.CreateCell("..b..")));
            StoryTest test = builder.MakeStoryTest();
            test.ExecuteOnConfiguration(configuration);
            Assert.IsTrue(new Service.Service(configuration).Compare(new TypedValue("abc"), TestUtils.CreateCell("..b..")));
        }

        [Test]
        public void TestRemoveHandler()
        {
            var configuration = TestUtils.InitAssembliesAndNamespaces();
            new Service.Service(configuration).AddOperator(new CompareSubstring());
            var builder = new TestBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td colspan=\"2\">CellHandlerLoader</td></tr>");
            builder.Append("<tr><td>remove</td><td>SubstringHandler</td></tr>");
            builder.Append("</table>");
            Assert.IsTrue(new Service.Service(configuration).Compare(new TypedValue("abc"), TestUtils.CreateCell("..b..")));
            var test = builder.MakeStoryTest();
            test.ExecuteOnConfiguration(configuration);
            Assert.IsFalse(new Service.Service(configuration).Compare(new TypedValue("abc"), TestUtils.CreateCell("..b..")));
        }
    }
}