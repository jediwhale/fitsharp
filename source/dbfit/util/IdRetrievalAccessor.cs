using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
namespace dbfit.util
{
    public class IdRetrievalAccessor: Accessor
    {
        private IDbEnvironment environment;
        private Type expectedType;
        private String tableName;
        public IdRetrievalAccessor(IDbEnvironment environment, Type expectedType)
        {
            this.environment = environment;
            this.expectedType = expectedType;
        }
        public IdRetrievalAccessor(IDbEnvironment environment, Type expectedType, String tableName)
        {
            this.environment = environment;
            this.expectedType = expectedType;
            this.tableName = tableName;
        }
        public object Get()
        {
            if (environment.SupportsReturnOnInsert)
                throw new ApplicationException(environment.GetType() + 
                    " supports return on insert, IdRetrievalAccessor should not be used");
            var cmd = environment.CreateCommand(environment.IdentitySelectStatement(tableName), CommandType.Text);
         //   Console.WriteLine(environment.IdentitySelectExpression);
            object value = cmd.ExecuteScalar();
            value=Convert.ChangeType(value, expectedType);
            //Console.WriteLine("value=" + value + " of " + value.GetType());
            return (DBNull.Value.Equals(value) ? null : value);
        }
        public void Set(object value)
        {
        }

        public Type DotNetType {
            get { return expectedType; }
        }

        public string Name {
            get { return string.Empty; }
        }
    }
}
