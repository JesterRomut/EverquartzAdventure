using Terraria.GameContent.Personalities;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Terraria.GameContent.Bestiary;
using CalamityMod;
using CalamityMod.World;
using System.Collections.Generic;

namespace EverquartzAdventure
{
    internal static partial class CalamityWeakRef
    {
        internal static bool IsProvDefeated()
        {
            return DownedBossSystem.downedProvidence;
        }
    }
}

namespace EverquartzAdventure.NPCs.TownNPCs
{
    [AutoloadHead]
    public class StarbornPrincess : ModNPC
    {
        //public override string Texture => "CalamityMod/NPCs/TownNPCs/WITCH";
        public override void SetStaticDefaults()
        {
            base.DisplayName.SetDefault("Starborn Princess");
            Main.npcFrameCount[base.NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[base.NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[base.NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[base.NPC.type] = 400;
            NPCID.Sets.AttackType[base.NPC.type] = 0;
            NPCID.Sets.AttackTime[base.NPC.type] = 60;
            NPCID.Sets.AttackAverageChance[base.NPC.type] = 15;
            base.NPC.Happiness.SetBiomeAffection<HallowBiome>(AffectionLevel.Love).SetBiomeAffection<DesertBiome>(AffectionLevel.Hate);
            NPCID.Sets.NPCBestiaryDrawModifiers nPCBestiaryDrawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            nPCBestiaryDrawModifiers.Velocity = 1f;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = nPCBestiaryDrawModifiers;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(base.NPC.type, drawModifiers);
        }

        public override void SetDefaults()
        {
            base.NPC.townNPC = true;
            base.NPC.friendly = true;
            base.NPC.lavaImmune = true;
            base.NPC.width = 18;
            base.NPC.height = 40;
            base.NPC.aiStyle = 7;
            base.NPC.damage = 1;
            base.NPC.defense = 0;
            base.NPC.lifeMax = 1;
            base.NPC.HitSound = SoundID.NPCHit1;
            base.NPC.DeathSound = SoundID.NPCDeath6;
            base.NPC.knockBackResist = 0.5f;
            base.AnimationType = 108;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[2]
            {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
            new FlavorTextBestiaryInfoElement("son of dog")
            });
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            if(ModCompatibility.calamityEnabled)
            {
                return CalamityWeakRef.IsProvDefeated();
            }
            else
            {
                return false;
            }
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Deimos" };
        }
    }
    }