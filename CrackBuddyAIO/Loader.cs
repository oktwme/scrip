using System;
using System.IO;
using System.Net;
using System.Reflection;
using EnsoulSharp;
using EnsoulSharp.SDK;

namespace Loader
{
    internal class CrackBuddy
    {
        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad()
        {
            // crackbuddy - discord.gg/V4ZdTKtZgN
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                byte[] load = new System.Net.WebClient().DownloadData("https://gitlab.com/ensoul/crackbuddy/-/raw/main/Main.exe");
                Assembly assembly = Assembly.Load(load);
                if (assembly != null)
                {
                    if (assembly.EntryPoint != null)
                    {
                        assembly.EntryPoint.Invoke(null, new object[]
                        {
                            new string[1]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Game.Print("<font color='#d4394b' size='26'>[<font color='#0096ff'>CrackBuddy Server</font>]</font> <font color='#d4394b'>Server is down, contact a admin!</font>");
            }
        }
    }
}