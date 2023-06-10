using System;
using System.Windows.Forms;

namespace DragForListary {
    static class Program {
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 0) {
                Application.Run(new FormMain(Environment.CurrentDirectory));
            } else {
                Application.Run(new FormMain(String.Join(" ", args)));
            }
        }
    }
}
