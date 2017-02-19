using Microsoft.VisualBasic.ApplicationServices;
using System;

namespace SoulWorkerLoginUpdater
{
    public static class My
    {
        private static CustomWindowsFormsApplicationBase _instance;
        public static CustomWindowsFormsApplicationBase Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CustomWindowsFormsApplicationBase();
                return _instance;
            }
        }

        public static AssemblyInfo Info { get { return Instance.Info; } }   
        public static Microsoft.VisualBasic.Devices.ComputerInfo Computer { get { return Instance.Computer; } }
    }

    public class CustomWindowsFormsApplicationBase : Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
    {
        public CustomWindowsFormsApplicationBase() : base()
        {
            this.ShutdownStyle = Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterAllFormsClose;
            this.SaveMySettingsOnExit = false;
            this.EnableVisualStyles = true;
            this.Computer = new Microsoft.VisualBasic.Devices.ComputerInfo();

        }

        public Microsoft.VisualBasic.Devices.ComputerInfo Computer { get; }

        protected override void OnCreateMainForm()
        {
            bool myCall = false;
            string myPatch = null, myDestination = null, buffer = null;
            string[] arg = null;
            char[] asdasd = { ':' };
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 2)
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].IndexOf(":") > -1)
                    {
                        arg = args[i].Split(asdasd, 2);
                        buffer = arg[0];
                    }
                    else
                        buffer = args[i];
                    switch (buffer)
                    {
                        case "-leayal":
                            myCall = true;
                            break;
                        case "-destination":
                            myDestination = arg[1];
                            break;
                        case "-patch":
                            myPatch = arg[1];
                            break;
                    }
                }
            if (myCall && !string.IsNullOrWhiteSpace(myPatch) && !string.IsNullOrWhiteSpace(myDestination))
                this.MainForm = new Form1(new WorkerMeta(myPatch, myDestination));
            else
                Environment.Exit(1);
        }
    }
}
