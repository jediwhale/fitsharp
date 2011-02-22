// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class ApplicationUnderTest: Copyable {
        const int cacheSize = 50;
        readonly Assemblies assemblies;
        readonly Namespaces namespaces;
        readonly List<Type> cache = new List<Type>();

        public ApplicationUnderTest(): this(new CurrentDomain()) {}

        public ApplicationUnderTest(ApplicationDomain appDomain) {
            assemblies = new Assemblies(appDomain);
            namespaces = new Namespaces();
            AddNamespace(GetType().Namespace);
        }

        public ApplicationUnderTest(ApplicationUnderTest other) {
            assemblies = new Assemblies(other.assemblies);
            namespaces = new Namespaces(other.namespaces);
        }

        public void AddAssembly(string assemblyName){
          assemblies.AddAssembly(assemblyName); 
        }

        public void AddNamespace(string namespaceName) {
            namespaces.Add(namespaceName.Trim());
            assemblies.LoadWellKnownAssemblies(namespaceName.Trim() + ".");
        }

        public void RemoveNamespace(string namespaceName) {
            namespaces.Remove(namespaceName.Trim());
        }

        public RuntimeType FindType(NameMatcher typeName) {
            Type type = Type.GetType(typeName.MatchName);
            if (type == null) {
                type = SearchForType(typeName, cache);
                if (type == null) {
                    assemblies.LoadWellKnownAssemblies(typeName.MatchName);
                    type = SearchForType(typeName, assemblies.Types);
                }
                if (type == null) throw new TypeMissingException(typeName.MatchName, assemblies.Report);
                UpdateCache(type);
            }
            return new RuntimeType(type);
        }


        Type SearchForType(NameMatcher typeName, IEnumerable<Type> types) {
            foreach (Type type in types) {
                if (typeName.Matches(type.FullName)) return type;
                if (type.Namespace == null || !namespaces.IsRegistered(type.Namespace)) continue;
                if (typeName.Matches(type.Name)) return type;
            }
            return null;
        }

        void UpdateCache(Type type) {
            if (cache.Contains(type)) cache.Remove(type);
            cache.Add(type);
            if (cache.Count > cacheSize) cache.RemoveAt(0);
        }

        public Copyable Copy() {
            return new ApplicationUnderTest(this);
        }

        class Assemblies {
            readonly ApplicationDomain appDomain;
            readonly List<Types> assemblies;

            public Assemblies(ApplicationDomain appDomain) {
                this.appDomain = appDomain;
                assemblies = new List<Types>();
            }
            public Assemblies(Assemblies other) {
                appDomain = other.appDomain;
                assemblies = new List<Types>(other.assemblies);
            }

            public void LoadWellKnownAssemblies(string typeName) {
                if (!typeName.StartsWith("fit.") && !typeName.StartsWith("fitnesse.")) return;
                if (!assemblies.Exists(a => a.Name.EndsWith("/fit.dll", StringComparison.OrdinalIgnoreCase))) {
                    AddAssembly(Assembly.GetExecutingAssembly().CodeBase.Replace("/fitSharp.", "/fit."));
                }
            }

            public void AddAssembly(string assemblyName) {
                if (IsIgnored(assemblyName)) return;
                if (assemblies.Exists(a => a.Name == assemblyName)) return;

                if (!AssemblyFileExists(assemblyName)) return;
                var assembly = appDomain.LoadAssembly(assemblyName);
                
                if (assemblies.Exists(a => a.Name == assembly.Name)) return;
                assemblies.Add(assembly);
            }

            private bool AssemblyFileExists(string assemblyName) {
                // file:///C:\dev\DEV-Fitsharp\packages\windows\components\ClientAcceptanceTests\bin\SE.WorkStation.Script.AcceptanceTest.dll
                // fit.dll

                string assemblyFileName = NormalizeAssemblyFileName(assemblyName);
                return File.Exists(assemblyFileName);
            }

            private string NormalizeAssemblyFileName(string assemblyName) {
                string assemblyFileName = assemblyName;
                if (assemblyName.StartsWith("file:"))
                  assemblyFileName = new Uri(assemblyName).AbsolutePath;

                return Path.GetFullPath(assemblyFileName);
            }

            static bool IsIgnored(string assemblyName) {
                return string.Equals(".jar", Path.GetExtension(assemblyName), StringComparison.OrdinalIgnoreCase);
            }

            public IEnumerable<Type> Types {
                get {
                    foreach (var assembly in assemblies) {
                        foreach (var type in assembly.Types) yield return type;
                    }
                    foreach (var assembly in appDomain.LoadedAssemblies) {
                        if (assemblies.Exists(a => a.Name == assembly.Name)) continue;
                        foreach (var type in assembly.Types) yield return type;
                    }
                }
            }

            public string Report {
                get {
                    var result = new StringBuilder();
                    foreach (var assembly in appDomain.LoadedAssemblies) {
                        result.AppendFormat("    {0}{1}", assembly.Name, Environment.NewLine);
                    }
                    return result.ToString();
                }
            }
        }

        class Namespaces {
            readonly List<string> namespaces;

            public Namespaces() {
                namespaces = new List<string>();
            }

            public Namespaces(Namespaces other) {
                namespaces = new List<string>(other.namespaces);
            }

            public void Add(string namespaceName) {
                if (!namespaces.Contains(namespaceName)) {
                    namespaces.Add(namespaceName);
                }
            }

            public void Remove(string namespaceName) {
                if (namespaces.Contains(namespaceName)) {
                    namespaces.Remove(namespaceName);
                }
            }

            public bool IsRegistered(string namespaceName) {
                foreach (string name in namespaces) {
                    if (string.Compare(name, namespaceName, StringComparison.OrdinalIgnoreCase) == 0) return true;
                }
                return false;
            }
        }
    }
}
