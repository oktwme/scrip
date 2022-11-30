using System;
using System.Reflection;
using System.Security.Permissions;
using LoadMachine.Properties;
using EnsoulSharp.SDK;

namespace LoadMachine
{
    class Program
    {
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        static void Main(string[] args)
        {
            GameEvent.OnGameLoad += GameEventOnOnGameLoad;
        }
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void GameEventOnOnGameLoad()
        {
            
            LoadAssembly(Resources.PerfectAIO, new string[1]);
        }
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void LoadAssembly(byte[] dll, string[] args)
        {
            try
            {
                Assembly assembly = Assembly.Load(dll);
                if (assembly != null)
                {
                    if (assembly.EntryPoint != null)
                    {
                        assembly.EntryPoint.Invoke(null, new object[]
                        {
                            args
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
