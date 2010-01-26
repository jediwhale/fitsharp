// Copyright © 2010 Syterra Software Inc. Includes work Copyright (C) Gojko Adzic 2006-2008 http://gojko.net
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;

namespace dbfit {
	public class ColumnMissingException : ApplicationException {
		public ColumnMissingException(String name) : base("Cannot find column " + name) { }
	}
}
