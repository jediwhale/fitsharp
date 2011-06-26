// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Xml;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Engine {
    public class Configuration {

        readonly Dictionary<Type, object> items = new Dictionary<Type, object>();

        public Configuration() {}

        public Configuration(Configuration other) {
            foreach (Type key in other.items.Keys) {
                var item = other.items[key];
                var copyableItem = item as Copyable;
                SetItem(key, copyableItem != null ? copyableItem.Copy() : item);
            }
        }

        public void SetUp() {
            foreach (var item in items.Values) {
                var setUpTearDown = item as SetUpTearDown;
                if (setUpTearDown != null) setUpTearDown.SetUp();
            }
        }

        public void TearDown() {
            foreach (var item in items.Values) {
                var setUpTearDown = item as SetUpTearDown;
                if (setUpTearDown != null) setUpTearDown.TearDown();
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
            new BasicProcessor().InvokeWithThrow(AliasType(typeName), AliasMethod(typeName, methodNode.Name), NodeParameters(methodNode));
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

        public T GetItem<T>() where T: new() {
            if (!items.ContainsKey(typeof(T))) {
                items[typeof(T)] = new T();
            }
            return (T)items[typeof(T)];
        }

        public object GetItem(string typeName) {
            RuntimeType type = new ApplicationUnderTest().FindType(new IdentifierName(typeName));
            return GetItem(type.Type);
        }

        public object GetItem(Type type) {
            if (!items.ContainsKey(type)) {
                items[type] = new BasicProcessor().Create(type.AssemblyQualifiedName).GetValue<Copyable>();
            }
            return items[type];
        }

        public void SetItem(Type type, object value) { items[type] = value; }
    }
}