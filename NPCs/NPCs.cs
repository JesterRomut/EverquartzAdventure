using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.NPCs.TownNPCs;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace EverquartzAdventure
{
    internal static partial class CalamityWeakRef
    {
        [JITWhenModsEnabled("CalamityMod")]
        internal static class NPCType
        {
            internal static int WITCH => ModContent.NPCType<WITCH>();
        }
    }
}

namespace EverquartzAdventure.NPCs
{
    public abstract class EverquartzNPC: ModNPC
    {
        public virtual string TownNPCDeathMessageKey => null;
        //public virtual Color? TownNPCDeathMessageColor => null;
    }
}
