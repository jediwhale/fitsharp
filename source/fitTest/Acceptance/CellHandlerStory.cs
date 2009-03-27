// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;

namespace fit.Test.Acceptance {
    public class CellHandlerStory: ColumnFixture {
        public Guid Guid {
            get { return (Guid) myValue; }
            set { myValue = value; }
        }

        public string String {
            get { return (string) myValue; }
            set { myValue = value; }
        }

        public string Type {
            get { return myValue == null ? "<null>" : myValue.GetType().FullName; }
        }

        private object myValue;
    }
}