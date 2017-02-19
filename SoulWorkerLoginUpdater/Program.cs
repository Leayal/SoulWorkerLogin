using System;
using System.Windows.Forms;

namespace SoulWorkerLoginUpdater
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyLoader.AssemblyResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += AssemblyLoader.AssemblyResolve;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            new CustomWindowsFormsApplicationBase().Run(Environment.GetCommandLineArgs());
        }
    }
}
