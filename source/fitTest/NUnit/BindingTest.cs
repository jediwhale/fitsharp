// Copyright © 2010 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Service;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class BindingTest
    {
        [Test]
        public void TestSetterBinding()
        {
            var f = new TestFixture { Processor = new Service.Service()};

            BindingOperation bindingOperation = new InputBinding(f.CellOperation, f, TestUtils.CreateCell("sampleInt"));
            bindingOperation.Do(MakeCell("123456"));
            Assert.AreEqual(123456, f.sampleInt);

            bindingOperation.Do(MakeCell("-234567"));
            Assert.AreEqual(-234567, f.sampleInt);
			
            bindingOperation = new InputBinding(f.CellOperation, f, TestUtils.CreateCell("sampleDouble"));
            bindingOperation.Do(MakeCell("3.14159"));
            Assert.AreEqual(3.14159, f.sampleDouble);

            bindingOperation = new InputBinding(f.CellOperation, f, TestUtils.CreateCell("sampleChar"));
            bindingOperation.Do(MakeCell("a"));
            Assert.AreEqual('a', f.sampleChar);

            bindingOperation = new InputBinding(f.CellOperation, f, TestUtils.CreateCell("sampleString"));
            bindingOperation.Do(MakeCell("xyzzy"));
            Assert.AreEqual("xyzzy", f.sampleString);

            bindingOperation = new InputBinding(f.CellOperation, f, TestUtils.CreateCell("sampleFloat"));
            bindingOperation.Do(MakeCell("6.02e23"));
            Assert.AreEqual(6.02e23f, f.sampleFloat, 1e17f);

            bindingOperation = new InputBinding(f.CellOperation, f, TestUtils.CreateCell("sampleByte"));
            bindingOperation.Do(MakeCell("123"));
            Assert.AreEqual(123, f.sampleByte);

            bindingOperation = new InputBinding(f.CellOperation, f, TestUtils.CreateCell("sampleShort"));
            bindingOperation.Do(MakeCell("12345"));
            Assert.AreEqual(12345, f.sampleShort);
        }

        static Parse MakeCell(string cellContent) {
            return Parse.ParseFrom("<table><tr><td>" + cellContent + "</td></tr></table>").Parts.Parts;
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