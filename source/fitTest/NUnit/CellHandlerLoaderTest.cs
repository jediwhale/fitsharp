// Copyright � 2011 Syterra Software Inc. Includes work by Object Mentor, Inc., � 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;

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
            var service = new Service.Service(configuration);
            ClassicAssert.IsFalse(service.Compare(new TypedValue("abc"), TestUtils.CreateCell("..b..")));
            var test = builder.MakeStoryTest(service);
            test.Execute();
            ClassicAssert.IsTrue(service.Compare(new TypedValue("abc"), TestUtils.CreateCell("..b..")));
        }

        [Test]
        public void TestRemoveHandler()
        {
            var configuration = TestUtils.InitAssembliesAndNamespaces();
            var service = new Service.Service(configuration);
            service.AddOperator(new CompareSubstring());
            var builder = new TestBuilder();
            builder.Append("<table>");
            builder.Append("<tr><td colspan=\"2\">CellHandlerLoader</td></tr>");
            builder.Append("<tr><td>remove</td><td>SubstringHandler</td></tr>");
            builder.Append("</table>");
            ClassicAssert.IsTrue(new Service.Service(configuration).Compare(new TypedValue("abc"), TestUtils.CreateCell("..b..")));
            var test = builder.MakeStoryTest(service);
            test.Execute();
            ClassicAssert.IsFalse(new Service.Service(configuration).Compare(new TypedValue("abc"), TestUtils.CreateCell("..b..")));
        }
    }
}