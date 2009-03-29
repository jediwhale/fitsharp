// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit.Engine;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class BindingTest
    {
        [Test]
        public void TestIsQueryCell() 
        {
            ColumnFixture textFixture = new TestFixture();
            Assert.IsTrue(textFixture.CheckIsImpliedBy("isQuery?"));
            Assert.IsTrue(textFixture.CheckIsImpliedBy("isQuery!"));
            Assert.IsTrue(textFixture.CheckIsImpliedBy("isQuery()"));
            Assert.IsFalse(textFixture.CheckIsImpliedBy("isNotQuery+"));
            Assert.IsFalse(textFixture.CheckIsImpliedBy("isNotQuery*"));
            Assert.IsFalse(textFixture.CheckIsImpliedBy("isNotQuery<>"));
        }

        [Test]
        public void TestSetterBinding()
        {
            TestFixture f = new TestFixture { Service = new Service()};
			
            Parse p;
            Binding binding;

            binding = new Binding("sampleInt", TestUtils.CreateCell("sampleInt"), OperationType.Input);
            p = new Parse("<table><tr><td>123456</td></tr></table>").Parts.Parts;
            binding.HandleCell(f, p);
            Assert.AreEqual(123456, f.sampleInt);

            p = new Parse("<table><tr><td>-234567</td></tr></table>").Parts.Parts;
            binding.HandleCell(f, p);
            Assert.AreEqual(-234567, f.sampleInt);
			
            binding = new Binding("sampleDouble", TestUtils.CreateCell("sampleDouble"), OperationType.Input);
            p = new Parse("<table><tr><td>3.14159</td></tr></table>").Parts.Parts;
            binding.HandleCell(f, p);
            Assert.AreEqual(3.14159, f.sampleDouble);

            binding = new Binding("sampleChar", TestUtils.CreateCell("sampleChar"), OperationType.Input);
            p = new Parse("<table><tr><td>a</td></tr></table>").Parts.Parts;
            binding.HandleCell(f, p);
            Assert.AreEqual('a', f.sampleChar);

            binding = new Binding("sampleString", TestUtils.CreateCell("sampleString"), OperationType.Input);
            p = new Parse("<table><tr><td>xyzzy</td></tr></table>").Parts.Parts;
            binding.HandleCell(f, p);
            Assert.AreEqual("xyzzy", f.sampleString);

            binding = new Binding("sampleFloat", TestUtils.CreateCell("sampleFloat"), OperationType.Input);
            p = new Parse("<table><tr><td>6.02e23</td></tr></table>").Parts.Parts;
            binding.HandleCell(f, p);
            Assert.AreEqual(6.02e23f, f.sampleFloat, 1e17f);

            binding = new Binding("sampleByte", TestUtils.CreateCell("sampleByte"), OperationType.Input);
            p = new Parse("<table><tr><td>123</td></tr></table>").Parts.Parts;
            binding.HandleCell(f, p);
            Assert.AreEqual((byte)123, f.sampleByte);

            binding = new Binding("sampleShort", TestUtils.CreateCell("sampleShort"), OperationType.Input);
            p = new Parse("<table><tr><td>12345</td></tr></table>").Parts.Parts;
            binding.HandleCell(f, p);
            Assert.AreEqual((short)12345, f.sampleShort);
        }

        class TestFixture : ColumnFixture 
        {
            public byte sampleByte = 0;
            public short sampleShort =0;
            public int sampleInt = 0;
            public float sampleFloat = 0;
            public double sampleDouble = 3.14159862;
            public char sampleChar = '\0';
            public string sampleString = null;
            public int[] sampleArray = null;
            public DateTime sampleDate = DateTime.Now;
        }

    }
}