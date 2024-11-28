// Copyright � 2010 Syterra Software Inc. Includes work by Object Mentor, Inc., � 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace fit.Test.NUnit {
    [TestFixture]
    [SetCulture("en-US")]
    public class ScientificDoubleTest
    {
        [Test]
        public void TestScientificDouble()
        {
            double pi = 3.141592865;
            ClassicAssert.AreEqual(ScientificDouble.ValueOf("3.14"), pi);
            ClassicAssert.AreEqual(ScientificDouble.ValueOf("3.142"), pi);
            ClassicAssert.AreEqual(ScientificDouble.ValueOf("3.1416"), pi);
            ClassicAssert.AreEqual(ScientificDouble.ValueOf("3.14159"), pi);
            ClassicAssert.AreEqual(ScientificDouble.ValueOf("3.141592865"), pi);
            ClassicAssert.IsTrue(!ScientificDouble.ValueOf("3.140").Equals(pi));
            ClassicAssert.IsTrue(!ScientificDouble.ValueOf("3.144").Equals(pi));
            ClassicAssert.IsTrue(!ScientificDouble.ValueOf("3.1414").Equals(pi));
            ClassicAssert.IsTrue(!ScientificDouble.ValueOf("3.141592863").Equals(pi));
            ClassicAssert.AreEqual(ScientificDouble.ValueOf("6.02e23"), 6.02e23d);
            ClassicAssert.AreEqual(ScientificDouble.ValueOf("6.02E23"), 6.024E23d);
            ClassicAssert.AreEqual(ScientificDouble.ValueOf("6.02e23"), 6.016e23d);
            ClassicAssert.IsTrue(!ScientificDouble.ValueOf("6.02e23").Equals(6.026e23d));
            ClassicAssert.IsTrue(!ScientificDouble.ValueOf("6.02e23").Equals(6.014e23d));
            ClassicAssert.AreEqual(ScientificDouble.ValueOf("3.14"), ScientificDouble.ValueOf("3.14"));
        }
    }
}