// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using fitSharp.Machine.Exception;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class ApplicationUnderTest: Copyable {
        private readonly List<Assembly> assemblies;
        private readonly List<LanguageName> namespaces;
        private const int cacheSize = 50;
        private readonly List<Type> cache = new List<Type>(cacheSize);

        public ApplicationUnderTest() {
            assemblies = new List<Assembly>();
            namespaces = new List<LanguageName>();
            AddNamespace(GetType().Namespace);
        }

        public ApplicationUnderTest(ApplicationUnderTest other) {
            assemblies = new List<Assembly>(other.assemblies);
            namespaces = new List<LanguageName>(other.namespaces);
        }

        public void AddAssembly(string assemblyName) {
            Assembly assembly = Assembly.LoadFrom(assemblyName);
            if (assemblies.Contains(assembly)) return;
            assemblies.Add(assembly);
        }

        public void AddNamespace(string namespaceName) {
            var newNamespace = new LanguageName(namespaceName);
            if (!namespaces.Contains(newNamespace)) namespaces.Add(newNamespace);
        }

        public void RemoveNamespace(string namespaceName) {
            var existingNamespace = new LanguageName(namespaceName);
            if (namespaces.Contains(existingNamespace)) namespaces.Remove(existingNamespace);
        }

        public RuntimeType FindType(NameMatcher typeName) {
            Type type = Type.GetType(typeName.MatchName);
            if (type == null) {
                type = SearchForType(typeName, cache)
                       ?? SearchForType(typeName, AssemblyTypes(assemblies))
                          ?? SearchForType(typeName, AssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()));
                if (type == null) throw new TypeMissingException(typeName.MatchName, TypeNotFoundMessage());
                UpdateCache(type);
            }
            return new RuntimeType(type);
        }

        private static string TypeNotFoundMessage() {
            var result = new StringBuilder();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                result.AppendFormat("    {0}{1}", assembly.CodeBase, Environment.NewLine);
            return result.ToString();
        }

        private void UpdateCache(Type type) {
            if (cache.Contains(type)) cache.Remove(type);
            cache.Add(type);
            if (cache.Count > cacheSize) cache.RemoveAt(0);
        }

        private Type SearchForType(NameMatcher typeName, IEnumerable<Type> types) {
            foreach (Type type in types) {
                if (typeName.Matches(type.FullName)) return type;
                if (type.Namespace == null || !IsRegistered(type.Namespace)) continue;
                if (typeName.Matches(type.Name)) return type;
            }
            return null;
        }

        private static IEnumerable<Type> AssemblyTypes(IEnumerable<Assembly> assemblies) {
            foreach (Assembly assembly in assemblies) {
                foreach (Type type in assembly.GetExportedTypes()) yield return type;
            }
        }

        private bool IsRegistered(string namespaceString) {
            var existingNamespace = new LanguageName(namespaceString);
            return namespaces.Contains(existingNamespace);
        }

        public Copyable Copy() {
            return new ApplicationUnderTest(this);
        }
    }
}