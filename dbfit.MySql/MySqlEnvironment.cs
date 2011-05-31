// Copyright © 2011 Ed Harper Includes work Copyright (C) Gojko Adzic 2006-2008 http://gojko.net
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using MySql.Data.Types;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using dbfit.util;

namespace dbfit
{
    /// <summary>
    /// Implementation of IDbEnvironment that works with MySql
    /// </summary>
    public class MySqlEnvironment : AbstractDbEnvironment
    {
        private const int MAX_STRING_SIZE = 65535;
        protected override String GetConnectionString(String dataSource, String username, String password, String databaseName)
        {
            return String.Format("data source={0};user id={1};password={2};database={3};", dataSource, username, password, databaseName);
        }

        protected override String GetConnectionString(String dataSource, String username, String password)
        {
            return String.Format("Data Source={0}; User ID={1}; Password={2}", dataSource, username, password);
        }
        private static readonly DbProviderFactory dbp = DbProviderFactories.GetFactory("MySql.Data");
        private readonly Regex paramNames = new Regex("[?]([A-Za-z0-9_]*)");
        protected override Regex ParamNameRegex { get { return paramNames; } }

        public override DbProviderFactory DbProviderFactory
        {
            get { return dbp; }
        }
        public override Dictionary<String, DbParameterAccessor> GetAllProcedureParameters(String procName) //done
        {
            String[] qualifiers = NameNormaliser.NormaliseName(procName).Split('.');
            String qry = " select type,param_list,returns from mysql.proc where ";
            if (qualifiers.Length == 2)
            {
                qry += " lower(db)=?0 and lower(name)=?1 ";
            }
            else
            {
                qry += " (db=database() and lower(name)=?1)";
            }
            //Console.WriteLine(qry);
            Dictionary<String, DbParameterAccessor> res = ReadIntoParams(qualifiers, qry);
            if (res.Count == 0) throw new ApplicationException("Cannot read list of parameters for " + procName + " - check spelling and access privileges");
            return res;
        }
        public override Dictionary<String, DbParameterAccessor> GetAllColumns(String tableOrViewName) //done
        {
            String[] qualifiers = NameNormaliser.NormaliseName(tableOrViewName).Split('.');
            String qry = @" select column_name, data_type, character_maximum_length 
				'IN' as direction from information_schema.columns where  ";
            if (qualifiers.Length == 2)
            {
                qry += " lower(table_schema)=?0 and lower(table_name)=?1 ";
            }
            else
            {
                qry += @" 
					(table_schema=database() and lower(table_name)=?1)";
            }
            qry += " order by ordinal_position ";
            Dictionary<String, DbParameterAccessor> res = ReadIntoParams(qualifiers, qry);
            if (res.Count == 0) throw new ApplicationException("Cannot read list of columns for " + tableOrViewName + " - check spelling and access privileges");
            return res;
        }

        private Dictionary<string, DbParameterAccessor> ReadIntoParams(String[] queryParameters, String query)
        {
            DbCommand dc = CurrentConnection.CreateCommand();
            dc.Transaction = CurrentTransaction;
            dc.CommandText = query;
            dc.CommandType = CommandType.Text;
            for (int i = 0; i < queryParameters.Length; i++)
            {
                AddInput(dc, ":" + i, queryParameters[i].ToUpper());
            }
            DbDataReader reader = dc.ExecuteReader();
            Dictionary<String, DbParameterAccessor>
                allParams = new Dictionary<string, DbParameterAccessor>();
            int position = 0;
            while (reader.Read())
            {

                String paramName = (reader.IsDBNull(0)) ? null : reader.GetString(0);
                String dataType = reader.GetString(1);
                int length = (reader.IsDBNull(2)) ? 0 : reader.GetInt32(2);
                String direction = reader.GetString(3);
                OracleParameter dp = new OracleParameter();
                dp.Direction = GetParameterDirection(direction);
                if (paramName != null)
                {
                    dp.ParameterName = paramName; dp.SourceColumn = paramName;
                }
                else
                {
                    dp.Direction = ParameterDirection.ReturnValue;
                }

                dp.OracleType = GetDBType(dataType);
                if (length > 0)
                {
                    dp.Size = length;

                }
                else
                {
                    if (!ParameterDirection.Input.Equals(dp.Direction) || typeof(String).Equals(GetDotNetType(dataType)))
                        dp.Size = 4000;
                }
                allParams[NameNormaliser.NormaliseName(paramName)] =
                    new DbParameterAccessor(dp, GetDotNetType(dataType), position++, dataType);
            }
            return allParams;
        }
        //datatypes done
        private static string[] StringTypes = new string[] { "VARCHAR", "CHAR", "TEXT" };
        private static string[] IntTypes = new string[] { "TINYINT", "SMALLINT", "MEDIUMINT", "INT", "INTEGER" };
        private static string[] LongTypes = new string[] { "BIGINT", "INTEGER UNSIGNED", "INT UNSIGNED" };
        private static string[] FloatTypes = new string[] { "FLOAT" };
        private static string[] DoubleTypes = new string[] { "DOUBLE" };
        private static string[] DecimalTypes = new string[] { "DECIMAL", "DEC" };
        private static string[] DateTypes = new string[] { "DATE" };
        private static string[] TimestampTypes = new string[] { "TIMESTAMP", "DATETIME" };
        //\done

        private static string NormaliseTypeName(string dataType) //done
        {
            dataType = dataType.ToUpper().Trim();
            return dataType;
        }
        private static MySqlDbType GetDBType(String dataType) //done
        {
            //todo:strip everything from first blank
            dataType = NormaliseTypeName(dataType);

            if (Array.IndexOf(StringTypes, dataType) >= 0) return MySqlDbType.VarChar;
            if (Array.IndexOf(IntTypes, dataType) >= 0) return MySqlDbType.Int32;
            if (Array.IndexOf(LongTypes, dataType) >= 0) return MySqlDbType.Int64;
            if (Array.IndexOf(FloatTypes, dataType) >= 0) return MySqlDbType.Float;
            if (Array.IndexOf(DoubleTypes, dataType) >= 0) return MySqlDbType.Double;
            if (Array.IndexOf(DecimalTypes, dataType) >= 0) return MySqlDbType.Decimal;
            if (Array.IndexOf(DateTypes, dataType) >= 0) return MySqlDbType.Date;
            if (Array.IndexOf(TimestampTypes, dataType) >= 0) return MySqlDbType.DateTime;
            throw new NotSupportedException("Type " + dataType + " is not supported");
        }
        private static Type GetDotNetType(String dataType) //done
        {
            dataType = NormaliseTypeName(dataType);
            if (Array.IndexOf(StringTypes, dataType) >= 0) return typeof(string);
            if (Array.IndexOf(IntTypes, dataType) >= 0) return typeof(Int32);
            if (Array.IndexOf(LongTypes, dataType) >= 0) return typeof(long);
            if (Array.IndexOf(FloatTypes, dataType) >= 0) return typeof(float);
            if (Array.IndexOf(DoubleTypes, dataType) >= 0) return typeof(double);
            if (Array.IndexOf(DecimalTypes, dataType) >= 0) return typeof(decimal);
            if (Array.IndexOf(DateTypes, dataType) >= 0) return typeof(DateTime);
            if (Array.IndexOf(TimestampTypes, dataType) >= 0) return typeof(DateTime);
            throw new NotSupportedException("Type " + dataType + " is not supported");
        }
        private static ParameterDirection GetParameterDirection(String direction)
        {
            if ("IN".Equals(direction)) return ParameterDirection.Input;
            if ("OUT".Equals(direction)) return ParameterDirection.Output;
            if ("IN/OUT".Equals(direction)) return ParameterDirection.InputOutput;
            //todo return val
            throw new NotSupportedException("Direction " + direction + " is not supported");
        }
        public override String BuildInsertCommand(String tableName, DbParameterAccessor[] accessors)
        {
            StringBuilder sb = new StringBuilder("insert into ");
            sb.Append(tableName).Append("(");
            String comma = "";
            String retComma = "";

            StringBuilder values = new StringBuilder();
            StringBuilder retNames = new StringBuilder();
            StringBuilder retValues = new StringBuilder();

            foreach (DbParameterAccessor accessor in accessors)
            {
                if (!accessor.IsBoundToCheckOperation)
                {
                    sb.Append(comma);
                    values.Append(comma);
                    sb.Append(accessor.DbParameter.SourceColumn);
                    values.Append(":").Append(accessor.DbParameter.ParameterName);
                    comma = ",";
                }
                else
                {
                    retNames.Append(retComma);
                    retValues.Append(retComma);
                    retNames.Append(accessor.DbParameter.SourceColumn);
                    retValues.Append(":").Append(accessor.DbParameter.ParameterName);
                    retComma = ",";
                }
            }
            sb.Append(") values (");
            sb.Append(values);
            sb.Append(")");
            if (retValues.Length > 0)
            {
                sb.Append(" returning ").Append(retNames).Append(" into ").Append(retValues);
            }
            return sb.ToString();
        }
        public override int GetExceptionCode(Exception dbException) //done
        {
            if (dbException is MySql.Data.MySqlClient.MySqlException)
                return ((MySql.Data.MySqlClient.MySqlException)dbException).ErrorCode;
            else if (dbException is System.Data.Common.DbException)
                return ((System.Data.Common.DbException)dbException).ErrorCode;
            else return 0;
        }
        public override String ParameterPrefix //done
        {
            get { return "?"; }
        }
        public override bool SupportsReturnOnInsert { get { return false; } } //done
        public override String IdentitySelectStatement(string tableName) { return "select last_insert_id()"; } //done

    }
}
