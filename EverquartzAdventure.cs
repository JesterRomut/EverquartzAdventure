using System.Collections.Generic;
using Terraria.ModLoader;
using System.Linq;
using Terraria;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using EverquartzAdventure.NPCs.TownNPCs;
using CalamityMod.NPCs.TownNPCs;
using static Terraria.ModLoader.PlayerDrawLayer;
using System;
using Terraria.Audio;
using Terraria.Localization;

namespace EverquartzAdventure
{
	public class EverquartzAdventureMod : Mod
	{
        public override void PostSetupContent()
        {
			ModCompatibility.calamityEnabled = ModLoader.HasMod("CalamityMod");
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            EverquartzMessageType msgType = (EverquartzMessageType)reader.ReadByte();
            switch (msgType)
            {
                case EverquartzMessageType.DeimosItemKilled:
                    //Main.player[reader.ReadInt32()];
                    Player murderer = Main.player[reader.ReadInt32()];
                    int helptext = reader.ReadInt32();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        StarbornPrincess.ItemDeathEffectServer(murderer, helptext);
                    }
                    else
                    {
                        
                        StarbornPrincess.ItemDeathEffectClient(murderer.position, murderer.width, murderer.height, helptext);
                    }
                        
                    break;
            }
        }
    }

    public class EverquartzPlayer: ModPlayer
    {
        public void HandleDeadDeimos(Player murderer)
        {
            StarbornPrincess.DeathEffectClient(murderer.position, murderer.width, murderer.height);
        }
    }

    public enum EverquartzMessageType
    {
        DeimosItemKilled // id, player, helptext
    }

	public static class ModCompatibility
	{
		public static bool calamityEnabled = false;
	}

	[JITWhenModsEnabled("CalamityMod")]
	internal static partial class CalamityWeakRef
	{

	}

    internal static class EverquartzExtensions
    {
        internal static T Random<T>(this IEnumerable<T> li)
        {
            return li.ElementAt(Main.rand.Next(li.Count()));
        }

        
    }

    internal static class EverquartzUtils
    {
        internal static List<string> GetTextListFromKey(string key)
        {
            int index = 0;
            List<string> li = new List<string>();
            while (true)
            {
                if (index > 300)
                {
                    break;
                }
                string lineKey = $"{key}.{index}";
                string line = Language.GetTextValue(lineKey);
                if (line == lineKey || line == null || line == string.Empty)
                {
                    break;
                }
                else
                {
                    li.Add(line);
                }
                index++;
            }
            return li;
        }
    }
    }