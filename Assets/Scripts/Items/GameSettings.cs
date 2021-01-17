using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public static class GameSettings
    {
        /// <summary>
        /// Variable is set to true on game start so that players wont
        /// take damage in the preperation room.
        /// </summary>
        public static bool gameHasStarted = false;
        public static bool friendlyFire = false;
        public static Connection activeConnection = Connection.None;
        public static Map selectedMap = Map.SnowCastle;
        public static float RespawnDelay
        {
            get
            {
                return Maybers.Prefs.Get("gamesettings - respawn delay", 2f);
            }
            set
            {
                Maybers.Prefs.Set("gamesettings - respawn delay", value);
            }

        }
        public static bool TeamDamage
        {
            get
            {
                return Maybers.Prefs.Get("gamesettings - team damage", false);
            }
            set
            {
                Maybers.Prefs.Set("gamesettings - team damage", value);
            }

        }

        public static IGameMode gameMode;

        public enum Map
        {
            IceRocks = 0,
            SnowCastle = 1
        }
        
        public enum Connection
        {
            Host,
            Client,
            None
        }
    }
}
