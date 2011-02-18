// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Globalization;
using System.Threading;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class Culture: Copyable, SetUpTearDown {
        CultureInfo saved;
        public string Name { private get; set; }

        public void SetUp() {
            saved = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = MakeCultureInfo();
        }

        CultureInfo MakeCultureInfo() {
            return string.Equals(Name, "invariant", StringComparison.InvariantCultureIgnoreCase)
                ? CultureInfo.InvariantCulture
                : CultureInfo.GetCultureInfo(Name);
        }

        public void TearDown() {
            Thread.CurrentThread.CurrentCulture = saved;
        }

        public Copyable Copy() {
            return new Culture {Name = Name};
        }
    }
}
