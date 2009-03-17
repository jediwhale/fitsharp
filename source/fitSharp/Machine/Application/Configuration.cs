// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Application {
    public class Configuration {

        private readonly Dictionary<Type, Copyable> items = new Dictionary<Type, Copyable>();

        public Configuration() {}

        public Configuration(Configuration other) {
            foreach (Type key in other.items.Keys) {
                SetItem(key, other.items[key].Copy());
            }
        }

        public void LoadXml(string configXml) {
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

        public void LoadFile(string filePath) {
            var reader = new StreamReader(filePath);
            LoadXml(reader.ReadToEnd());
            reader.Close();
        }

        private void LoadNode(string typeName, XmlNode methodNode) {
            try {
                new Processor<string>().TryInvoke(new TypedValue(GetItem(typeName)), methodNode.Name, NodeParameters(methodNode));
            }
            catch (TargetInvocationException e) {
                throw e.InnerException;
            }
        }

        private static Tree<string> NodeParameters(XmlNode node) {
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
            Type type = new BasicProcessor().Parse<RuntimeType>(typeName).Type;
            return GetItem(type);
        }

        public Copyable GetItem(Type type) {
            if (!items.ContainsKey(type)) {
                items[type] = (Copyable)new BasicProcessor().Create(type.AssemblyQualifiedName).Value;
            }
            return items[type];
        }

        public void SetItem(Type type, Copyable value) { items[type] = value; }
    }
}