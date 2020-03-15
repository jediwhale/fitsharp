// Copyright © 2019 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using System.Linq;

namespace fitSharp.Machine.Application {
    public static class ConfigurationNames {
        public static string TypeName(string facility, string action) {
            var match = names.FirstOrDefault(n => n.Matches(facility, action));
            return match != null ? match.TypeName : facility;
        }

        static readonly List<ConfigurationName> names = new List<ConfigurationName> {
            new ConfigurationName("fit.assemblies", "fitSharp.Machine.Engine.ApplicationUnderTest"),
            new ConfigurationName("settings", "elapsed", "fitSharp.Fit.Engine.FitEnvironment"),
            new ConfigurationName("settings", "appconfigfile", "System.AppDomainSetup"),
            new ConfigurationName("settings", "fitSharp.Machine.Application.Settings"),
            new ConfigurationName("fit.fileexclusions", "fitSharp.Fit.Application.FileExclusions"),
            new ConfigurationName("fit.namespaces", "fitSharp.Machine.Engine.ApplicationUnderTest"),
            new ConfigurationName("fit.settings", "appconfigfile", "System.AppDomainSetup"),
            new ConfigurationName("fit.settings", "fitSharp.Machine.Application.Settings"),
            new ConfigurationName("fileexclusions", "fitSharp.Fit.Application.FileExclusions"),
            new ConfigurationName("slim.service", "fitSharp.Slim.Service.SlimOperators"),
            new ConfigurationName("slim.operators", "fitSharp.Slim.Service.SlimOperators"),
            new ConfigurationName("fitsharp.slim.service.service", "fitSharp.Slim.Service.SlimOperators"),
            new ConfigurationName("fit.service", "fit.Service.Operators"),
            new ConfigurationName("fit.operators", "fit.Service.Operators"),
            new ConfigurationName("fit.cellhandlers", "fit.Service.Operators"),
            new ConfigurationName("fitlibrary.cellhandlers", "fit.Service.Operators")
        };

        class ConfigurationName {
            public ConfigurationName(string facility, string typeName) {
                this.facility = facility;
                TypeName = typeName;
            }

            public ConfigurationName(string facility, string action, string typeName) {
                this.facility = facility;
                TypeName = typeName;
                this.action = action;
            }

            public string TypeName { get; }

            public  bool Matches(string matchFacility, string matchAction) {
                return string.Equals(facility, matchFacility, StringComparison.OrdinalIgnoreCase)
                       && (action == null || string.Equals(action, matchAction, StringComparison.OrdinalIgnoreCase));
            }

            readonly string facility;
            readonly string action;
        }
    }
}