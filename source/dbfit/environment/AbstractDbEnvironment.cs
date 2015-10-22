/// Copyright (C) Gojko Adzic 2006-2008 http://gojko.net
/// Released under GNU GPL 2.0

using dbfit.util;
using fitSharp.Machine.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace dbfit
{
    /// <summary>
    /// Utility class to simplify development of IDbEnvironment implementations. This class
    /// has implementations for methods that will be common across most databases. It also provides
    /// default accessors for the current connection and transaction.
    /// </summary>
    public abstract class AbstractDbEnvironment : IDbEnvironment
    {
        /// <summary>
        /// optional prefix symbol that is added to parameter names in Sql commands.
        /// </summary>
        public abstract String ParameterPrefix { get; }

        #region default implementations for IDbEnvironment methods and properties

        private DbConnection _currentConnection;
        private DbTransaction _currentTransaction;

        public DbConnection CurrentConnection
        {
            get
            {
                return _currentConnection;
            }
            protected set
            {
                _currentConnection = value;
            }
        }

        public DbTransaction CurrentTransaction
        {
            get
            {
                return _currentTransaction;
            }
            protected set
            {
                _currentTransaction = value;
            }
        }

        public virtual void Connect(String dataSource, String username, String password, String database)
        {
            Connect(GetConnectionString(dataSource, username, password, database));
        }

        public virtual void Connect(String dataSource, String username, String password)
        {
            Connect(GetConnectionString(dataSource, username, password));
        }

        public virtual void Connect(String connectionString)
        {
            CurrentConnection = DbProviderFactory.CreateConnection();
            CurrentConnection.ConnectionString = connectionString;
            CurrentConnection.Open();
            CurrentTransaction = CurrentConnection.BeginTransaction();
        }

        public virtual void ConnectUsingFile(String connectionPropertiesFile)
        {
            DbConnectionProperties dbp = DbConnectionProperties.CreateFromFile(connectionPropertiesFile);
            if (dbp.FullConnectionString != null) Connect(dbp.FullConnectionString);
            else if (dbp.DbName != null) Connect(dbp.Service, dbp.Username, dbp.Password, dbp.DbName);
            else Connect(dbp.Service, dbp.Username, dbp.Password);
        }

        public virtual void ConnectUsingConfig(String connectionName)
        {
            string fullConnectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;

            if (fullConnectionString == null)
            {
                throw new Exception(string.Format("No Connection String found for {0}", connectionName));
            }
            Connect(fullConnectionString);
        }

        public virtual void ConnectNoTransaction(String dataSource, String username, String password, String database)
        {
            string connectionString = GetConnectionString(dataSource, username, password, database);
            CurrentConnection = DbProviderFactory.CreateConnection();
            CurrentConnection.ConnectionString = connectionString;
            CurrentConnection.Open();
        }

        protected virtual void AddInput(DbCommand dbCommand, String name, Object value)
        {
            DbParameter dbParameter = dbCommand.CreateParameter();
            dbParameter.Direction = ParameterDirection.Input;
            dbParameter.ParameterName = name;
            dbParameter.Value = (value == null ? DBNull.Value : value);
            dbCommand.Parameters.Add(dbParameter);
        }

        public virtual DbCommand CreateCommand(string statement, CommandType commandType)
        {
            if (CurrentConnection == null) throw new ApplicationException("Not connected to database");

            DbCommand dc = CurrentConnection.CreateCommand();
            dc.CommandText = statement.Replace("\r", " ").Replace("\n", " ");
            dc.CommandType = commandType;
            dc.Transaction = CurrentTransaction;
            dc.CommandTimeout = Options.CommandTimeOut;
            return dc;
        }

        public void BindFixtureSymbols(Symbols symbols, DbCommand dc)
        {
            foreach (String paramName in ExtractParamNames(dc.CommandText))
            {
                AddInput(dc, paramName, symbols.GetValueOrDefault(paramName, null));
            }
        }

        public void CloseConnection()
        {
            if (CurrentTransaction != null)
            {
                CurrentTransaction.Rollback();
            }
            if (CurrentConnection != null)
            {
                CurrentConnection.Close();
                CurrentConnection = null;
                CurrentTransaction = null;
            }
        }

        public void Commit()
        {
            CurrentTransaction.Commit();
            CurrentTransaction = CurrentConnection.BeginTransaction();
        }

        public void Rollback()
        {
            CurrentTransaction.Rollback();
            CurrentTransaction = CurrentConnection.BeginTransaction();
        }

        public virtual String BuildUpdateCommand(String tableName, DbParameterAccessor[] updateAccessors, DbParameterAccessor[] selectAccessors)
        {
            if (updateAccessors.Length == 0)
            {
                throw new ApplicationException("must have at least one field to update. Have you forgotten = after the column name?");
            }
            var s = new StringBuilder("update ").Append(tableName).Append(" set ");
            for (var i = 0; i < updateAccessors.Length; i++)
            {
                if (i > 0) s.Append(", ");
                s.Append(BuildColumnName(updateAccessors[i].DbParameter.SourceColumn)).Append("=");
                s.Append(ParameterPrefix).Append(updateAccessors[i].DbParameter.ParameterName);
            }
            s.Append(" where ");
            for (var i = 0; i < selectAccessors.Length; i++)
            {
                if (i > 0) s.Append(" and ");
                s.Append(BuildColumnName(selectAccessors[i].DbParameter.SourceColumn)).Append("=");
                s.Append(ParameterPrefix).Append(selectAccessors[i].DbParameter.ParameterName);
            }
            return s.ToString();
        }

        public virtual string[] ExtractParamNames(string commandText)
        {
            //dotnet2 does not support sets, so a set is simmulated with a hashmap
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            MatchCollection mc = ParamNameRegex.Matches(commandText);
            for (int i = 0; i < mc.Count; i++) parameters[mc[i].Groups[1].Value] = mc[i].Groups[1].Value;
            string[] arr = new string[parameters.Keys.Count];
            parameters.Keys.CopyTo(arr, 0);
            return arr;
        }

        public virtual int GetExceptionCode(Exception dbException)
        {
            if (dbException is System.Data.Common.DbException)
            {
                System.Console.WriteLine("DBEXCEPTION:" + ((System.Data.Common.DbException)dbException).ErrorCode);
                return ((System.Data.Common.DbException)dbException).ErrorCode;
            }
            else
            {
                System.Console.WriteLine("EXCEPTION:" + dbException.GetType().ToString());
                return 0;
            }
        }

        public virtual String BuildInsertCommand(String tableName, DbParameterAccessor[] accessors)
        {
            var sb = new StringBuilder("insert into ");
            sb.Append(tableName).Append("(");
            var separator = "";
            var values = new StringBuilder();
            foreach (var accessor in accessors)
            {
                sb.Append(separator);
                values.Append(separator);
                sb.Append(BuildColumnName(accessor.DbParameter.SourceColumn));
                values.Append(ParameterPrefix).Append(accessor.DbParameter.ParameterName);
                separator = ",";
            }
            sb.Append(") values (");
            sb.Append(values);
            sb.Append(")");
            return sb.ToString();
        }

        #endregion default implementations for IDbEnvironment methods and properties

        #region inherited methods from IDbEnvironment that have no default implementation

        protected abstract String GetConnectionString(String dataSource, String username, String password);

        protected abstract String GetConnectionString(String dataSource, String username, String password, String database);

        protected abstract Regex ParamNameRegex { get; }

        public abstract DbProviderFactory DbProviderFactory { get; }

        public abstract Dictionary<string, DbParameterAccessor> GetAllProcedureParameters(string procName);

        public abstract Dictionary<string, DbParameterAccessor> GetAllColumns(string tableOrViewName);

        public abstract bool SupportsReturnOnInsert { get; }

        public abstract String IdentitySelectStatement(String tableName);

        #endregion inherited methods from IDbEnvironment that have no default implementation

        protected virtual string BuildColumnName(string sourceColumnName)
        {
            return sourceColumnName;
        }
    }
}