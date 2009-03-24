// FitNesse.NET
// Copyright © 2007-2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Configuration;
using fit.Engine;
using fitSharp.Fit.Operators;
using fitSharp.Machine.Application;

namespace fit {

	public class FitVersionFixture: Fixture {
	    
	    static FitVersionFixture() {
	        Reset();
	    }

        public override bool IsVisible { get { return false; } }
	    
        public override void DoTable(Parse theTable) {
	        if (Args.Length > 0) {
	            myVersion = Args[0].Trim().ToLower();
                //todo: clean up
                if (myVersion.ToLower().IndexOf("fitlibrary1") >= 0) {
                    Context.Configuration.GetItem<Service>().RemoveOperator(typeof(ParseMemberName).FullName);
                    Context.Configuration.GetItem<Service>().AddOperator(new ParseMemberNameExtended());
                }
	        }
	    }
	    
	    public static bool IsStandard {get { return myVersion.IndexOf("std") >= 0;}}
	    
	    public static void Set(string theVersion) {
            myVersion = (theVersion == null ? string.Empty : theVersion.Trim().ToLower());
        }
	    
	    public static void Reset() {
	        Set(ConfigurationSettings.AppSettings["fitVersion"]);
	    }
	    
        private static string myVersion;
    }
}
