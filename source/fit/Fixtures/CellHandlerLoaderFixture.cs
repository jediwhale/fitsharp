// Copyright © 2009,2010 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Model;

namespace fit
{
	public class CellHandlerLoaderFixture : Fixture
	{

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
	        ((Service.Service)Processor).AddCellHandler(handler);
	    }

	    private void Remove(Parse row) {
	        string handler = new GracefulName(row.Parts.More.Text).IdentifierName.ToString();
	        ((Service.Service)Processor).RemoveCellHandler(handler);
	    }

	    private static void Reset(Parse row) {}

		private static string GetTypeOfOperation(Parse row)
		{
			return row.Parts.Text.ToLower();
		}

	}
}