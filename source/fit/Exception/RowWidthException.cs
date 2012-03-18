// Copyright © 2009 Syterra Software Inc. Includes work © 2003-2006 Rick Mugridge, University of Auckland, New Zealand.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using fitSharp.Fit.Exception;

namespace fitlibrary.exception {

	public class RowWidthException: TableStructureException {
        public RowWidthException(int theExpectedSize)
            : base(string.Format("Row should be {0} cells wide.", theExpectedSize)) {}
	}
}
