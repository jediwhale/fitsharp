// Copyright © 2012 Syterra Software Inc. Includes work Copyright (C) Gojko Adzic 2006-2008 http://gojko.net
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using dbfit.util;
using fit;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Engine;

namespace dbfit.fixture {
	public class Update: ColumnFixture, MemberQueryable {
	    private readonly IDbEnvironment dbEnvironment;
		private DbCommand command;
		private String tableName;
		private DbParameterAccessor[] updateAccessors;
		private DbParameterAccessor[] selectAccessors;
		private ColumnAccessors columnAccessors;
	    private Parse headerCells;
	    private Parse currentHeader;
		
		public Update(){
			dbEnvironment = DbEnvironmentFactory.DefaultEnvironment;
		}
		public Update(IDbEnvironment dbEnvironment){
			this.dbEnvironment=dbEnvironment;
		}
		public Update(IDbEnvironment dbEnvironment, String tableName){
			this.tableName= tableName;
			this.dbEnvironment = dbEnvironment;
		}		
		public override void DoRows(Parse rows) {
         if (String.IsNullOrEmpty(tableName) && Args.Length > 0){
             tableName = Args[0];
         }
			else if (tableName == null) {
					tableName=rows.Parts.Text;
					rows = rows.More;
			}			
			InitParameters(rows.Parts);
			InitCommand();
		    headerCells = rows.Parts;
            base.DoRows(rows);
		}

        public override void DoCell(Parse cell, int column) {
            currentHeader = headerCells.At(column);
            base.DoCell(cell, column);
        }

		private void InitCommand() {
			String ctext=dbEnvironment.BuildUpdateCommand(tableName,updateAccessors,selectAccessors);
			command = (DbCommand)dbEnvironment.CreateCommand(ctext,CommandType.Text);
         foreach (DbParameterAccessor accessor in updateAccessors) {
                command.Parameters.Add(accessor.DbParameter);
         }
			foreach (DbParameterAccessor accessor in selectAccessors)
			{
				command.Parameters.Add(accessor.DbParameter);
			}
		}

        public override void Execute() {
			command.ExecuteNonQuery();
        }

		private void InitParameters(Parse headerCells) {			
			Dictionary<String,DbParameterAccessor> allParams=
				dbEnvironment.GetAllColumns(tableName);

		    columnAccessors = new ColumnAccessors();
			IList<DbParameterAccessor> selectAccList=new List<DbParameterAccessor>();
			IList<DbParameterAccessor> updateAccList = new List<DbParameterAccessor>();
			for (int i = 0; headerCells != null; i++, headerCells = headerCells.More)
			{
				String paramName= NameNormaliser.NormaliseName(headerCells.Text);
                try
                {
                    DbParameterAccessor acc = allParams[paramName];
                    acc.DbParameter.Direction = ParameterDirection.Input;
                    // allow same column to be used in both sides: 
                    // remap update parameters to u_paramname and select to s_paramname
                    acc = DbParameterAccessor.Clone(acc, dbEnvironment);
                    if (headerCells.Text.EndsWith("="))
                    {
                        acc.DbParameter.ParameterName = acc.DbParameter.ParameterName+"_u";
                        updateAccList.Add(acc);
                        columnAccessors.Assign(paramName +"=", acc);
                    }
                    else
                    {
                        acc.DbParameter.ParameterName = acc.DbParameter.ParameterName+"_s";
                        selectAccList.Add(acc);
                        columnAccessors.Assign(paramName, acc);
                    }
                }
                catch (KeyNotFoundException)
                {
                    Wrong(headerCells);
                    throw new ApplicationException("Cannot find column for " + paramName);
                }
			}
			selectAccessors=new DbParameterAccessor[selectAccList.Count];
			selectAccList.CopyTo(selectAccessors,0);
			updateAccessors = new DbParameterAccessor[updateAccList.Count];
			updateAccList.CopyTo(updateAccessors, 0);			
		}

	    public RuntimeMember Find(MemberSpecification specification) {
	        return columnAccessors.Find(specification, accessor => {
	            var accessorName = accessor.Key.EndsWith("=")
	                                      ? accessor.Key.Substring(0, accessor.Key.Length - 1)
	                                      : accessor.Key;
	            if (!specification.MatchesIdentifierName(accessorName)) return false;
	            if (currentHeader != null && currentHeader.Text.EndsWith("=") && !accessor.Key.EndsWith("=")) return false;
	            if (currentHeader != null && !currentHeader.Text.EndsWith("=") && accessor.Key.EndsWith("=")) return false;
	            return true;
	        });
	    }
	}
}
