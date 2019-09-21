using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DragForListary
{
    public partial class FormMain : Form
    {
        string arg = null;
        string[] DropFile = null;
        FileInfo info;
        bool Copy = true;
        bool Exec = false;
        bool ExecAble = false;

        public FormMain(string args)
        {
            InitializeComponent();
            arg = args;

            info = new FileInfo(args);
            string[] Ext = Environment.GetEnvironmentVariable("PATHEXT").ToUpper().Split(';');
            if (((System.Collections.IList)Ext).Contains(info.Extension.ToUpper()))
            {
                ExecAble = true;
            }
        }

        private void FormMain_DragOver(object sender, DragEventArgs e)
        {
            if (ExecAble && e.KeyState == 1)                //LeftButton
            {
                e.Effect = DragDropEffects.Link;
                this.Text = "Exec";
            }
            else if (e.KeyState == 5)                       //Shift With LeftButton
            {
                e.Effect = DragDropEffects.Move;
                this.Text = "Move";
            }
            else if (e.KeyState == 1 || e.KeyState == 9)    //(None or Ctrl) With LeftButton
            {
                e.Effect = DragDropEffects.Copy;
                this.Text = "Copy";
            }
            else
            {
                e.Effect = DragDropEffects.None;
                this.Text = "";
            }
        }

        private void FormMain_DragLeave(object sender, EventArgs e)
        {
            this.Text = "";
        }

        private void FormMain_DragDrop(object sender, DragEventArgs e)
        {
            DropFile = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (DropFile != null)
            {
                if (ExecAble && e.KeyState == 0)
                {
                    Exec = true;
                }
                else if (e.KeyState == 4)       //Shift
                {
                    Exec = false;
                    Copy = false;
                }
                timer.Enabled = true;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;
            if (Exec && info.Exists)
            {
                ProcessStartInfo ps = new ProcessStartInfo(arg);
                ps.UseShellExecute = true;
                ps.CreateNoWindow = false;
                ps.Arguments = " \"" + string.Join("\" \"", DropFile) + '"';
                Process.Start(ps);
            }
            else
            {
                SHFILEOPSTRUCT pm = new SHFILEOPSTRUCT();
                pm.fFlags = FILEOP_FLAGS.FOF_ALLOWUNDO;
                pm.pFrom = string.Join(FILE_SPLITER, DropFile);
                pm.pFrom += (FILE_SPLITER + FILE_SPLITER);
                if (Copy)
                {
                    pm.wFunc = wFunc.FO_COPY;
                }
                else
                {
                    pm.wFunc = wFunc.FO_MOVE;
                }
                if ((info.Attributes & System.IO.FileAttributes.Directory) != 0)
                {
                    pm.pTo = info.FullName;
                }
                else
                {
                    pm.pTo = info.DirectoryName;
                }
                SHFileOperation(pm);
            }
            Environment.Exit(0);
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]private static extern int SHFileOperation(SHFILEOPSTRUCT lpFileOp);
        private const string FILE_SPLITER = "\0";
        // Shell文件操作数据类型
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private class SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public wFunc wFunc;                 //操作方式
            public string pFrom;                //源
            public string pTo;                  //目标
            public FILEOP_FLAGS fFlags;
            public bool fAnyOperationsAborted;  //中止
            public IntPtr hNameMappings;
            public string lpszProgressTitle;    //标题
        }

        private enum wFunc
        {
            FO_MOVE = 0x1,
            FO_COPY = 0x2,
            FO_DELETE = 0x3,
            FO_RENAME = 0x4
        }

        private enum FILEOP_FLAGS
        {
            FOF_MULTIDESTFILES = 0x1,
                //The pTo member specifies multiple destination files (one for each source file) rather than one directory where all source files are to be deposited.
            FOF_CONFIRMMOUSE = 0x2,
                //Not currently used.
            FOF_SILENT = 0x4,
                //Do not display a progress dialog box.
            FOF_RENAMEONCOLLISION = 0x8,
                //Give the file being operated on a new name in a move, copy, or rename operation if a file with the target name already exists.
            FOF_NOCONFIRMATION = 0x10,
                //Respond with "Yes to All" for any dialog box that is displayed.
            FOF_WANTMAPPINGHANDLE = 0x20,
                //If FOF_RENAMEONCOLLISION is specified and any files were renamed, assign a name mapping object containing their old and new names to the hNameMappings member.
            FOF_ALLOWUNDO = 0x40,
                //Preserve Undo information, if possible. If pFrom does not contain fully qualified path and file names, this flag is ignored.
            FOF_FILESONLY = 0x80,
                //Perform the operation on files only if a wildcard file name (*.*) is specified.
            FOF_SIMPLEPROGRESS = 0x100,
                //Display a progress dialog box but do not show the file names.
            FOF_NOCONFIRMMKDIR = 0x200,
                //Do not confirm the creation of a new directory if the operation requires one to be created.
            FOF_NOERRORUI = 0x400,
                //Do not display a user interface if an error occurs.
            FOF_NOCOPYSECURITYATTRIBS = 0x800,
                //Do not copy the security attributes of the file.
            FOF_NORECURSION = 0x1000,
                //Only operate in the local directory. Don't operate recursively into subdirectories.
            FOF_NO_CONNECTED_ELEMENTS = 0x2000,
                //Do not move connected files as a group. Only move the specified files.
            FOF_WANTNUKEWARNING = 0x4000,
                //Send a warning if a file is being destroyed during a delete operation rather than recycled. This flag partially overrides FOF_NOCONFIRMATION.
            FOF_NORECURSEREPARSE = 0x8000,
                //Treat reparse points as objects, not containers.
        }
    }
}
