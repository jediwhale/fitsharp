// Copyright © 2009 Syterra Software Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.IO;
using fitlibrary;
using fitSharp.Fit.Model;
using fitSharp.Machine.Application;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fit.Test.FitUnit {
    public class ObjectFactoryTest: DoFixture {

        public void AddCopyOfAssembly(string thePath) {
            string copyPath = Path.Combine(Path.GetDirectoryName(thePath), "copy of " + Path.GetFileName(thePath));
            if (File.Exists(copyPath)) File.Delete(copyPath);
            File.Copy(thePath, copyPath);
            Context.Configuration.GetItem<ApplicationUnderTest>().AddAssembly(copyPath);
        }

        public string CreateInstance(string theFixture) {
            return Context.Configuration.GetItem<Service.Service>().Create(theFixture, new TreeList<Cell>()).Type.FullName;
        }
    }
}