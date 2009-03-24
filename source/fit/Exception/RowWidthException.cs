// FitLibrary for FitNesse .NET.
// Copyright (c) 2006 Syterra Software Inc. Released under the terms of the GNU General Public License version 2 or later.
// Based on designs from Fit (c) 2002 Cunningham & Cunningham, Inc., FitNesse by Object Mentor Inc., FitLibrary (c) 2003-2006 Rick Mugridge, University of Auckland, New Zealand.

using fit;

namespace fitlibrary.exception {

	public class RowWidthException: TableStructureException {
        public RowWidthException(int theExpectedSize)
            : base(string.Format("Row should be {0} cells wide.", theExpectedSize)) {}
	}
}
