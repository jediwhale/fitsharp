using System;
using System.Collections.Generic;
using System.Text;
using fit;
using System.Data;
using System.Data.Common;
namespace dbfit.fixture
{
    public class ExecuteDDL:fit.Fixture
    {
        private IDbEnvironment environment;
        private String statement;
        public ExecuteDDL()
        {
            environment = DbEnvironmentFactory.DefaultEnvironment;
        }
        public ExecuteDDL(IDbEnvironment environment, String statement)
        {
            this.environment = environment;
            this.statement = statement;
        }
        public override void DoRows(Parse rows)
        {
            if (String.IsNullOrEmpty(statement))
                statement = Args[0];
            using (var dc = environment.CreateCommand(statement, CommandType.Text))
            {
                dc.ExecuteNonQuery();
            }
        }
    }
}
