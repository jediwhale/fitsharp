using System.Collections.Generic;
using System.Xml;
using fitSharp.Machine.Engine;
// Copyright © 2011 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitSharp.Machine.Model;

namespace fitSharp.Machine.Application {
    public class SuiteConfiguration {
        public SuiteConfiguration(Configuration configuration) {
            this.configuration = configuration;
        }

        public void LoadXml(string  configurationXml) {
            if (string.IsNullOrEmpty(configurationXml)) return;
            var document = new XmlDocument();
            document.LoadXml(configurationXml);
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
            new BasicProcessor().InvokeWithThrow(AliasType(typeName, methodNode.Name), AliasMethod(typeName, methodNode.Name), NodeParameters(methodNode));
        }

        TypedValue AliasType(string originalType, string originalMethod) {
            var originalTypeLower = originalType.ToLowerInvariant();
            var newType = aliasTypes.ContainsKey(originalTypeLower) ? aliasTypes[originalTypeLower] : originalType;
            if (newType == "fitSharp.Machine.Application.Settings" && originalMethod.ToLowerInvariant() == "appconfigfile") {
                newType = "System.AppDomainSetup";
            }
            return new TypedValue(configuration.GetItem(newType));
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
                case "AppConfigFile":
                    return "ConfigurationFile";
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

        readonly Configuration configuration;
    }
}
