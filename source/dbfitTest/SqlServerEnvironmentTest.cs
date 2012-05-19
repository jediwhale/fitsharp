// Copyright © 2012 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Data.Common;
using dbfit;
using NUnit.Framework;

namespace dbfitTest {
    [TestFixture] public class SqlServerEnvironmentTest {
        [Test] public void BuildsInsertCommand() {
            var environment = new SqlServerEnvironment();
            var parameterA = MakeParameter(environment, "aColumn", "aParameter");
            var parameterB = MakeParameter(environment, "bColumn", "bParameter");
            var command = environment.BuildInsertCommand(
                "aTable",
                new[] {
                    new DbParameterAccessor(parameterA, typeof(string), 0, "varchar"),
                    new DbParameterAccessor(parameterB, typeof(int), 1, "integer")
                });
            Assert.AreEqual("insert into aTable([aColumn],[bColumn]) values (@aParameter,@bParameter)", command);
        }

        static DbParameter MakeParameter(SqlServerEnvironment environment, string sourceColumn, string parameterName) {
            var parameterA = environment.DbProviderFactory.CreateParameter();
            parameterA.SourceColumn = sourceColumn;
            parameterA.ParameterName = parameterName;
            return parameterA;
        }

        [Test] public void BuildUpdateCommand() {
            var environment = new SqlServerEnvironment();
            var parameterA = MakeParameter(environment, "aColumn", "aParameter");
            var parameterB = MakeParameter(environment, "bColumn", "bParameter");
            var parameterC = MakeParameter(environment, "cColumn", "cParameter");
            var parameterD = MakeParameter(environment, "dColumn", "dParameter");
            var command = environment.BuildUpdateCommand(
                "aTable",
                new [] {
                    new DbParameterAccessor(parameterA, typeof(string), 0, "varchar"),
                    new DbParameterAccessor(parameterB, typeof(int), 1, "integer")
                }, 
                new[] {
                    new DbParameterAccessor(parameterC, typeof(string), 2, "varchar"),
                    new DbParameterAccessor(parameterD, typeof(int), 3, "integer")
                });
            Assert.AreEqual("update aTable set [aColumn]=@aParameter, [bColumn]=@bParameter where [cColumn]=@cParameter and [dColumn]=@dParameter", command);
        }
    }
}
