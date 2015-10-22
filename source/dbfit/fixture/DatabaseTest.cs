// Copyright © 2015 Syterra Software Inc. Includes work Copyright (C) Gojko Adzic 2006-2008 http://gojko.net
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Data;
using System.Data.Common;
using dbfit.fixture;
using dbfit.util;
using fit;

using fitlibrary;
using fitSharp.Machine.Engine;

namespace dbfit
{
    public class DatabaseTest : SequenceFixture
    {
        protected IDbEnvironment environment;

        public void SetUp() {
            util.Options.reset();
        }

        public void TearDown() {
            environment.CloseConnection();
        }

        public DatabaseTest(IDbEnvironment environment)
        {
            this.environment = environment;
        }

        public void Connect(String dataSource, String username, String password, String database)
        {
            environment.Connect(dataSource, username, password, database);
        }

        public void Connect(String dataSource, String username, String password)
        {
            environment.Connect(dataSource, username, password);
        }

        public void Connect(String connectionString)
        {
            environment.Connect(connectionString);
        }

        public void ConnectUsingFile(String path)
        {
            environment.ConnectUsingFile(path);
        }

        public void ConnectUsingConfig(string connectionName) {
            environment.ConnectUsingConfig(connectionName);
        }

        public void Close()
        {
            environment.CloseConnection();
        }

        public void SetParameter(String name, object value)
        {
            fixture.SetParameter.SetParameterValue(Symbols, name, value);
        }

        public void ClearParameters()
        {
            Symbols.Clear();
        }

        public Fixture Query(String query)
        {
            return new Query(GetDataTable(Symbols, query, environment), false);
        }

        public Fixture Query(String query, int resultSet)
        {
            return new Query(GetDataTable(Symbols, query, environment, resultSet), false);
        }

        public Fixture Query(DataTable queryTable)
        {
            return new Query(queryTable, false);
        }

        public Fixture Update(String table)
        {
            return new Update(environment, table);
        }

        public Fixture OrderedQuery(String query)
        {
            return new Query(GetDataTable(Symbols, query, environment), true);
        }

        public Fixture Execute(String statement)
        {
            return new Execute(environment, statement);
        }

        public Fixture Insert(String table)
        {
            return new Insert(environment, table);
        }

        public Fixture ExecuteProcedure(String procedure)
        {
            return new ExecuteProcedure(environment, procedure);
        }

        public Fixture ExecuteProcedureExpectException(String procedure)
        {
            return new ExecuteProcedure(environment, procedure, true);
        }

        public Fixture ExecuteProcedureExpectException(String procedure, int errorCode)
        {
            return new ExecuteProcedure(environment, procedure, errorCode);
        }

        public void Commit()
        {
            environment.Commit();
        }

        public void Rollback()
        {
            environment.Rollback();
        }

        public Fixture QueryStats()
        {
            return new QueryStats(environment);
        }

        public Fixture Clean()
        {
            return new Clean(environment);
        }

        public Fixture InspectProcedure(String procedure)
        {
            return new Inspect(environment, Inspect.MODE_PROCEDURE, procedure);
        }

        public Fixture InspectTable(String table)
        {
            return new Inspect(environment, Inspect.MODE_TABLE, table);
        }

        public Fixture InspectView(String view)
        {
            return new Inspect(environment, Inspect.MODE_TABLE, view);
        }

        public Fixture InspectQuery(String query)
        {
            return new Inspect(environment, Inspect.MODE_QUERY, query);
        }

        public Fixture StoreQuery(String query, String symbolName)
        {
            return new StoreQuery(environment, query, symbolName);
        }

        public Fixture CompareStoredQueries(String symbol1, String symbol2)
        {
            return new CompareStoredQueries(environment, symbol1, symbol2);
        }

        public void SetOption(String option, String value)
        {
            util.Options.SetOption(Processor, option, value);
        }

        public static DataTable GetDataTable(Symbols symbols, String query, IDbEnvironment environment)
        {
            return GetDataTable(symbols, query, environment, 1);
        }

        public static DataTable GetDataTable(Symbols symbols, String query,IDbEnvironment environment, int rsNo)
        {
            DbCommand dc = environment.CreateCommand(query, CommandType.Text);
            if (Options.ShouldBindSymbols())
                environment.BindFixtureSymbols(symbols, dc);

            DbDataAdapter oap = environment.DbProviderFactory.CreateDataAdapter();
            oap.SelectCommand = dc;
            var ds = new DataSet();
            oap.Fill(ds);
            dc.Dispose();
            return ds.Tables[rsNo - 1];
        }
    }
}
