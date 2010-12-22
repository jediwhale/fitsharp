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
        readonly Assemblies assemblies;
        readonly Namespaces namespaces;
        const int cacheSize = 50;
        readonly List<Type> cache = new List<Type>(cacheSize);

        public ApplicationUnderTest() {
            assemblies = new Assemblies();
            namespaces = new Namespaces();
            AddNamespace(GetType().Namespace);
        }

        public ApplicationUnderTest(ApplicationUnderTest other) {
            assemblies = new Assemblies(other.assemblies);
            namespaces = new Namespaces(other.namespaces);
        }

        public void AddAssembly(string assemblyName) { assemblies.AddAssembly(assemblyName); }

        public void AddNamespace(string namespaceName) {
            namespaces.Add(namespaceName.Trim());
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
                if (type == null) throw new TypeMissingException(typeName.MatchName, Assemblies.Report);
                UpdateCache(type);
            }
            return new RuntimeType(type);
        }

        void UpdateCache(Type type) {
            if (cache.Contains(type)) cache.Remove(type);
            cache.Add(type);
            if (cache.Count > cacheSize) cache.RemoveAt(0);
        }

        Type SearchForType(NameMatcher typeName, IEnumerable<Type> types) {
            foreach (Type type in types) {
                if (typeName.Matches(type.FullName)) return type;
                if (type.Namespace == null || !namespaces.IsRegistered(type.Namespace)) continue;
                if (typeName.Matches(type.Name)) return type;
            }
            return null;
        }

        public Copyable Copy() {
            return new ApplicationUnderTest(this);
        }

        class Assemblies {
            public readonly List<Assembly> assemblies;
            public Assemblies() { assemblies = new List<Assembly>(); }
            public Assemblies(Assemblies other) { assemblies = new List<Assembly>(other.assemblies); }

            public void LoadWellKnownAssemblies(string typeName) {
                if (!typeName.StartsWith("fit.")) return;
                if (!assemblies.Exists(a => a.CodeBase.EndsWith("/fit.dll", StringComparison.OrdinalIgnoreCase))) {
                    AddAssembly(Assembly.GetExecutingAssembly().CodeBase.Replace("/fitSharp.", "/fit."));
                }
            }

            public void AddAssembly(string assemblyName) {
                if (IsIgnored(assemblyName)) return;
                if (assemblies.Exists(a => a.CodeBase == assemblyName)) return;
                var assembly = Assembly.LoadFrom(assemblyName);
                if (assemblies.Contains(assembly)) return;
                assemblies.Add(assembly);
                EnsureAllReferencedAssembliesAreLoaded(assembly);
            }

            static bool IsIgnored(string assemblyName) {
                return string.Equals(".jar", Path.GetExtension(assemblyName), StringComparison.OrdinalIgnoreCase);
            }

            static void EnsureAllReferencedAssembliesAreLoaded(Assembly assembly) {
                assembly.GetExportedTypes();
            }

            public IEnumerable<Type> Types {
                get {
                    foreach (Assembly assembly in assemblies) {
                        if (IsDynamic(assembly)) continue;
                        foreach (Type type in assembly.GetExportedTypes()) yield return type;
                    }
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                        if (IsDynamic(assembly)) continue;
                        foreach (Type type in assembly.GetExportedTypes()) yield return type;
                    }
                }
            }

            public static string Report {
                get {
                    var result = new StringBuilder();
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                        if (IsDynamic(assembly)) continue;
                        result.AppendFormat("    {0}{1}", assembly.CodeBase, Environment.NewLine);
                    }
                    return result.ToString();
                }
            }

            static bool IsDynamic(Assembly assembly) {
                return assembly.ManifestModule.GetType().Namespace == "System.Reflection.Emit";
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
