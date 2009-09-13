// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Collections.Generic;
using fit;
using fit.Operators;
using fitSharp.Fit.Model;
using fitSharp.Fit.Operators;

namespace fitnesse.handlers
{
	public class CellHandlerLoaderFixture : Fixture
	{
        private static readonly Dictionary<string, string> renames = new Dictionary<string, string> {
            {"boolhandler", typeof(ParseBoolean).FullName},
            {"emptycellhandler", typeof(ExecuteEmpty).FullName},
            {"exceptionkeywordhandler", typeof(ExecuteException).FullName},
            {"nullkeywordhandler", typeof(ParseNull).FullName},
            {"blankkeywordhandler", typeof(ParseBlank).FullName},
            {"errorkeywordhandler", typeof(ExecuteError).FullName},
            {"endswithhandler", typeof(CompareEndsWith).FullName},
            {"failkeywordhandler", typeof(CompareFail).FullName},
            {"startswithhandler", typeof(CompareStartsWith).FullName},
            {"integralrangehandler", typeof(CompareIntegralRange).FullName},
            {"listhandler", typeof(ExecuteList).FullName},
            {"numericcomparehandler", typeof(CompareNumeric).FullName},
            {"parsecellhandler", typeof(ExecuteParse).FullName},
            {"stringhandler", typeof(CompareString).FullName},
            {"substringhandler", typeof(CompareSubstring).FullName},
            {"symbolsavehandler", typeof(ExecuteSymbolSave).FullName},
            {"symbolrecallhandler", typeof(ParseSymbol).FullName},
            {"tablehandler", typeof(ParseTable).FullName},
            {"regexhandler", typeof(CompareRegEx).FullName}
        };

		public override void DoRow(Parse row)
		{

			switch (GetTypeOfOperation(row))
			{
				case "load":
		            Load(row);
			        break;
				case "remove":
		            Remove(row);
			        break;
                case "reset":
			        Reset(row);
			        break;
			}
		}

	    private void Load(Parse row) {
	        string handler = new GracefulName(row.Parts.More.Text).IdentifierName.ToString();
	        if (renames.ContainsKey(handler)) {
	            Processor.AddOperator(renames[handler]);
	        }
	        //else {
	        //    Configuration.Instance.Add(GetKey(row.Parts.More.More), handler);
            //}
	    }

	    private void Remove(Parse row) {
	        string handler = new GracefulName(row.Parts.More.Text).IdentifierName.ToString();
	        if (renames.ContainsKey(handler)) {
	            Processor.RemoveOperator(renames[handler]);
	        }
	        //else {
	        //    Configuration.Instance.Remove(GetKey(row.Parts.More.More), handler);
            //}
	    }

	    private static void Reset(Parse row) {
	        //Configuration.Instance.Reset(GetKey(row.Parts.More));
	    }

	    //private static string GetKey(Parse theCell) {
        //    if (theCell == null) return Configuration.FitCellHandlersKey;
        //    string name = theCell.Text;
        //    if (name.ToLower() == "core") name = "fit";
        //    return name.ToLower() + ".CellHandlers";
        //}

		private static string GetTypeOfOperation(Parse row)
		{
			return row.Parts.Text.ToLower();
		}

	}
}