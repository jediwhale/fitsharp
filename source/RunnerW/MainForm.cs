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
        }

        private class TextReporter: ProgressReporter {
            private readonly TextBox text;

            public TextReporter(TextBox text) { this.text = text; }

            public void Write(string message) {
                text.Text += message;
                text.Refresh();
            }
        }

        private void MainForm_Activated(object sender, EventArgs e) {
            if (hasRun) return;
            textReporter = new TextReporter(progressText);
            int result = new Shell(textReporter).Run(arguments);
            textReporter.Write(string.Format("\r\nResult: {0}", result));
            hasRun = true;
        }
    }
}
