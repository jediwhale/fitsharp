using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using fitSharp.Machine.Application;

namespace fitSharp.Test.NUnit.Application
{
  [TestFixture]
  public class SettingsTest
  {
    [Test]
    public void CopyConstructor()
    {
      Settings s1 = new Settings();
      s1.ApartmentState = "apartmentstate";
      s1.AppConfigFile = "appconfigfile";
      s1.Behavior = "behavior";
      s1.CodePage = "codepage";
      s1.Culture = "culture";
      s1.InputFolder = "inputfolder";
      s1.OutputFolder = "outputfolder";
      s1.Runner = "runner";
      s1.XmlOutput = "xmloutput";

      Settings s2 = new Settings(s1);
      Assert.AreEqual(s1.ApartmentState, s2.ApartmentState);
      Assert.AreEqual(s1.AppConfigFile, s2.AppConfigFile);
      Assert.AreEqual(s1.Behavior, s2.Behavior);
      Assert.AreEqual(s1.CodePage, s2.CodePage);
      Assert.AreEqual(s1.Culture, s2.Culture);
      Assert.AreEqual(s1.InputFolder, s2.InputFolder);
      Assert.AreEqual(s1.OutputFolder, s2.OutputFolder);
      Assert.AreEqual(s1.Runner, s2.Runner);
      Assert.AreEqual(s1.XmlOutput, s2.XmlOutput);
    }

    [Test]
    public void AppConfigFileIsMaintainedAsSpecified()
    {
      var s = new Settings();

      s.AppConfigFile = "bob.config";
      Assert.AreEqual("bob.config", s.AppConfigFile);

      s.AppConfigFile = @"..\..\bob.config";
      Assert.AreEqual(@"..\..\bob.config", s.AppConfigFile);

      s.AppConfigFile = @"C:\bob.config";
      Assert.AreEqual(@"C:\bob.config", s.AppConfigFile);
    }
  }
}
