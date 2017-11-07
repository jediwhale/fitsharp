// Copyright © 2017 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Windows.Forms;
using fitSharp.IO;
using fitSharp.Machine.Application;

namespace fitSharp.RunnerW {
    public partial class MainForm : Form {

        public MainForm() {
            InitializeComponent();
        }

        public MainForm(string[] arguments): this() {
            this.arguments = arguments;
        }

        void MainForm_Load(object sender, EventArgs e) {
            hasRun = false;
            textReporter = new TextReporter(progressText);
            foreach (var argument in arguments) {
                textReporter.Write(string.Format("{0} ", argument));
            }
            textReporter.Write(Environment.NewLine);
        }

        class TextReporter: ProgressReporter {

            public TextReporter(TextBox text) { this.text = text; }

            public void Write(string message) {
                text.Text += message;
                text.Refresh();
            }

            readonly TextBox text;
        }

        void goButton_Click(object sender, EventArgs e) {
            if (hasRun) {
                Close();
            }
            else {
                var result = new Shell(textReporter, new FileSystemModel(), arguments).Run();
                textReporter.Write(string.Format("{0}Result: {1}", Environment.NewLine, result));
                hasRun = true;
            }
        }

        readonly string[] arguments;

        TextReporter textReporter;
        bool hasRun;
    }
}
