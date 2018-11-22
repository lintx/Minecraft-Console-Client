using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.ChatBots
{
    class AutoFish : ChatBot
    {
        private static short QUICK_BAR_START = 36;
        private short[] QuickBarItem = new short[9];
        private short Slot = -1;
        private int ProtocolVersion;
        private DateTime LastTime = DateTime.Now;
        private bool Fishing = false;
        private int FishrowEntityId = 0;

        public AutoFish()
        {
            LogToConsole("This is open and load.");
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void AfterGameJoined()
        {
            ProtocolVersion = GetProtocolVersion();
            base.AfterGameJoined();
        }

        public override void OnHeldItemSlot(short slot)
        {
            Slot = slot;
            AutoSwitchToFishRod();
            base.OnHeldItemSlot(slot);
        }

        public override void OnSetSlot(byte windowId, short slot, short itemId, short itemCount)
        {
            if (windowId == 0 && slot >= QUICK_BAR_START && slot < QUICK_BAR_START + 9)
            {
                slot -= QUICK_BAR_START;
                QuickBarItem[slot] = itemId;
                AutoSwitchToFishRod();
            }
            base.OnSetSlot(windowId, slot, itemId, itemCount);
        }

        public override void Update()
        {
            TimeSpan interval = DateTime.Now - LastTime;
            int time = (int)((DateTime.Now - LastTime).TotalSeconds);
            if (Fishing)
            {
                if (time > Settings.AutoFish_Timeout)
                {
                    Fishing = false;
                    UseFishRod();
                    LogToConsole("Fishing time out.");
                }
            }
            else
            {
                if (time >= Settings.AutoFish_Delay && CanFish())
                {
                    UseFishRod();
                }
            }
            base.Update();
        }

        public override void OnSpawnEntity(int entityId, short type, Guid UUID, Mapping.Location location)
        {
            if (type == 90)
            {
                TimeSpan interval = DateTime.Now - LastTime;
                if (interval.TotalMilliseconds <= 500)
                {
                    FishrowEntityId = entityId;
                    Fishing = true;
                }
            }
            base.OnSpawnEntity(entityId, type, UUID, location);
        }

        public override void OnEntityDestroy(int[] entitys)
        {
            if (Fishing && FishrowEntityId > 0)
            {
                for (int i = 0; i < entitys.Length; i++)
                {
                    if (entitys[i] == FishrowEntityId)
                    {
                        Fishing = false;
                        FishrowEntityId = 0;
                    }
                }
            }
            base.OnEntityDestroy(entitys);
        }

        public override void OnEntityMoveLook(int entityId, short dX, short dY, short dZ)
        {
            if (Fishing && FishrowEntityId == entityId)
            {
                if (dX == 0 && dZ == 0 && dY < -800)
                {
                    UseFishRod();
                }
            }
            base.OnEntityMoveLook(entityId, dX, dY, dZ);
        }

        private void UseFishRod()
        {
            LastTime = DateTime.Now;
            UseItem(0);
        }

        private bool CanFish()
        {
            if (Slot < 0 || Slot > 8) return false;
            if (ProtocolVersion==404)
            {
                return QuickBarItem[Slot] == 568;
            }
            return false;
        }

        private void AutoSwitchToFishRod()
        {
            if (Slot < 0 || Slot > 8) return;
            if (ProtocolVersion == 404)
            {
                if (QuickBarItem[Slot] != 568)
                {
                    for (short i = 0; i < QuickBarItem.Length; i++)
                    {
                        if (QuickBarItem[i] == 568)
                        {
                            HeldItemSlot(i);
                        }
                    }
                }
            }
        }
    }
}
