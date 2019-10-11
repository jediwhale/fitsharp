// Copyright © 2019 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace fitSharp.IO {
    
    public interface TimeKeeper {
        DateTime Now {get;}
        int Start();
        TimeSpan Stop(int index);
    }

	public class Clock: TimeKeeper {
	    public static TimeKeeper Instance = new Clock();

	    public DateTime Now => DateTime.Now;

	    public int Start() {
	        var index = watches.FindIndex(w => !w.IsRunning);
	        if (index < 0) {
	            watches.Add(new Stopwatch());
	            index = watches.Count - 1;
	        }
	        watches[index].Restart();
	        return index;
	    }

	    public TimeSpan Stop(int index) {
	        watches[index].Stop();
	        return watches[index].Elapsed;
	    }

        static readonly List<Stopwatch> watches = new List<Stopwatch>();
	}
}
