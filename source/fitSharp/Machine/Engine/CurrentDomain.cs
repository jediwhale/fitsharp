// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace fitSharp.Machine.Engine {
    public class CurrentDomain: ApplicationDomain {
        public Types LoadAssembly(string assemblyPath) {
            var assembly = Assembly.LoadFrom(assemblyPath);
            EnsureAllReferencedAssembliesAreLoaded(assembly);
            return new AssemblyTypes(assembly);
        }

        static void EnsureAllReferencedAssembliesAreLoaded(Assembly assembly) {
            assembly.GetExportedTypes();
        }

        public IEnumerable<Types> LoadedAssemblies {
            get {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                    if (IsDynamic(assembly)) continue;
                    yield return new AssemblyTypes(assembly);
                }
            }
        }

        static bool IsDynamic(Assembly assembly) {
            return assembly.ManifestModule.GetType().Namespace == "System.Reflection.Emit";
        }

        class AssemblyTypes: Types {
            readonly Assembly assembly;
            public AssemblyTypes(Assembly assembly) { this.assembly = assembly; }
            public string Name { get { return assembly.CodeBase; } }
            public IEnumerable<Type> Types { get { return assembly.GetExportedTypes(); } }
        }
    }
}
