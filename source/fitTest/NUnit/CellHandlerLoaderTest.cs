// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Text;
using fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Application;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class CellHandlerLoaderTest
    {
        [SetUp]
        public void SetUp()
        {
            Context.Configuration.GetItem<Service>().RemoveOperator(typeof (CompareSubstring).FullName);
        }

        [Test]
        public void TestLoadHandler()
        {
            TestUtils.InitAssembliesAndNamespaces();
            StringBuilder builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td colspan=\"2\">cell handler loader</td></tr>");
            builder.Append("<tr><td>load</td><td>substring handler</td></tr>");
            builder.Append("</table>");
            Assert.IsFalse(Context.Configuration.GetItem<Service>().Compare(new TypedValue("abc"), TestUtils.CreateCell("..b..")));
            StoryTest test = new StoryTest(new Parse(builder.ToString()));
            test.ExecuteOnConfiguration();
            Assert.IsTrue(Context.Configuration.GetItem<Service>().Compare(new TypedValue("abc"), TestUtils.CreateCell("..b..")));
        }

        [Test]
        public void TestRemoveHandler()
        {
            TestUtils.InitAssembliesAndNamespaces();
            Context.Configuration.GetItem<Service>().AddOperator(new CompareSubstring());
            StringBuilder builder = new StringBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td colspan=\"2\">CellHandlerLoader</td></tr>");
            builder.Append("<tr><td>remove</td><td>SubstringHandler</td></tr>");
            builder.Append("</table>");
            Assert.IsTrue(Context.Configuration.GetItem<Service>().Compare(new TypedValue("abc"), TestUtils.CreateCell("..b..")));
            StoryTest test = new StoryTest(new Parse(builder.ToString()));
            test.ExecuteOnConfiguration();
            Assert.IsFalse(Context.Configuration.GetItem<Service>().Compare(new TypedValue("abc"), TestUtils.CreateCell("..b..")));
        }
    }
}