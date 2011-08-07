// Copyright © 2010 Syterra Software Inc. Includes work Copyright (C) Gojko Adzic 2006-2008 http://gojko.net
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fitSharp.Fit.Model;
using fitSharp.Machine.Engine;

namespace dbfit.fixture
{
    /// <summary>
    /// Utility fixture that provides database control features in standalone mode. This fixture
    /// should be used to connect/disconnect and manipulate transactions when the flow mode
    /// cannot be used. It is also used to control dbfit options and advanced features. 
    /// 
    /// If it has a fixture argument, that argument is used to initialise a new IDbEnvironment object. 
    /// The argument value should be the type of required IDbInstance implementation, like 
    /// ORACLE, SQLSERVER, SQLSERVER2000 or DB2. That object is then stored for later use in the 
    /// DbEnvironmentFactory singleton.
    /// 
    /// If there are no fixture arguments, then the last previously initialised
    /// IDbEnvironment object is used.
    /// 
    /// </summary>
    public class DatabaseEnvironment : fitlibrary.SequenceFixture
    {
		 public DatabaseEnvironment(){
			mySystemUnderTest=DbEnvironmentFactory.DefaultEnvironment;
	     }
		 public override void DoTable(fit.Parse theTable)
		 {
			if (Args.Length>0){
				String requestedEnv=Args[0].ToUpper().Trim();
        IDbEnvironment env; 
        if ("ORACLE".Equals(requestedEnv))
          env = MakeEnvironment("dbfit.OracleEnvironment");
        else if ("SQLSERVER".Equals(requestedEnv))
          env = MakeEnvironment("dbfit.SqlServerEnvironment");
        else if ("SQLSERVER2000".Equals(requestedEnv))
          env = MakeEnvironment("dbfit.SqlServer2000Environment");
        else if ("SYBASE".Equals(requestedEnv))
          env = MakeEnvironment("dbfit.SybaseEnvironment");
        else if ("MYSQL".Equals(requestedEnv))
          env = MakeEnvironment("dbfit.MySqlEnvironment");
        /*  else if ("DB2".Equals(requestedEnv))
          env = Processor.Create("DB2Environment", new TreeList<Cell>()); */
        else
          throw new ApplicationException("DB Environment not supported " + requestedEnv);
				DbEnvironmentFactory.DefaultEnvironment=env;
				mySystemUnderTest=DbEnvironmentFactory.DefaultEnvironment;
			}
			base.DoTable(theTable);
		 }

      private IDbEnvironment MakeEnvironment(string name) {
        return Processor.Create(name, new CellTree()).GetValue<IDbEnvironment>();
      }

      /// <summary>
        /// set the value of DbFit options. See dbfit.util.options for more information
        /// </summary>
        /// <param name="option">option name</param>
        /// <param name="value">option vaue</param>
        public void SetOption(String option, String value)
        {
            util.Options.SetOption(Processor, option, value);
        }
    }
}
