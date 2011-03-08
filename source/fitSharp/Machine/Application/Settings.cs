// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Configuration;
using System.IO;
using fitSharp.Machine.Model;
using System.Threading;
using System.Globalization;
using System.Linq;

namespace fitSharp.Machine.Application {
    public class Settings: Copyable {
        private static readonly string appSettingsBehavior;
        static Settings() {
            appSettingsBehavior = ConfigurationManager.AppSettings["fitVersion"];
        }

        private const int DefaultCodePage = 1252;

        public string ApartmentState { get; set; }
        public string CodePage { get; set; }
        public string InputFolder { get; set; }
        public string OutputFolder { get; set; }
        public string Runner { get; set; }
        public string XmlOutput { get; set; }
        public string Behavior { get; set; }
        public string Culture { get; set; }
        public string AppConfigFile { get; set; }
        public bool DryRun { get; set; }

        public CultureInfo CultureInfo
        {
          get
          {
            if (IsInvariantCultureTag(Culture))
              return CultureInfo.InvariantCulture;

            if (!IsValidCultureName(Culture))
              return DefaultCulture;

            return CultureInfo.GetCultureInfo(Culture);
          }
        }

        public static CultureInfo DefaultCulture
        {
          get
          {
            return Thread.CurrentThread.CurrentCulture;
          }
        }

        public int CodePageNumber {
            get {
                int result = DefaultCodePage;
                if (CodePage != null) int.TryParse(CodePage, out result);
                return result;
            }
        }

        public bool IsStandard { get { return BehaviorHas("std"); } }

        public bool BehaviorHas(string keyword) {
            return Behavior != null && Behavior.ToLower().IndexOf(keyword) >= 0;
        }

        public Settings() {
            Behavior = appSettingsBehavior;
        }

        public Settings(Settings other) {
            ApartmentState = other.ApartmentState;
            AppConfigFile = other.AppConfigFile;
            Behavior = other.Behavior;
            CodePage = other.CodePage;
            InputFolder = other.InputFolder;
            OutputFolder = other.OutputFolder;
            Runner = other.Runner;
            XmlOutput = other.XmlOutput;
            Culture = other.Culture;
            DryRun = other.DryRun;
        }

        public Copyable Copy() {
            return new Settings(this);
        }

        private bool IsInvariantCultureTag(string name)
        {
          return (string.Compare(name, "invariant", true) == 0);
        }

        private bool IsValidCultureName(string name)
        {
          if (string.IsNullOrEmpty(name))
            return false;

          return CultureInfo.GetCultures(CultureTypes.AllCultures).Any(ci => ci.Name == name);
        }
    }
}
