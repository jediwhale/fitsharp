// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitnesse.fitserver;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class PageResultTest
    {
        [Test]
        public void TestToString()
        {
            PageResult result = new PageResult("PageTitle");
            result.TestStatus = TestUtils.MakeTestStatus();
            result.Append("content");
            Assert.AreEqual("PageTitle\n1 right, 2 wrong, 3 ignored, 4 exceptions\ncontent", result.ToString());
        }
    }
}