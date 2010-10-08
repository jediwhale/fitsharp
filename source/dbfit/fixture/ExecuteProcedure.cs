// Copyright © 2010 Syterra Software Inc. Includes work Copyright (C) Gojko Adzic 2006-2008 http://gojko.net
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using fit;
using dbfit.util;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace dbfit.fixture
{
    public class ExecuteProcedure : ColumnFixture, MemberQueryable
    {
        private readonly IDbEnvironment dbEnvironment;
        private readonly bool expectException;
        private readonly List<DbParameterAccessor> accessors = new List<DbParameterAccessor>();
        private DbCommand command;
        private String procedureName;
        private int? errorCode;
        private Parse currentRow;
        private ColumnAccessors columnAccessors;

        public ExecuteProcedure()
        {
            dbEnvironment = DbEnvironmentFactory.DefaultEnvironment;
        }
        public ExecuteProcedure(IDbEnvironment dbEnvironment, String procedureName, bool expectException)
        {
            this.procedureName = procedureName;
            this.dbEnvironment = dbEnvironment;
            this.expectException = expectException;
            errorCode = null;
        }
        public ExecuteProcedure(IDbEnvironment dbEnvironment, String procedureName, int errorCode)
        {
            this.procedureName = procedureName;
            this.dbEnvironment = dbEnvironment;
            expectException = true;
            this.errorCode = errorCode;
        }
        public ExecuteProcedure(IDbEnvironment dbEnvironment, String procedureName)
            : this(dbEnvironment, procedureName, false)
        {
        }
        public override void DoRows(Parse rows)
        {
            if (String.IsNullOrEmpty(procedureName)) procedureName = Args[0];
            if (rows != null)
            {
                InitParameters(rows.Parts);
                InitCommand();
                base.DoRows(rows);
            }
            else
            {
                columnAccessors = new ColumnAccessors();
                InitCommand();
                command.ExecuteNonQuery();
            }
        }

        public override void DoRow(Parse row) {
            currentRow = row;
            base.DoRow(row);
        }

        public override void DoCells(Parse cells) {
            ProcessCells(cells, b => !b.IsCheck);
            ProcessCells(cells, b => b.IsCheck);
        }

        private void ProcessCells(Parse cells, Func<Binding, bool> bindingFilter) {
            int i = 0;
            for (Parse cell = cells; cell != null && i < ColumnBindings.Length; cell = cell.More, i++) {
                if (!bindingFilter(ColumnBindings[i])) continue;
                try {
                    DoCell(cell, i);
                }
                catch (Exception e) {
                    TestStatus.MarkException(cells, e);
                }
            }
        }

        public static DbParameterAccessor[] SortAccessors(IEnumerable<DbParameterAccessor> accessors)
        {
            var sortedAccessors = new List<DbParameterAccessor>(accessors);
            for (int i = 0; i < sortedAccessors.Count - 1; i++)
                for (int j = i + 1; j < sortedAccessors.Count; j++)
                {
                    if (sortedAccessors[i].Position <= sortedAccessors[j].Position) continue;
                    DbParameterAccessor x = sortedAccessors[i];
                    sortedAccessors[i] = sortedAccessors[j];
                    sortedAccessors[j] = x;
                }
            return sortedAccessors.ToArray();
        }
        private void InitCommand()
        {
            command = dbEnvironment.CreateCommand(procedureName, CommandType.StoredProcedure);
            DbParameterAccessor[] sortedAccessors = SortAccessors(accessors);

            foreach (DbParameterAccessor accessor in sortedAccessors)
            {
                // in/out params can cause the same parameter to be added twice,
                // check to avoid that
                if (!command.Parameters.Contains(accessor.DbParameter))
                    command.Parameters.Add(accessor.DbParameter);
            }
        }

        public override void Execute() {
            if (expectException) {
                try {
                    command.ExecuteNonQuery();
                    Wrong(currentRow);
                }
                catch (Exception e) {
                    currentRow.Parts.Last.More = new Parse("td", Gray(e.ToString()), null, null);
                    if (errorCode.HasValue) {
                        if (dbEnvironment.GetExceptionCode(e) == errorCode)
                            Right(currentRow);
                        else
                            Wrong(currentRow);
                    }
                    else
                        Right(currentRow);
                }
            }
            else {
                command.ExecuteNonQuery();
            }
        }

        private void InitParameters(Parse headerCells)
        {
            Dictionary<String, DbParameterAccessor> allParams =
                dbEnvironment.GetAllProcedureParameters(procedureName);
            columnAccessors = new ColumnAccessors();
            for (int i = 0; headerCells != null; i++, headerCells = headerCells.More)
            {
                String paramName = NameNormaliser.NormaliseName(headerCells.Text);
                try
                {
                    DbParameterAccessor accessor = DbParameterAccessor.CloneWithSameParameter(allParams[paramName]);
                    accessor.IsBoundToCheckOperation = BindingFactory.CheckIsImpliedBy(headerCells.Text);
                    // sql server quirk. if output parameter is used in an input column, then 
                    // the param should be remapped to IN/OUT
                    if ((!accessor.IsBoundToCheckOperation) &&
                            accessor.DbParameter.Direction == ParameterDirection.Output)
                        accessor.DbParameter.Direction = ParameterDirection.InputOutput;
                    columnAccessors.Assign(paramName, accessor);
                    accessors.Add(accessor);
                }
                catch (KeyNotFoundException)
                {
                    Wrong(headerCells);
                    throw new ApplicationException("Cannot find parameter " + paramName);
                }
            }
        }

        public RuntimeMember Find(IdentifierName memberName, int parameterCount, IList<Type> parameterTypes) {
	        return columnAccessors.Find(memberName, parameterCount, accessor => memberName.Matches(accessor.Key));
        }
    }
}
