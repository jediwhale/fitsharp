// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Application {
    public class Settings: Copyable {

        public string ApartmentState { get; set; }
        private string appConfigFile;
        public string CodePage { get; set; }
        public string InputFolder { get; set; }
        public string OutputFolder { get; set; }
        public string Runner { get; set; }
        public string XmlOutput { get; set; }

        public string AppConfigFile {
            get { return appConfigFile; }
            set {
                appConfigFile = value;
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", value);
            }
        }

        public Copyable Copy() {
            return new Settings {
                ApartmentState = ApartmentState,
                appConfigFile = appConfigFile,
                CodePage = CodePage,
                InputFolder = InputFolder,
                OutputFolder = OutputFolder,
                Runner = Runner,
                XmlOutput = XmlOutput
            };
        }
    }
}