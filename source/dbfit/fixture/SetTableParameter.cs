using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using fit;
using dbfit.util;
using fitSharp.Fit.Operators;
using fitSharp.Fit.Service;
using fitSharp.Machine.Engine;

namespace dbfit.fixture
{
    public class SetTableParameter : ColumnFixture, MemberQueryable
    {
        private readonly IDbEnvironment dbEnvironment;
        private String parameterTableName;
        private String parameterTableType;
        private Symbols symbols;

        private DbParameterAccessor[] accessors;
        private ColumnAccessors columnAccessors;
        private bool[] isOutputColumn;
        private DataTable table;

        public SetTableParameter()
        {
            dbEnvironment = DbEnvironmentFactory.DefaultEnvironment;
        }
        public SetTableParameter(IDbEnvironment dbEnvironment)
        {
            this.dbEnvironment = dbEnvironment;
        }
        public SetTableParameter(IDbEnvironment dbEnvironment, Symbols symbols, String parameterTableName, String parameterTableType)
        {
            this.parameterTableName = parameterTableName;
            this.parameterTableType = parameterTableType;
            this.dbEnvironment = dbEnvironment;
            this.symbols = symbols;
            table = new DataTable();
        }

        public override void DoRows(Parse rows)
        {
            if (String.IsNullOrEmpty(parameterTableName) && Args.Length > 0)
            {
                parameterTableName = Args[0];
            }
            else if (parameterTableName == null)
            {
                parameterTableName = rows.Parts.Text;
                rows = rows.More;
            }
            InitParameters(rows.Parts);
            
            symbols.Save(parameterTableName, new TableTypeParameter(parameterTableType, table));

            base.DoRows(rows);
        }
        public override void DoRow(Parse row)
        {
            HasExecuted = false;
            try
            {
                Reset();
                base.DoRow(row);
                if (!HasExecuted)
                {
                    var CurrentRow = table.NewRow();
                    foreach (DbParameterAccessor accessor in accessors)
                    {
                        CurrentRow[accessor.DbParameter.ParameterName] = accessor.DbParameter.Value;
                    }
                    table.Rows.Add(CurrentRow);
                }                    
            }
            catch (Exception e)
            {
                TestStatus.MarkException(row.Leaf, e);
            }
        }

        // this method will initialise accessors array from the parameters that
        // really go into the insert command and columnAccessors for all columns
        private void InitParameters(Parse headerCells)
        {
            Dictionary<String, DbParameterAccessor> allParams =
                dbEnvironment.GetAllColumns(parameterTableType);
            columnAccessors = new ColumnAccessors();
            isOutputColumn = new bool[headerCells.Size];
            var paramAccessors = new List<DbParameterAccessor>();
            for (int i = 0; headerCells != null; i++, headerCells = headerCells.More)
            {
                String paramName = NameNormaliser.NormaliseName(headerCells.Text);
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
                isOutputColumn[i] = BindingFactory.CheckIsImpliedBy(headerCells.Text);
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
                        //columnAccessors.Assign(paramName, new IdRetrievalAccessor(dbEnvironment, currentColumn.DotNetType));
                        columnAccessors.Assign(paramName, new IdRetrievalAccessor(dbEnvironment, currentColumn.DotNetType, parameterTableType));
                    }
                }
                else // not output
                {
                    currentColumn.DbParameter.Direction = ParameterDirection.Input;
                    paramAccessors.Add(currentColumn);
                }

                table.Columns.Add(currentColumn.DbFieldName, currentColumn.DotNetType);
            }
            accessors = paramAccessors.ToArray();
        }

        public RuntimeMember Find(MemberSpecification specification)
        {
            return columnAccessors.Find(specification, accessor => specification.MatchesIdentifierName(accessor.Key));
        }
        
    }
}
