// Copyright © 2013 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using NUnit.Framework;
using fitSharp.Samples.Fit;

namespace fitSharp.Test.NUnit.Fit {
    [TestFixture]
    [SetCulture("en-US")]
    public class CompareDefaultTest {

        [SetUp] public void SetUp() {
            processor = Builder.CellProcessor();
        }

        [Test]
        public void TestBooleanSymbol() {
            processor.Memory.GetItem<Symbols>().Save("truth", "True");
            Assert.IsTrue(AreEqual(true, "<<truth"));
        }

        [Test]
        public void TestInt() {
            Assert.IsTrue(AreEqual(123, "123"));
        }

        [Test]
        public void TestFloat() {
            Assert.IsTrue(AreEqual(12.3f, "12.3"));
        }

        [Test]
        public void TestDouble() {
            Assert.IsTrue(AreEqual(12.3, "12.3"));
        }

        [Test]
        public void TestDecimal() {
            Assert.IsTrue(AreEqual(12.3M, "12.3"));
        }

        [Test]
        public void TestLong() {
            Assert.IsTrue(AreEqual(123L, "123"));
        }

        [Test]
        public void TestString() {
            Assert.IsTrue(AreEqual("123", "123"));
        }

        [Test]
        public void TestStringArray() {
            string[] array1 = {"a", "b", "c"};
            Assert.IsTrue(AreEqual(array1, "a,b,c"));
            Assert.IsFalse(AreEqual(array1, "d,e"));
        }

        [Test]
        public void TestIntArray() {
            int[] array1 = {1, 2, 3};
            Assert.IsTrue(AreEqual(array1, "1,2,3"));
            Assert.IsFalse(AreEqual(array1, "4,5"));
            Assert.IsFalse(AreEqual(array1, "6,7,8"));
        }

        bool AreEqual(object o1, string o2) {
            var compare = new CompareDefault {Processor = processor};
            return compare.Compare(new TypedValue(o1), new CellTreeLeaf(o2));
        }

        CellProcessorBase processor;
    }
}
