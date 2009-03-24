// FitNesse.NET
// Copyright (c) 2006 Syterra Software Inc. This program is free software;
// you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;

namespace fit {
    
    public interface TimeKeeper {
        DateTime Now {get;}
        DateTime UtcNow {get;}
    }

	public class Clock: TimeKeeper {
	    public static TimeKeeper Instance = new Clock();
	    public DateTime Now {get { return DateTime.Now; }}
	    public DateTime UtcNow {get { return DateTime.UtcNow; }}
	}
}
