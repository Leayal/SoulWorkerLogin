using System;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SoulWorkerLoginUpdater
{
    public partial class Form1 : Form
    {

        private BackgroundWorker myBWorker;
        private WorkerMeta myMeta;
        public Form1(WorkerMeta meta)
        {
            InitializeComponent();
            this.myMeta = meta;
            this.Icon = Properties.Resources.haru_sd_WM1UAm_256px;
            this.myBWorker = new BackgroundWorker();
            this.myBWorker.WorkerSupportsCancellation = true;
            this.myBWorker.WorkerReportsProgress = true;
            this.myBWorker.DoWork += MyBWorker_DoWork;
            this.myBWorker.RunWorkerCompleted += MyBWorker_RunWorkerCompleted;
            this.myBWorker.ProgressChanged += MyBWorker_ProgressChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        { }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.myBWorker.RunWorkerAsync(this.myMeta);
        }

        private void MyBWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (e.Error is IOException)
                { MessageBox.Show(this, e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                else if (e.Error is Exception)
                { MessageBox.Show(this, e.Error.Message + "\n" + e.Error.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                Environment.Exit(1);
            }
            else
            {
                if (e.Result != null)
                {
                    WorkerMeta myMeta = e.Result as WorkerMeta;
                    try { File.Delete(myMeta.Patch); }
                    catch { }
                    using (Process myProcess = new Process())
                    {
                        myProcess.StartInfo.FileName = myMeta.Destination;
                        if (My.Computer.OSFullName.IndexOf("Windows XP") == -1)
                            myProcess.StartInfo.Verb = "runas";
                        myProcess.Start();
                    }
                    Environment.Exit(0);
                }
                else
                { MessageBox.Show(this, "The updates somehow failed. Unknown Error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); Environment.Exit(1); }
            }            
        }

        private void MyBWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 1:
                    this.progressBar1.Value = Convert.ToInt32(e.UserState);
                    break;
                case 2:
                    this.label1.Text = "Current: Extracting '" + e.UserState as string + "'";
                    break;
                case 3:
                    this.label1.Text = "Current: Closing 'WebClient Lite'";
                    break;
            }
        }

        private void MyBWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            WorkerMeta myMeta = e.Argument as WorkerMeta;
            if (File.Exists(myMeta.Patch))
            {
                this.myBWorker.ReportProgress(3);
                this.CloseProcess(myMeta.Destination);
                File.Delete(myMeta.Destination);
                File.Move(myMeta.Patch, myMeta.Destination);
                e.Result = myMeta;
            }
        }

        private void CloseProcess(string fullpath)
        {
            foreach (Process derp in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(fullpath)))
            {
                if (GetExecutablePath(derp).ToLower() == fullpath.ToLower())
                {
                    derp.WaitForExit(500);
                    if (!derp.HasExited)
                        derp.CloseMainWindow();
                    derp.WaitForExit(5000);
                    if (!derp.HasExited)
                        derp.Kill();
                    derp.WaitForExit(3000);
                }
            }
        }

        private static string GetExecutablePath(Process Process)
        {
            //If running on Vista or later use the new function
            if (Environment.OSVersion.Version.Major >= 6)
            {
                return GetExecutablePathAboveVista(Process.Id);
            }
            else
            {
                return Process.MainModule.FileName;
            }
        }

        private static string GetExecutablePathAboveVista(int ProcessId)
        {
            var buffer = new StringBuilder(1024);
            IntPtr hprocess = OpenProcess(ProcessAccessFlags.PROCESS_QUERY_LIMITED_INFORMATION,
                                          false, ProcessId);
            if (hprocess != IntPtr.Zero)
            {
                try
                {
                    int size = buffer.Capacity;
                    if (QueryFullProcessImageName(hprocess, 0, buffer, out size))
                    {
                        return buffer.ToString();
                    }
                }
                finally
                {
                    CloseHandle(hprocess);
                }
            }
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        [DllImport("kernel32.dll")]
        private static extern bool QueryFullProcessImageName(IntPtr hprocess, int dwFlags,
                       StringBuilder lpExeName, out int size);
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
                       bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        private enum ProcessAccessFlags
        {
            PROCESS_QUERY_LIMITED_INFORMATION = 0x1000
        }
    }
}
