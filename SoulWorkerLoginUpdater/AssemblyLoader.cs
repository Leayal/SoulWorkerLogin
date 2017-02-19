using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoulWorkerLoginUpdater
{
    public sealed partial class AssemblyLoader
    {
        private static Dictionary<string, Assembly> myDict;

        public static Assembly AssemblyResolve(object sender, ResolveEventArgs e)
        {
            if (myDict == null)
                myDict = new Dictionary<string, Assembly>();
            string RealName = e.Name.Split(',')[0].Trim();
            if (myDict.ContainsKey(RealName))
                return myDict[RealName];
            else
            {
                byte[] bytes;
                string resourceName = "SoulWorkerLoginUpdater." + RealName + ".dll";
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                using (System.IO.Stream stream = currentAssembly.GetManifestResourceStream(resourceName))
                {
                    bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                }
                Assembly result = Assembly.Load(bytes);
                myDict.Add(RealName, result);
                bytes = null;
                return result;
            }
        }
    }
}
