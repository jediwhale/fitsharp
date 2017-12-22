// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.IO;
using fitSharp.Fit.Model;
using fitSharp.IO;

namespace fitSharp.Fit.Runner {
    public class Report {

        const string ourReportName = "reportIndex.html";
	        
        public Report(string reportPath) {
            this.reportPath = reportPath;
            content = new StringWriter();
            content.WriteLine("<html><head><link href=\"fit.css\" type=\"text/css\" rel=\"stylesheet\">");
            content.WriteLine("<title>Folder Runner Report</title></head>");
            content.WriteLine("<body><h1>Folder Runner Report {0:yyyy-MM-dd HH:mm:ss}</h1>", Clock.Instance.Now);
        }

        public void ListFile(string thePath, TestCounts counts, ElapsedTime elapsedTime) {
            content.WriteLine("<br /><a href=\"{0}\">{0}</a> <span class=\"{2}\">{1}</span> in {3}",
                thePath.Substring(reportPath.Length + 1).Replace('\\', '/'),
                counts.Description,
                counts.Style,
                elapsedTime);
        }
	        
        public void Finish(FolderModel folderModel) {
            content.WriteLine("</body></html>");
            folderModel.MakeFile(System.IO.Path.Combine(reportPath, ourReportName), content.ToString());
        }

        readonly StringWriter content;
        readonly string reportPath;
    }
}
