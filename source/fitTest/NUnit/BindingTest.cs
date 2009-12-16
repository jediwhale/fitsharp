// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
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
            Parse p = new Parse("<table><tr><td>123456</td></tr></table>").Parts.Parts;
            bindingOperation.Do(null, p);
            Assert.AreEqual(123456, f.sampleInt);

            p = new Parse("<table><tr><td>-234567</td></tr></table>").Parts.Parts;
            bindingOperation.Do(null, p);
            Assert.AreEqual(-234567, f.sampleInt);
			
            bindingOperation = new InputBinding(f.CellOperation, f, TestUtils.CreateCell("sampleDouble"));
            p = new Parse("<table><tr><td>3.14159</td></tr></table>").Parts.Parts;
            bindingOperation.Do(null, p);
            Assert.AreEqual(3.14159, f.sampleDouble);

            bindingOperation = new InputBinding(f.CellOperation, f, TestUtils.CreateCell("sampleChar"));
            p = new Parse("<table><tr><td>a</td></tr></table>").Parts.Parts;
            bindingOperation.Do(null, p);
            Assert.AreEqual('a', f.sampleChar);

            bindingOperation = new InputBinding(f.CellOperation, f, TestUtils.CreateCell("sampleString"));
            p = new Parse("<table><tr><td>xyzzy</td></tr></table>").Parts.Parts;
            bindingOperation.Do(null, p);
            Assert.AreEqual("xyzzy", f.sampleString);

            bindingOperation = new InputBinding(f.CellOperation, f, TestUtils.CreateCell("sampleFloat"));
            p = new Parse("<table><tr><td>6.02e23</td></tr></table>").Parts.Parts;
            bindingOperation.Do(null, p);
            Assert.AreEqual(6.02e23f, f.sampleFloat, 1e17f);

            bindingOperation = new InputBinding(f.CellOperation, f, TestUtils.CreateCell("sampleByte"));
            p = new Parse("<table><tr><td>123</td></tr></table>").Parts.Parts;
            bindingOperation.Do(null, p);
            Assert.AreEqual(123, f.sampleByte);

            bindingOperation = new InputBinding(f.CellOperation, f, TestUtils.CreateCell("sampleShort"));
            p = new Parse("<table><tr><td>12345</td></tr></table>").Parts.Parts;
            bindingOperation.Do(null, p);
            Assert.AreEqual(12345, f.sampleShort);
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