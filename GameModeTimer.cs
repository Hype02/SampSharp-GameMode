using System;
using System.Collections.Generic;
using System.Text;
using SampSharp.GameMode;
using SampSharp;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Factories;
using SampSharp.GameMode.SAMP;

namespace SAPC
{
    public class GameModeTimer
    {
        public GameModeTimer()
        {
           
        }
        public void Init()
        {
            new Timer(1000, true).Tick+=GameModeTimerTick;
            void GameModeTimerTick(object sender, EventArgs e)
            {
                foreach(Player player in Player.All)
                {

                }
            }
        }
    }
}
