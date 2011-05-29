// 
// <author>Ori Helman</author>
// <date>12 Aug 2010</date>
// <summary>Contains an implementation for the Sybase database. It was tested with Sybase 12.5.
//          THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND.
// </summary>

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using dbfit.util;
using Sybase.Data.AseClient;

namespace dbfit
{
    public class SybaseEnvironment : AbstractDbEnvironment
    {
        private const int MAX_STRING_SIZE = 32767;
        public override string ParameterPrefix
        {
            get { return "@"; }
        }

        protected override string GetConnectionString(string dataSource, string username, string password)
        {
            return String.Format("Data Source={0}; User ID={1}; pwd={2}", dataSource, username, password);
        }

        protected override string GetConnectionString(string dataSource, string username, string password, string database)
        {
            return String.Format("Data Source={0}; User ID={1}; pwd={2}; DatabaseName={3}", dataSource, username, password, database);
        }
        
        private Regex paramNames = new Regex("@([A-Za-z0-9_]*)");
        private static DbProviderFactory dbp = new AseClientFactory();

        protected override Regex ParamNameRegex
        {

            get { return paramNames; }
        }
        
        protected override void AddInput(DbCommand dbCommand, String name, Object value)
        {
            DbParameter dbParameter = dbCommand.CreateParameter();
            dbParameter.Direction = ParameterDirection.Input;
            if (!name.StartsWith(ParameterPrefix))
                dbParameter.ParameterName = ParameterPrefix + name;
            else
                dbParameter.ParameterName = name;                
            dbParameter.Value = (value == null ? DBNull.Value : value);
            dbCommand.Parameters.Add(dbParameter);
        }
        

        public override DbProviderFactory DbProviderFactory
        {
            get { return dbp; }
        }

        public override string IdentitySelectStatement(string tableName) {
            return tableName == null ? "select @@identity" : "select IDENT_CURRENT('" + tableName + "')";
        }

        public override Dictionary<string, DbParameterAccessor> GetAllProcedureParameters(string procName)
        {
            return ReadIntoParams(procName,
@"	select  c.name column_name
		,	t.name type_name
		,	c.length
        ,	c.status2
		,	c.prec
		,	c.scale
	from	sysobjects  o
	join    syscolumns	c	on	o.id = c.id
	join    systypes    t   on  t.usertype = c.usertype
	where	o.type = 'P'
	and o.name=@objname
            ");
        }

        public override Dictionary<string, DbParameterAccessor> GetAllColumns(string tableOrViewName)
        {
            return ReadIntoParams(tableOrViewName,
@"	select  c.name column_name
		,	t.name type_name
		,	c.length
        ,	c.status2
		,	c.prec
		,	c.scale
	from	sysobjects  o
	join    syscolumns	c	on	o.id = c.id
	join    systypes    t   on  t.usertype = c.usertype
	where	o.type in ('U','V')
	and o.name=@objname
            ");
        }

        public override bool SupportsReturnOnInsert
        {
            get { return false;}
        }


        public override int GetExceptionCode(Exception dbException)
        {

            if (dbException is Sybase.Data.AseClient.AseException)
            {
                AseException ae = dbException as AseException;
                if (ae.Errors != null)
                {
                    AseError aseError = ae.Errors[0];
                    Console.WriteLine("Sybase Exception code" + aseError.MessageNumber);
                    return aseError.MessageNumber;
                }
            }
            
            return base.GetExceptionCode(dbException);
        }

        private Dictionary<string, DbParameterAccessor> ReadIntoParams(String objname, String query)
        {
            if (objname.Contains("."))
            {
                String[] schemaAndName = objname.Split(new char[] { '.' }, 2);
                objname =  schemaAndName[0] + ".." + schemaAndName[1] ;
            }
            else
            {
                objname =  NameNormaliser.NormaliseName(objname) ;
            }
            DbCommand dc = CurrentConnection.CreateCommand();
            dc.Transaction = CurrentTransaction;
            dc.CommandText = query;
            dc.CommandType = CommandType.Text;
            AddInput(dc, "@objname", objname);
            DbDataReader reader = dc.ExecuteReader();
            Dictionary<String, DbParameterAccessor> allParams = new Dictionary<string, DbParameterAccessor>();
            int position = 0;
            while (reader.Read())
            {

                String paramName = (reader.IsDBNull(0)) ? null : reader.GetString(0);
                String dataType = reader.GetString(1);
                int length = (reader.IsDBNull(2)) ? 0 : System.Convert.ToInt32(reader[2]);
                int isOutput = (reader.IsDBNull(3)) ? 0 : System.Convert.ToInt32(reader[3]);
                byte precision = (reader.IsDBNull(4)) ? Convert.ToByte(0) : System.Convert.ToByte(reader[4]);
                byte scale = (reader.IsDBNull(5)) ? Convert.ToByte(0) : System.Convert.ToByte(reader[5]);

                AseParameter dp = new AseParameter();
                dp.Direction = GetParameterDirection(isOutput);
                if (!String.IsNullOrEmpty(paramName))
                {
                    if (!paramName.StartsWith(ParameterPrefix))
                        dp.ParameterName = ParameterPrefix + paramName; 
                    else
                        dp.ParameterName = paramName; 
                    dp.SourceColumn = paramName;
                }
                else
                {
                    dp.Direction = ParameterDirection.ReturnValue;
                }
                dp.AseDbType = GetDBType(dataType);
                String typeName = NormaliseTypeName(dataType);
                if (precision > 0) dp.Precision = precision;
                if (scale > 0) dp.Scale = scale;
                if ("NTEXT".Equals(typeName) || ("TEXT".Equals(typeName)))
                    dp.Size = MAX_STRING_SIZE;
                else if (length > 0)
                {
                    dp.Size = System.Convert.ToInt32(length);
                }
                else
                {
                    if (!ParameterDirection.Input.Equals(dp.Direction) ||
                        typeof(String).Equals(GetDotNetType(dataType)))
                        dp.Size = MAX_STRING_SIZE;
                }
                allParams[NameNormaliser.NormaliseName(paramName)] = new DbParameterAccessor(dp, GetDotNetType(dataType), position++, dataType);
            }
            reader.Close();
            return allParams;
        }

        private static ParameterDirection GetParameterDirection(int isOutput)
        {
            if (isOutput == 2) return ParameterDirection.Output;
            else return ParameterDirection.Input;
        }

        private static string[] StringTypes = new string[] { "VARCHAR", "NVARCHAR", "CHAR", "NCHAR", "TEXT", "NTEXT", "XML" };
        private static string[] DecimalTypes = new string[] { "DECIMAL", "NUMERIC", "MONEY", "SMALLMONEY" };
        private static string[] DateTypes = new string[] { "SMALLDATETIME", "DATETIME", "TIMESTAMP" };
        private static string[] RefCursorTypes = new string[] { "REF" };
        private static string[] Int32Types = new string[] { "INT" };
        private static string[] Int16Types = new string[] { "TINYINT", "SMALLINT" };
        private static string[] Int64Types = new string[] { "BIGINT" };

        private static string[] BooleanTypes = new string[] { "BIT" };
        private static string[] BinaryTypes = new string[] { "BINARY", "VARBINARY", "IMAGE" };
        private static string[] GuidTypes = new string[] { "UNIQUEIDENTIFIER" };
        private static string[] VariantTypes = new string[] { "SQL_VARIANT" };
        private static string[] FloatTypes = new String[] { "REAL", "FLOAT" };
        private static string NormaliseTypeName(string dataType)
        {
            dataType = dataType.ToUpper().Trim();
            int idx = dataType.IndexOf(" ");
            if (idx >= 0) dataType = dataType.Substring(0, idx);
            idx = dataType.IndexOf("(");
            if (idx >= 0) dataType = dataType.Substring(0, idx);
            return dataType;

        }
        protected static AseDbType GetDBType(String dataType)
        {
            //todo:strip everything from first blank
            dataType = NormaliseTypeName(dataType);

            if (Array.IndexOf(StringTypes, dataType) >= 0) return AseDbType.VarChar;
            if (Array.IndexOf(DecimalTypes, dataType) >= 0) return AseDbType.Decimal;
            if (Array.IndexOf(DateTypes, dataType) >= 0) return AseDbType.DateTime;
            if (Array.IndexOf(Int32Types, dataType) >= 0) return AseDbType.Integer;
            if (Array.IndexOf(Int16Types, dataType) >= 0) return AseDbType.Integer;
            if (Array.IndexOf(Int64Types, dataType) >= 0) return AseDbType.Integer;
            if (Array.IndexOf(BooleanTypes, dataType) >= 0) return AseDbType.Bit;
            if (Array.IndexOf(BinaryTypes, dataType) >= 0) return AseDbType.VarBinary;
            //if (Array.IndexOf(RefCursorTypes, dataType) >= 0) return OracleType.Cursor;
            if (Array.IndexOf(GuidTypes, dataType) >= 0) return AseDbType.VarChar;
            if (Array.IndexOf(VariantTypes, dataType) >= 0) return AseDbType.VarChar;
            if (Array.IndexOf(FloatTypes, dataType) >= 0) return AseDbType.Decimal;

            throw new NotSupportedException("Type " + dataType + " is not supported");
        }
        protected static Type GetDotNetType(String dataType)
        {
            dataType = NormaliseTypeName(dataType);
            if (Array.IndexOf(StringTypes, dataType) >= 0) return typeof(string);
            if (Array.IndexOf(DecimalTypes, dataType) >= 0) return typeof(decimal);
            if (Array.IndexOf(Int32Types, dataType) >= 0) return typeof(Int32);
            if (Array.IndexOf(Int16Types, dataType) >= 0) return typeof(Int16);
            if (Array.IndexOf(Int64Types, dataType) >= 0) return typeof(Int64);
            if (Array.IndexOf(DateTypes, dataType) >= 0) return typeof(DateTime);
            if (Array.IndexOf(RefCursorTypes, dataType) >= 0) return typeof(DataTable);
            if (Array.IndexOf(BooleanTypes, dataType) >= 0) return typeof(bool);
            if (Array.IndexOf(BinaryTypes, dataType) >= 0) return typeof(byte[]);
           // if (Array.IndexOf(GuidTypes, dataType) >= 0) return typeof(System.Guid);
            if (Array.IndexOf(GuidTypes, dataType) >= 0) return typeof(string);
            if (Array.IndexOf(VariantTypes, dataType) >= 0) return typeof(string);
            if (Array.IndexOf(FloatTypes, dataType) >= 0) return typeof(double);

            throw new NotSupportedException("Type " + dataType + " is not supported");
        }


 

    }
}
