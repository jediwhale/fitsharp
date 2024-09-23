// Copyright © 2011 Syterra Software Inc. Includes work Copyright (C) Gojko Adzic 2006-2008 http://gojko.net
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using dbfit.util;
using fitSharp.Machine.Engine;

namespace dbfit
{
    /// <summary>
    /// Implementation of IDbEnvironment that works with SqlServer 2005 and newer versions
    /// </summary>
    public class SqlServerEnvironment : AbstractDbEnvironment
    {
        private const int MAX_STRING_SIZE = 4000;
        protected override String GetConnectionString(String dataSource, String username, String password, String databaseName)
        {
            return String.Format("data source={0};user id={1};password={2};database={3};", dataSource, username, password, databaseName);
        }

        protected override String GetConnectionString(String dataSource, String username, String password)
        {
            return String.Format("Data Source={0}; User ID={1}; Password={2}", dataSource, username, password);
        }

        private static readonly DbProviderFactory dbp = SqlClientFactory.Instance;
        private readonly Regex paramNames = new Regex("@([A-Za-z0-9_]*)");
        protected override Regex ParamNameRegex { get { return paramNames;}}

        public override DbProviderFactory DbProviderFactory
        {
            get { return dbp; }
        }
        public override Dictionary<String, DbParameterAccessor> GetAllProcedureParameters(String procName)
        {
            string dbName = GetDbName(procName);
            string retString = "select p.[name], TYPE_NAME(p.system_type_id) as [Type], p.max_length, p.is_output, p.is_cursor_ref, p.precision, p.scale from ";
            retString += dbName + "sys.parameters p where p.object_id = OBJECT_ID(@objname) ";
            retString += "UNION ALL select '','int',4,1,0,10,0 where EXISTS (SELECT * FROM ";
            retString += dbName + "sys.objects WHERE object_id = OBJECT_ID(@objname) AND type in (N'P', N'PC'))";

            return ReadIntoParams(procName, retString);
        }

        private static string GetDbName(String objectName)
        {
            String[] objNameArr = objectName.Split(new[] { '.' });
            if (objNameArr.Length == 3)
            {
                //the table is in another database
                return objNameArr[0] + ".";
            }
            return "";
        }
        public override Dictionary<String, DbParameterAccessor> GetAllColumns(String tableOrViewOrTypeName)
        {
            string dbName = GetDbName(tableOrViewOrTypeName);
            return ReadIntoParams(tableOrViewOrTypeName,
            @"SELECT c.[name], TYPE_NAME(c.system_type_id) as [Type], c.max_length, 
            0 As is_output, 0 As is_cursor_ref, c.precision, c.scale
            FROM " + dbName + @"sys.columns c 
            WHERE c.object_id = COALESCE(OBJECT_ID(@objname), (SELECT type_table_object_id FROM "+ dbName + @"sys.table_types WHERE user_type_id = TYPE_ID(@objname)))
            ORDER BY column_id" );
        }

        private  Dictionary<string, DbParameterAccessor> ReadIntoParams(String objname, String query)
        {
            if (objname.Contains("."))
            {
                // The object name is multi-part an will not be amended
            }
            else
            {
                objname = "[" + NameNormaliser.NormaliseName(objname) + "]";
            }
            var cnx = (SqlConnection)CurrentConnection;
            SqlCommand dc = cnx.CreateCommand();
            dc.Transaction = (SqlTransaction)CurrentTransaction;
            dc.CommandText = query;
            dc.CommandType = CommandType.Text;
            dc.Parameters.Clear();
            AddInput(dc, "objname", objname);
            DbDataReader reader = dc.ExecuteReader();
            var allParams = new Dictionary<string, DbParameterAccessor>();
            int position=0;
            while (reader.Read())
            {
                String paramName = (reader.IsDBNull(0)) ? null : reader.GetString(0);
                String dataType = reader.GetString(1);
                int length = (reader.IsDBNull(2)) ? 0 : Convert.ToInt32(reader[2]);
                int isOutput = (reader.IsDBNull(3)) ? 0 : Convert.ToInt32(reader[3]);
                byte precision =  Convert.ToByte(reader[5]);
                byte scale = Convert.ToByte(reader[6]);

                var dp = new SqlParameter {Direction = GetParameterDirection(isOutput)};
                if (!String.IsNullOrEmpty(paramName)) { 
						dp.ParameterName = paramName; dp.SourceColumn=paramName; 
				}
                else
                {
                    dp.Direction = ParameterDirection.ReturnValue;
                }
                dp.SqlDbType= GetDBType(dataType);
                String typeName = NormaliseTypeName(dataType);
                if (precision > 0) dp.Precision = precision;         
                if (scale > 0) dp.Scale = scale;
                if ("NTEXT".Equals(typeName)||("TEXT".Equals(typeName)))
                    dp.Size=MAX_STRING_SIZE;
                else if ("NVARCHAR".Equals(typeName) || ("NCHAR".Equals(typeName)))
                {
                    dp.Size = System.Convert.ToInt32(length) / 2;
                }
                else if (length > 0)
                {
                    dp.Size = Convert.ToInt32(length);
                }
                else
                {
                    if (!ParameterDirection.Input.Equals(dp.Direction) || 
                        typeof(String).Equals(GetDotNetType(dataType)))
                        dp.Size = MAX_STRING_SIZE;
                }
                allParams[NameNormaliser.NormaliseName(paramName)] =
                    new DbParameterAccessor(dp, GetDotNetType(dataType), position++, dataType);
            }
            reader.Close();
            dc.Parameters.Clear();
            if (allParams.Count == 0)
                throw new ApplicationException("Cannot read columns/parameters for object " + objname + " - check spelling or access privileges ");
            return allParams;
        }
        private static readonly string[] SingleByteStringTypes = new string[] { "VARCHAR", "CHAR", "TEXT" };
        private static readonly string[] DoubleByteStringTypes = new string[] { "NVARCHAR", "NCHAR", "NTEXT" };
        private static readonly string[] XMLTypes = new string[] { "XML" };
        private static readonly string[] DecimalTypes = new[] { "DECIMAL", "NUMERIC", "MONEY", "SMALLMONEY" };
        private static readonly string[] DateTimeTypes = new[] { "SMALLDATETIME", "DATETIME" };
        private static readonly string[] DateTypes2008 = new[] { "DATETIME2" };
        private static readonly string[] DateTypes = new[] { "DATE" };
        private static readonly string[] TimeTypes = new[] { "TIME" };
        private static readonly string[] DateTimeOffsetTypes = new[] { "DATETIMEOFFSET" };
        private static readonly string[] RefCursorTypes = new[] { "REF" };
        private static readonly string[] Int8Types = new[] { "TINYINT" };
        private static readonly string[] Int16Types = new[] { "SMALLINT" };
        private static readonly string[] Int32Types=new[] {"INT"};
        private static readonly string[] Int64Types = new[] { "BIGINT"};
        private static readonly string[] TimestampTypes = new[] { "ROWVERSION", "TIMESTAMP" };

        private static readonly string[] BooleanTypes = new[] { "BIT" };
        private static readonly string[] BinaryTypes = new[] { "BINARY", "VARBINARY", "IMAGE"};//, };
        private static readonly string[] GuidTypes = new[] { "UNIQUEIDENTIFIER" };
        private static readonly string[] VariantTypes = new[] { "SQL_VARIANT" };
        private static readonly string[] FloatTypes = new[] { "FLOAT" };
        private static readonly string[] RealTypes = new[] { "REAL" };
        private static readonly string[] Structured = new[] { "TABLE TYPE" };

        private static string NormaliseTypeName(string dataType)
        {
            dataType = dataType.ToUpper().Trim();
            int idx = dataType.IndexOf(" ");
            if (idx >= 0) dataType = dataType.Substring(0, idx);
            idx = dataType.IndexOf("(");
            if (idx >= 0) dataType = dataType.Substring(0, idx);
            return dataType;            
        }
        protected static SqlDbType GetDBType(String dataType)
        { 
            //todo:strip everything from first blank
            dataType = NormaliseTypeName(dataType);

            if (Array.IndexOf(SingleByteStringTypes, dataType) >= 0) return SqlDbType.VarChar;
            if (Array.IndexOf(DoubleByteStringTypes, dataType) >= 0) return SqlDbType.NVarChar;
            if (Array.IndexOf(XMLTypes, dataType) >= 0) return SqlDbType.Xml;
            if (Array.IndexOf(DecimalTypes, dataType) >= 0) return SqlDbType.Decimal;
            if (Array.IndexOf(DateTimeTypes, dataType) >= 0) return SqlDbType.DateTime;
            if (Array.IndexOf(DateTypes2008, dataType) >= 0) return SqlDbType.DateTime2;
            if (Array.IndexOf(DateTypes, dataType) >= 0) return SqlDbType.Date;
            if (Array.IndexOf(TimeTypes, dataType) >= 0) return SqlDbType.Time;
            if (Array.IndexOf(DateTimeOffsetTypes, dataType) >= 0) return SqlDbType.DateTimeOffset;
            if (Array.IndexOf(Int8Types, dataType) >= 0) return SqlDbType.TinyInt;
            if (Array.IndexOf(Int16Types, dataType) >= 0) return SqlDbType.SmallInt;
            if (Array.IndexOf(Int32Types, dataType) >= 0) return SqlDbType.Int;
            if (Array.IndexOf(Int64Types, dataType) >= 0) return SqlDbType.BigInt;
            if (Array.IndexOf(BooleanTypes, dataType) >= 0) return SqlDbType.Bit;
            if (Array.IndexOf(BinaryTypes,dataType)>=0) return SqlDbType.VarBinary;
            if (Array.IndexOf(TimestampTypes, dataType) >= 0) return SqlDbType.Timestamp;
            if (Array.IndexOf(GuidTypes, dataType) >= 0) return SqlDbType.UniqueIdentifier;
            if (Array.IndexOf(VariantTypes, dataType) >= 0) return SqlDbType.Variant;
            if (Array.IndexOf(FloatTypes, dataType) >= 0) return SqlDbType.Float;
            if (Array.IndexOf(RealTypes, dataType) >= 0) return SqlDbType.Real;
            if (Array.IndexOf(Structured, dataType) >= 0) return SqlDbType.Structured;
            
            throw new NotSupportedException("Type " + dataType + " is not supported");
        }
        protected static Type GetDotNetType(String dataType)
        {
            dataType = NormaliseTypeName(dataType);
            if (Array.IndexOf(SingleByteStringTypes, dataType) >= 0) return typeof(string);
            if (Array.IndexOf(DoubleByteStringTypes, dataType) >= 0) return typeof(string);
            if (Array.IndexOf(XMLTypes, dataType) >= 0) return typeof(string);
            if (Array.IndexOf(DecimalTypes, dataType) >= 0) return typeof(decimal);
            if (Array.IndexOf(Int8Types, dataType) >= 0) return typeof(byte);
            if (Array.IndexOf(Int16Types, dataType) >= 0) return typeof(Int16);
            if (Array.IndexOf(Int32Types, dataType) >= 0) return typeof(Int32);
            if (Array.IndexOf(Int64Types, dataType) >= 0) return typeof(Int64);
            if (Array.IndexOf(DateTimeTypes, dataType) >= 0) return typeof(DateTime);
            if (Array.IndexOf(DateTypes, dataType) >= 0) return typeof(DateTime);
            if (Array.IndexOf(DateTypes2008, dataType) >= 0) return typeof(DateTime);
            if (Array.IndexOf(DateTimeOffsetTypes, dataType) >= 0) return typeof(DateTimeOffset);
            if (Array.IndexOf(RefCursorTypes, dataType) >= 0) return typeof(DataTable);
            if (Array.IndexOf(BooleanTypes, dataType) >= 0) return typeof(bool);
            if (Array.IndexOf(BinaryTypes, dataType) >= 0) return typeof(byte[]);
            if (Array.IndexOf(TimestampTypes, dataType) >= 0) return typeof(byte[]);
            if (Array.IndexOf(GuidTypes, dataType) >= 0) return typeof(Guid);
            if (Array.IndexOf(VariantTypes, dataType) >= 0) return typeof(string);
            if (Array.IndexOf(FloatTypes, dataType) >= 0) return typeof(double);
            if (Array.IndexOf(RealTypes, dataType) >= 0) return typeof(float);
            if (Array.IndexOf(TimeTypes, dataType) >= 0) return typeof(TimeSpan);
            if (Array.IndexOf(Structured, dataType) >= 0) return typeof(DataTable);
            
            throw new NotSupportedException(".net Type " + dataType + " is not supported");
        }
        private static ParameterDirection GetParameterDirection(int isOutput) {
            return isOutput==1 ? ParameterDirection.Output : ParameterDirection.Input;
        }

        public override bool SupportsReturnOnInsert { get { return false; } }
        public override String IdentitySelectStatement(string tableName) {
            return tableName == null ? "select @@identity" : "select IDENT_CURRENT('" + tableName + "')";
        }

        public override int GetExceptionCode(Exception dbException) {
            if (dbException is SqlException)
            {
                Console.WriteLine("SQL Exception " + ((SqlException)dbException).Number);
                return ((SqlException)dbException).Number;
            }
            return base.GetExceptionCode(dbException);
        }

        public override string ParameterPrefix
		 {
			 get { return "@";}
		 }

        protected override string BuildColumnName(string sourceColumnName)
        {
            return "[" + sourceColumnName + "]";
        }

        protected override void AddInput(IDbCommand dbCommand, String name, Object value)
        {
            SqlParameter dbParameter;
            var cmd = (SqlCommand)dbCommand;

            var parameter = value as TableTypeParameter;
            if (parameter != null)
            {
                dbParameter = cmd.Parameters.AddWithValue(name, parameter.Datatable );
                dbParameter.Direction = ParameterDirection.Input;
                dbParameter.SqlDbType = SqlDbType.Structured ;
                dbParameter.TypeName = parameter.Tabletype;
            }
            else
            {
                dbParameter = cmd.Parameters.AddWithValue(name, (value ?? DBNull.Value));
                dbParameter.Direction = ParameterDirection.Input;
            }                
        }
        public override void BindFixtureSymbols(Symbols symbols, IDbCommand dc)
        {
            foreach (String paramName in ExtractParamNames(dc.CommandText))
            {
                AddInput(dc, paramName, symbols.GetValueOrDefault(paramName, null));
            }
        }

        public override IDbCommand CreateCommand(string statement, CommandType commandType)
        {
            if (CurrentConnection == null) throw new ApplicationException("Not connected to database");

            var cnx = CurrentConnection;
            var dc = cnx.CreateCommand();
            dc.CommandText = statement.Replace("\r", " ").Replace("\n", " ");
            dc.CommandType = commandType;
            dc.Transaction = CurrentTransaction;
            dc.CommandTimeout = Options.CommandTimeOut;
            return dc;
        }
    }
}
