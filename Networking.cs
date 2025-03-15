using KrofEngine;
//using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krof
{
    internal class Networking
    {
        public static Networking Instance;
        public static string steamUserName;
        public Networking()
        {
            //Instance = this;
            //if (!SteamAPI.Init())
            //{
            //    Console.WriteLine("Steam API failed to initialize!");
            //}
            //else
            //{
            //    Console.WriteLine("Steam API initialized successfully!");
            //    steamUserName = SteamFriends.GetPersonaName();
            //    Console.WriteLine("Steam Username: " + steamUserName);
            //}
            //GameManager.OnApplicationQuit += delegate () { SteamAPI.Shutdown(); };
        }
    }
}
