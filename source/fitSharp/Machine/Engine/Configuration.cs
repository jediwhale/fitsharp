// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class Configuration {

        readonly Dictionary<Type, Copyable> items = new Dictionary<Type, Copyable>();

        public Configuration() {}

        public Configuration(Configuration other) {
            foreach (Type key in other.items.Keys) {
                SetItem(key, other.items[key].Copy());
            }
        }

        public void LoadXml(string configXml) {
            if (string.IsNullOrEmpty(configXml)) return;
            var document = new XmlDocument();
            document.LoadXml(configXml);
            if (document.DocumentElement == null) return;
            foreach (XmlNode typeNode in document.DocumentElement.ChildNodes) {
                foreach (XmlNode methodNode in typeNode.ChildNodes) {
                    if (methodNode.NodeType == XmlNodeType.Element) {
                        LoadNode(typeNode.Name, methodNode);
                    }
                }
            }
        }

        void LoadNode(string typeName, XmlNode methodNode) {
            try {
                new BasicProcessor().Invoke(AliasType(typeName), AliasMethod(typeName, methodNode.Name), NodeParameters(methodNode));
            }
            catch (TargetInvocationException e) {
                throw e.InnerException;
            }
        }

        static readonly Dictionary<string, string> aliasTypes = new Dictionary<string, string> {
           {"fit.assemblies", "fitSharp.Machine.Engine.ApplicationUnderTest"},
           {"fit.fileexclusions", "fitSharp.Fit.Application.FileExclusions"},
           {"fit.namespaces", "fitSharp.Machine.Engine.ApplicationUnderTest"},
           {"fit.settings", "fitSharp.Machine.Application.Settings"},
           {"settings", "fitSharp.Machine.Application.Settings"},
           {"fileexclusions", "fitSharp.Fit.Application.FileExclusions"},
           {"slim.service", "fitSharp.Slim.Service.SlimOperators"},
           {"slim.operators", "fitSharp.Slim.Service.SlimOperators"},
           {"fitsharp.slim.service.service", "fitSharp.Slim.Service.SlimOperators"},
           {"fit.service", "fit.Service.Operators"},
           {"fit.operators", "fit.Service.Operators"},
           {"fit.cellhandlers", "fit.Service.Operators"},
           {"fitlibrary.cellhandlers", "fit.Service.Operators"}
       };

        TypedValue AliasType(string originalType) {
            string originalTypeLower = originalType.ToLowerInvariant();
            return new TypedValue(GetItem(aliasTypes.ContainsKey(originalTypeLower) ? aliasTypes[originalTypeLower] : originalType));
        }

        static string AliasMethod(string originalType, string originalMethod) {
            switch (originalType.ToLowerInvariant()) {
                case "fit.assemblies":
                    if (originalMethod == "add") return "addAssembly";
                    break;
                case "fit.namespaces":
                    if (originalMethod == "add") return "addNamespace";
                    if (originalMethod == "remove") return "removeNamespace";
                    break;
                case "fit.cellhandlers":
                case "fitlibrary.cellhandlers":
                    if (originalMethod == "add") return "addCellHandler";
                    if (originalMethod == "remove") return "removeCellHandler";
                    break;
            }
            switch (originalMethod) {
                case "addOperator":
                    return "add";
                case "removeOperator":
                    return "remove";
            }
            return originalMethod;
        }

        static Tree<string> NodeParameters(XmlNode node) {
            var result = new TreeList<string>()
                .AddBranchValue(node.InnerText);
            foreach (XmlAttribute attribute in node.Attributes) {
                result.AddBranchValue(attribute.Value);
            }
            return result;
        }

        public T GetItem<T>() where T: Copyable, new() {
            if (!items.ContainsKey(typeof(T))) {
                items[typeof(T)] = new T();
            }
            return (T)items[typeof(T)];
        }

        public Copyable GetItem(string typeName) {
            RuntimeType type = new ApplicationUnderTest().FindType(new IdentifierName(typeName));
            return GetItem(type.Type);
        }

        public Copyable GetItem(Type type) {
            if (!items.ContainsKey(type)) {
                items[type] = new BasicProcessor().Create(type.AssemblyQualifiedName).GetValue<Copyable>();
            }
            return items[type];
        }

        public void SetItem(Type type, Copyable value) { items[type] = value; }
    }
}