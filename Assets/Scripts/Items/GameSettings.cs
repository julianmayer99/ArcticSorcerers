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

        public static IGameMode gameMode;
    }
}
