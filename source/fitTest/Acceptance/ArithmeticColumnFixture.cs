// Copyright © 2009 Syterra Software Inc. Includes work by Object Mentor, Inc., © 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using fit;

namespace eg
{
	public class ArithmeticColumnFixture : ColumnFixture 
	{

		public int x;
		public int y;

		public int plus() 
		{
			return x + y;
		}

		public int minus() 
		{
			return x - y;
		}

		public int times () 
		{
			return x * y;
		}

		public int divide () 
		{
			return x / y;
		}

		public float floating () 
		{
			return (float)x / (float)y;
		}
        
        public ScientificDouble  sin () {
            return new ScientificDouble(Math.Sin(toRadians(x)));
        }

        public ScientificDouble  cos () {
            return new ScientificDouble(Math.Cos(toRadians(x)));
        }

        private double toRadians(double degrees) {
            return (degrees * Math.PI) / 180d;
        }
	}
}