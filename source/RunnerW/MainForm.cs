// Copyright © 2009 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Windows.Forms;
using fitSharp.Machine.Application;

namespace fitSharp.RunnerW {
    public partial class MainForm : Form {
        private readonly string[] arguments;
        private TextReporter textReporter;
        private bool hasRun;

        public MainForm() {
            InitializeComponent();
        }

        public MainForm(string[] arguments): this() {
            this.arguments = arguments;
        }

        private void MainForm_Load(object sender, EventArgs e) {
            hasRun = false;
            textReporter = new TextReporter(progressText);
            foreach (string argument in arguments) {
                textReporter.Write(string.Format("{0} ", argument));
            }
            textReporter.Write(Environment.NewLine);

        }

        private class TextReporter: ProgressReporter {
            private readonly TextBox text;

            public TextReporter(TextBox text) { this.text = text; }

            public void Write(string message) {
                text.Text += message;
                text.Refresh();
            }
        }

        private void goButton_Click(object sender, EventArgs e) {
            if (hasRun) {
                Close();
            }
            else {
                int result = new Shell(textReporter).Run(arguments);
                textReporter.Write(string.Format("{0}Result: {1}", Environment.NewLine, result));
                hasRun = true;
            }
        }

    }
}
