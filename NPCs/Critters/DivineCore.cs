using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using CalamityMod.BiomeManagers;
using CalamityMod.Items.Critters;
using CalamityMod;

namespace EverquartzAdventure.NPCs.Critters
{
    public class DivineCore: ModNPC
    {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Divine Core");
            DisplayName.AddTranslation(7, "神圣核心");
            DisplayName.AddTranslation(6, "Божественное Ядро");
            //NPCID.Sets.CountsAsCritter[base.NPC.type] = true;
            Main.npcCatchable[base.NPC.type] = true;
        }
        public override void SetDefaults()
        {
            //base.NPC.CloneDefaults(NPCID.LightningBug);
            NPC.damage = 0;
            NPC.defense = 0;
            base.NPC.friendly = true;
            NPC.lifeMax = 5;
            //NPC.npcSlots = 0.2f;
            NPC.noGravity = true;
            base.NPC.width = 42;
            base.NPC.height = 102;
            base.NPC.HitSound = SoundID.NPCHit4;
            base.NPC.DeathSound = SoundID.NPCDeath44;
            base.NPC.catchItem = ModContent.ItemType<Items.Critters.DivineCore>();
            base.NPC.aiStyle = 2;
            AIType = NPCID.DemonEye;
        }

        public override void AI()
        {
            base.AI();
            NPC.rotation = NPC.velocity.ToRotation();
        }
    }
}
