using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace dbfit.util
{
    [TestFixture]
	
    public class DbConnectionPropertiesTest    
    {
        [Test]
        public void TestWithConnectionString()
        {
            DbConnectionProperties props=DbConnectionProperties.CreateFromString(
                @"connection-string=test1234");
            ClassicAssert.AreEqual("test1234", props.FullConnectionString);
            ClassicAssert.IsNull(props.Username);
            ClassicAssert.IsNull(props.Password);
            ClassicAssert.IsNull(props.Service);
            ClassicAssert.IsNull(props.DbName);
        }
        [Test]
        public void TestWithConnectionStringWithEquals()
        {
            DbConnectionProperties props = DbConnectionProperties.CreateFromString(
                @"connection-string=test1234&Username=US&Password=PW");
            ClassicAssert.AreEqual("test1234&Username=US&Password=PW", props.FullConnectionString);
            ClassicAssert.IsNull(props.Username);
            ClassicAssert.IsNull(props.Password);
            ClassicAssert.IsNull(props.Service);
            ClassicAssert.IsNull(props.DbName);
        }
        [Test]
        public void TestWithSplitProperties()
        {
            DbConnectionProperties props = DbConnectionProperties.CreateFromString(
                @"service=testsvc
                  username=testuser
                  password=testpwd
                  database=testdb");
            ClassicAssert.IsNull(props.FullConnectionString);
            ClassicAssert.AreEqual("testuser",props.Username);
            ClassicAssert.AreEqual("testpwd",props.Password);
            ClassicAssert.AreEqual("testsvc",props.Service);
            ClassicAssert.AreEqual("testdb",props.DbName);
        }
        [Test]
        public void TestCommentsAndEmptyLines()
        {
            DbConnectionProperties props = DbConnectionProperties.CreateFromString(
                @"service=testsvc

                  username=testuser
                  password=testpwd
                  #this is a comment
                  database=testdb
                ");
            ClassicAssert.IsNull(props.FullConnectionString);
            ClassicAssert.AreEqual("testuser", props.Username);
            ClassicAssert.AreEqual("testpwd", props.Password);
            ClassicAssert.AreEqual("testsvc", props.Service);
            ClassicAssert.AreEqual("testdb", props.DbName);
        }

    }
}
