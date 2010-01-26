// Copyright © 2010 Syterra Software Inc. Includes work Copyright (C) Gojko Adzic 2006-2008 http://gojko.net
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

using fit;
using dbfit.util;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace dbfit.fixture {
	public class Insert: ColumnFixture, MemberQueryable {
		private static readonly Regex checkIsImpliedByRegex = new Regex("(\\?|!|\\(\\))$");

		private readonly IDbEnvironment dbEnvironment;
		private DbCommand command;
		private String tableName;

        // array of parameter accessors for the input command
        // this contains only those accessors that really go into input (i.e. not the primary key
        // column if the database does not support return on insert. for databases that support
        // return on insert, this will be the same as columnAccessors
		private DbParameterAccessor[] accessors;
        private ColumnAccessors columnAccessors;
        private bool[] isOutputColumn; 
        public Insert()
        {
            dbEnvironment = DbEnvironmentFactory.DefaultEnvironment;
        }
        public Insert(IDbEnvironment dbEnvironment)
        {
            this.dbEnvironment=dbEnvironment;
        }
		public Insert(IDbEnvironment dbEnvironment, String tableName) {
            this.tableName= tableName;
			this.dbEnvironment = dbEnvironment;
			
		}		
		public override void DoRows(Parse rows) {
            if (String.IsNullOrEmpty(tableName) && Args.Length > 0)
            {
                tableName = Args[0];
            }
			else if (tableName == null) {
					tableName=rows.Parts.Text;
					rows = rows.More;
			}			
			InitParameters(rows.Parts);
			InitCommand();
			base.DoRows(rows);
		}
		private void InitCommand() {
            String insert = dbEnvironment.BuildInsertCommand(tableName, accessors);
            command = dbEnvironment.CreateCommand(insert,
                    CommandType.Text);
            foreach (DbParameterAccessor accessor in accessors) {
                  command.Parameters.Add(accessor.DbParameter);
            }
		}

        public override void Execute() {
            command.ExecuteNonQuery();
        }

        // this method will initialise accessors array from the parameters that
        // really go into the insert command and columnAccessors for all columns
		private void InitParameters(Parse headerCells) {			
			Dictionary<String,DbParameterAccessor> allParams=
				dbEnvironment.GetAllColumns(tableName);
			columnAccessors = new ColumnAccessors();
            isOutputColumn = new bool[headerCells.Size];
            var paramAccessors=new List<DbParameterAccessor>();
			for (int i = 0; headerCells != null; i++, headerCells = headerCells.More) {
				String paramName= NameNormaliser.NormaliseName(headerCells.Text);
                DbParameterAccessor currentColumn;
                try
                {
                    currentColumn = allParams[paramName];
                }
                catch (KeyNotFoundException)
                {
                    Wrong(headerCells);
                    throw new ApplicationException("Cannot find column " + paramName);
                }
                isOutputColumn[i] = checkIsImpliedByRegex.IsMatch(headerCells.Text);
                currentColumn.IsBoundToCheckOperation = isOutputColumn[i];
                columnAccessors.Assign(paramName, currentColumn);
                if (isOutputColumn[i])
                {
                    if (dbEnvironment.SupportsReturnOnInsert)
                    {
                        currentColumn.DbParameter.Direction = ParameterDirection.Output;
                        paramAccessors.Add(currentColumn);
                    }
                    else // don't add to paramAccessors
                    {
                        columnAccessors.Assign(paramName, new IdRetrievalAccessor(dbEnvironment, currentColumn.DotNetType));
                    }
                }
                else // not output
                {
                    currentColumn.DbParameter.Direction = ParameterDirection.Input;
                    paramAccessors.Add(currentColumn);
                }
            }
            accessors = paramAccessors.ToArray();
		}

	    public RuntimeMember Find(IdentifierName memberName, int parameterCount, Type[] parameterTypes) {
	        return columnAccessors.Find(memberName, parameterCount, accessor => memberName.Matches(accessor.Key));
	    }
	}
}
