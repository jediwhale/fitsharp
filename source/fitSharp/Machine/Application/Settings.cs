// Copyright © 2018 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Text;
using fitSharp.Machine.Model;

namespace fitSharp.Machine.Application {
    public class Settings: Copyable {

        public string ApartmentState { get; set; }
        public string CodePage { get; set; }
        public string InputFolder { get; set; }
        public string OutputFolder { get; set; }
        public string Runner { get; set; }
        public string XmlOutput { get; set; }
        public string Behavior { get; set; }
        public bool DryRun { get; set; }
        public string TagList { get; set; }

        public int CodePageNumber {
            get {
                var result = Encoding.Default.CodePage;
                if (CodePage != null) int.TryParse(CodePage, out result);
                return result;
            }
        }

        public bool IsStandard => BehaviorHas("std");

        public bool BehaviorHas(string keyword) {
            return Behavior != null && Behavior.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public Settings() {}

        public Settings(Settings other) {
            ApartmentState = other.ApartmentState;
            Behavior = other.Behavior;
            CodePage = other.CodePage;
            InputFolder = other.InputFolder;
            OutputFolder = other.OutputFolder;
            Runner = other.Runner;
            XmlOutput = other.XmlOutput;
            DryRun = other.DryRun;
            TagList = other.TagList;
        }

        public Copyable Copy() {
            return new Settings(this);
        }
    }
}
