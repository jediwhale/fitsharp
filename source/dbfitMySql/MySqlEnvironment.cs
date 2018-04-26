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
        private static readonly DbProviderFactory dbp = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
        private readonly Regex paramNames = new Regex("@([A-Za-z0-9_]*)");
        private readonly Regex multispaces = new Regex("\\s+");
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
                qry += " lower(db)=@schema and lower(name)=@objname ";
            }
            else
            {
                qry += " (db=database() and lower(name)=@objname)";
            }
            //Console.WriteLine(qry);
            Dictionary<String, DbParameterAccessor> res = ProcedureReadIntoParams(qualifiers, qry);
            if (res.Count == 0) throw new ApplicationException("Cannot read list of parameters for " + procName + " - check spelling and access privileges");
            return res;
        }
        public override Dictionary<String, DbParameterAccessor> GetAllColumns(String tableOrViewName) //done
        {
            String[] qualifiers = NameNormaliser.NormaliseName(tableOrViewName).Split('.');
            String qry = @" select column_name, data_type, character_maximum_length, 'IN' as direction from information_schema.columns where  ";
            if (qualifiers.Length == 2)
            {
                qry += " lower(table_schema)=@schema and lower(table_name)=@objname ";
            }
            else
            {
                qry += @" (table_schema=database() and lower(table_name)=@objname)";
            }
            qry += " order by ordinal_position ";
            //Console.WriteLine(qry);
            Dictionary<String, DbParameterAccessor> res = ReadIntoParams(qualifiers, qry);
            if (res.Count == 0) throw new ApplicationException("Cannot read list of columns for " + tableOrViewName + " - check spelling and access privileges");
            return res;
        }


        private Dictionary<string, DbParameterAccessor> ProcedureReadIntoParams(String[] queryParameters, String query) //done
        {
            var reader = ExecuteParameterQuery(queryParameters, query);
            Dictionary<String, DbParameterAccessor>
                allParams = new Dictionary<string, DbParameterAccessor>();
            reader.Read();
            String procType = (reader.IsDBNull(0)) ? null : reader.GetString(0);
            String paramList = multispaces.Replace(reader.GetString(1)," ");
            String returns = reader.GetString(2);
            reader.Close();
            int position = 0;
            foreach (string param in paramList.Split(','))
            {
                string[] tokens = param.Trim().ToLower().Split(new char[] { ' ' , '(' , ')' });
                int i = 0;
                string direction = "";
                string paramName = "";
                string dataType = "";
                int length = 0;

                if (tokens[i].Equals("in") || tokens[i].Equals("out") || tokens[i].Equals("inout"))
                {
                    direction = tokens[i];
                    i++;
                }
                else
                {
                    direction = "in";
                }
                paramName = tokens[i];
                i++;
                dataType = tokens[i];
                i++;
                
                if (i <= tokens.Length -1 && !Int32.TryParse(tokens[i],out length))
                {
                    length = 0;
                }

                MySqlParameter dp = BuildMySqlParameter(direction, paramName, dataType, length);
                allParams[NameNormaliser.NormaliseName(paramName)] =
                    new DbParameterAccessor(dp, GetDotNetType(dataType), position++, dataType);
            }
            if(procType.Equals("FUNCTION"))
            {
                string[] tokens = returns.Trim().ToLower().Split(new char[] { ' ', '(', ')' });
                string paramName = "?";
                string dataType = tokens[0];
                MySqlParameter dp = BuildMySqlParameter("return", paramName, dataType, -1);
                allParams[NameNormaliser.NormaliseName(paramName)] =
                    new DbParameterAccessor(dp, GetDotNetType(dataType), position++, dataType);
            }
            return allParams;
        }

        private static MySqlParameter BuildMySqlParameter(string direction, string paramName, string dataType, int length)
        {
            MySqlParameter dp = new MySqlParameter();
            dp.Direction = GetParameterDirection(direction);
            if (paramName != null)
            {
                dp.ParameterName = paramName; dp.SourceColumn = paramName;
            }
            else
            {
                dp.Direction = ParameterDirection.ReturnValue;
            }

            dp.MySqlDbType = GetDBType(dataType);
            if (length > 0)
            {
                dp.Size = length;
            }
            else
            {
                if (!ParameterDirection.Input.Equals(dp.Direction) || typeof(String).Equals(GetDotNetType(dataType))) dp.Size = MAX_STRING_SIZE;
            }
            return dp;
        }

        private Dictionary<string, DbParameterAccessor> ReadIntoParams(String[] queryParameters, String query) //done
        {
            var reader = ExecuteParameterQuery(queryParameters, query);
            Dictionary<String, DbParameterAccessor>
                allParams = new Dictionary<string, DbParameterAccessor>();
            int position = 0;
            while (reader.Read())
            {

                String paramName = (reader.IsDBNull(0)) ? null : reader.GetString(0);
                String dataType = reader.GetString(1);
                int length = (reader.IsDBNull(2)) ? 0 : reader.GetInt32(2);
                String direction = reader.GetString(3);
                MySqlParameter dp = BuildMySqlParameter(direction, paramName, dataType, length);
                allParams[NameNormaliser.NormaliseName(paramName)] =
                    new DbParameterAccessor(dp, GetDotNetType(dataType), position++, dataType);
            }
            reader.Close();
            return allParams;
        }

        private IDataReader ExecuteParameterQuery(String[] queryParameters, String query)
        {
            var cnx = CurrentConnection;
            var dc = cnx.CreateCommand();
            dc.Transaction = (DbTransaction)CurrentTransaction;
            dc.CommandText = query;
            dc.CommandType = CommandType.Text;
            if (queryParameters.Length == 2)
            {
                AddInput(dc, "@schema", queryParameters[0]);
                AddInput(dc, "@objname", queryParameters[1]);
            }
            else
            {
                AddInput(dc, "@objname", queryParameters[0]);
            }
            var reader = dc.ExecuteReader();
            return reader;
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
            if ("in".Equals(direction.ToLower())) return ParameterDirection.Input;
            if ("out".Equals(direction.ToLower())) return ParameterDirection.Output;
            if ("inout".Equals(direction.ToLower())) return ParameterDirection.InputOutput;
            if ("return".Equals(direction.ToLower())) return ParameterDirection.ReturnValue;
            throw new NotSupportedException("Direction " + direction + " is not supported");
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
            get { return "@"; }
        }
        public override bool SupportsReturnOnInsert { get { return false; } } //done
        public override String IdentitySelectStatement(string tableName) { return "select last_insert_id();"; } //done

    }
}
