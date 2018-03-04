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
            using (DbCommand dc = environment.CreateCommand(statement, CommandType.Text))
            {
                if (dbfit.util.Options.ShouldBindSymbols()) 
                    environment.BindFixtureSymbols(Symbols, dc);
                dc.ExecuteNonQuery();
                environment.Commit()
            }
        }
    }
}
