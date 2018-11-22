using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.ChatBots
{
    class AutoRespawn : ChatBot
    {
        public AutoRespawn()
        {
            LogToConsole("This is open and load.");
        }

        public override void OnUpdateHealth(float health, int food, float foodSaturation)
        {
            if (health <= 0)
            {
                LogToConsole("auto respawn");
                SendRespawnPacket();
            }
        }
    }
}
