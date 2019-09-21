using System;
using System.Windows.Forms;

namespace DragForListary
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 0)
            {
#if DEBUG
                Application.Run(new FormMain("c:\\program files\\bandizip\\bandizip.exe"));
#else
                Environment.Exit(0);
#endif
            }
            else
            {
                Application.Run(new FormMain(args[0]));
            }
        }
    }
}
