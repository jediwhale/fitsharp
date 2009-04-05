// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class CompareDefaultTest
    {
        [Test]
        public void TestIntAdapter()
        {
            Assert.IsTrue(AreEqual(123, "123"));
        }

        [Test]
        public void TestFloatAdapter()
        {
            Assert.IsTrue(AreEqual(12.3f, "12.3"));
        }

        [Test]
        public void TestDoubleAdapter()
        {
            Assert.IsTrue(AreEqual(12.3, "12.3"));
        }

        [Test]
        public void TestDecimalAdapter()
        {
            Assert.IsTrue(AreEqual(12.3M, "12.3"));
        }

        [Test]
        public void TestLongAdapter()
        {
            Assert.IsTrue(AreEqual(123L, "123"));
        }

        [Test]
        public void TestStringAdapter()
        {
            Assert.IsTrue(AreEqual("123", "123"));
        }

        [Test]
        public void TestStringArrayEquality()
        {
            string[] array1 = {"a", "b", "c"};
            Assert.IsTrue(AreEqual(array1, "a,b,c"));
            Assert.IsFalse(AreEqual(array1, "d,e"));
        }

        [Test]
        public void TestIntArrayAdapter()
        {
            Assert.IsTrue(AreEqual(new int[] {1, 2, 3}, "1,2,3"));
        }

        [Test]
        public void TestIntArrayEquality()
        {
            int[] array1 = {1, 2, 3};
            Assert.IsTrue(AreEqual(array1, "1,2,3"));
            Assert.IsFalse(AreEqual(array1, "4,5"));
            Assert.IsFalse(AreEqual(array1, "6,7,8"));
        }

        private static bool AreEqual(object o1, string o2) {
            bool result = false;
            new CompareDefault().TryCompare(new Service.Service(), new TypedValue(o1), new StringCell(o2), ref result);
            return result;
        }
    }
}