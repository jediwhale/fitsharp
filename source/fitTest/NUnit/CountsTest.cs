// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class CountsTest
    {
        private Counts counts;

        [SetUp]
        public void SetUp()
        {
            counts = new Counts();
        }
		
        [Test]
        public void TestInitialState()
        {
            Assert.AreEqual(0, counts.Right);
            Assert.AreEqual(0, counts.Wrong);
            Assert.AreEqual(0, counts.Exceptions);
            Assert.AreEqual(0, counts.Ignores);
        }

        [Test]
        public void TestTally()
        {
            SetCounts(1,2,3,4);
            counts.Tally(counts);
            Assert.AreEqual(2, counts.Right);
            Assert.AreEqual(4, counts.Wrong);
            Assert.AreEqual(6, counts.Exceptions);
            Assert.AreEqual(8, counts.Ignores);
        }

        [Test]
        public void TestProperties()
        {
            SetCounts(9,8,7,6);
            Assert.AreEqual(9, counts.Right);
            Assert.AreEqual(8, counts.Wrong);
            Assert.AreEqual(7, counts.Exceptions);
            Assert.AreEqual(6, counts.Ignores);
        }

        private void SetCounts(int right, int wrong, int exceptions, int ignores)
        {
            counts.Right = right;
            counts.Wrong = wrong;
            counts.Exceptions = exceptions;
            counts.Ignores = ignores;
        }
    }
}