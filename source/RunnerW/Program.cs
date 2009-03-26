using System;
using System.Windows.Forms;

namespace fitSharp.RunnerW {
    static class Program {
        [STAThread]
        static void Main(string [] arguments) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(arguments));
        }
    }
}
