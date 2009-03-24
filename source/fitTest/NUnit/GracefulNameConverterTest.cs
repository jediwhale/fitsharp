// FitNesse.NET
// Copyright © 2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;
using NUnit.Framework;

namespace fit.Test.NUnit {
    [TestFixture]
    public class GracefulNameTest
    {
        private string uglyString;

        [SetUp]
        public void SetUp() 
        {
            uglyString = "?&_)*( bad&^%$*()(*&)compAny~`+=-_,";
        }

        [Test]
        public void TestGracefulNameConverterOnTypeName() 
        {
            Assert.AreEqual("badcompany", new GracefulName(uglyString).IdentifierName.ToString());
            Assert.AreEqual("badcompany", new GracefulName("Bad.Company").IdentifierName.ToString());
            Assert.AreEqual("badcompany", new GracefulName("BadCompany").IdentifierName.ToString());
            Assert.AreEqual("badcompany", new GracefulName("Bad Company").IdentifierName.ToString());
            Assert.AreEqual("badcompany", new GracefulName("bad company").IdentifierName.ToString());
            Assert.AreEqual("badcompany", new GracefulName("Bad-Company").IdentifierName.ToString());
            Assert.AreEqual("badcompany", new GracefulName("Bad Company.").IdentifierName.ToString());
            Assert.AreEqual("badcompany", new GracefulName("(Bad Company)").IdentifierName.ToString());
            Assert.AreEqual("bad123company", new GracefulName("bad 123 company").IdentifierName.ToString());
            Assert.AreEqual("bad123company", new GracefulName("bad 123company").IdentifierName.ToString());
            Assert.AreEqual("bad123company", new GracefulName("   bad  \t123  company   ").IdentifierName.ToString());
        }

        [Test]
        public void TestConvertMemberName()
        {
            Assert.AreEqual("somemembername", new GracefulName("Some Member Name").IdentifierName.ToString());
            Assert.AreEqual("somemembername", new GracefulName("Some Member Name?").IdentifierName.ToString());
            Assert.AreEqual("somemembername", new GracefulName("Some Member Name!").IdentifierName.ToString());
            Assert.AreEqual("somemembername", new GracefulName("Some Member Name()").IdentifierName.ToString());
            Assert.AreEqual("member1name", new GracefulName("Member 1 Name.").IdentifierName.ToString());
        }
    }
}