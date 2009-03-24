// FitLibrary for FitNesse .NET.
// Copyright (c) 2006 Syterra Software Inc. Released under the terms of the GNU General Public License version 2 or later.
// Based on designs from Fit (c) 2002 Cunningham & Cunningham, Inc., FitNesse by Object Mentor Inc., FitLibrary (c) 2003-2006 Rick Mugridge, University of Auckland, New Zealand.

using System;

namespace fitlibrary.exception {

    public class TableStructureException: ApplicationException {

        public TableStructureException(string theMessage)
            : base(string.Format("This table is invalid for this fixture: {0}", theMessage)) {}
    }
}
