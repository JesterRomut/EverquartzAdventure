using Terraria.GameContent.Personalities;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Terraria.GameContent.Bestiary;
using CalamityMod;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria.GameContent;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Utilities;
using Terraria.GameContent.Events;
using CalamityMod.NPCs.TownNPCs;
using EverquartzAdventure.Items.Critters;
using EverquartzAdventure.Items.Weapons;
using EverquartzAdventure.Items;
using System.Linq;
using CalamityMod.NPCs.Providence;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.Chat;
using Humanizer;
using EverquartzAdventure.UI;

namespace EverquartzAdventure
{
    internal static partial class CalamityWeakRef
    {
        internal static bool downedProv => DownedBossSystem.downedProvidence;

        internal static bool downedDoG => DownedBossSystem.downedDoG;

        internal static int CalamitasNPC => ModContent.NPCType<WITCH>();

        

        

        



        internal static bool HasElysianAegisBuff(Player player)
        {
            return player.Calamity().elysianAegis;
        }

        internal static void SummonProv(Player player)
        {
            SoundEngine.PlaySound(in Providence.SpawnSound, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int npc = NPC.NewNPC(new EntitySource_BossSpawn(player), (int)(player.position.X + (float)Main.rand.Next(-500, 501)), (int)(player.position.Y - 250f), ModContent.NPCType<Providence>(), 1);
                Main.npc[npc].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(npc);
            }
            else
            {
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<Providence>());
            }
        }
    }


}

namespace EverquartzAdventure.NPCs.TownNPCs
{
    [AutoloadHead]
    public class StarbornPrincess : ModNPC
    {
        //public override string Texture => "CalamityMod/NPCs/TownNPCs/WITCH";

        #region Fields
        public static SoundStyle HitSound => SoundID.FemaleHit;
        public static SoundStyle DeathSound => SoundID.NPCDeath6;
        #endregion

        #region LanguageKeys
        public static string DeathMessageKey => "Mods.EverquartzAdventure.NPCs.StarbornPrincess.DeathMessage";
        public static string ShopTextKey => "Mods.EverquartzAdventure.NPCs.StarbornPrincess.ShopText";
        public static string TransTextKey => "Mods.EverquartzAdventure.NPCs.StarbornPrincess.TransText";
        public static string BestiaryTextKey => "Mods.EverquartzAdventure.NPCs.StarbornPrincess.BestiaryText";
        public static string HelpListKey => "Mods.EverquartzAdventure.NPCs.StarbornPrincess.Help";

        public static string ChatHomelessKey => "Mods.EverquartzAdventure.NPCs.StarbornPrincess.Chat.Homeless";
        public static string ChatCommonKey => "Mods.EverquartzAdventure.NPCs.StarbornPrincess.Chat.Common";
        public static string ChatBloodMoonKey => "Mods.EverquartzAdventure.NPCs.StarbornPrincess.Chat.BloodMoon";
        public static string ChatPartyKey => "Mods.EverquartzAdventure.NPCs.StarbornPrincess.Chat.Party";
        public static string ChatPostDoGKey => "Mods.EverquartzAdventure.NPCs.StarbornPrincess.Chat.PostDoG";
        public static string ChatInHallowKey => "Mods.EverquartzAdventure.NPCs.StarbornPrincess.Chat.InHallow";
        public static string ChatCalamitasRefKey => "Mods.EverquartzAdventure.NPCs.StarbornPrincess.Chat.CalamitasRef";
        public static string ChatAnglerRefKey => "Mods.EverquartzAdventure.NPCs.StarbornPrincess.Chat.AnglerRef";

        public static string CensusConditionKey => "Mods.EverquartzAdventure.NPCs.StarbornPrincess.CensusCondition";

        #endregion

        #region Overrides
        public override void SetStaticDefaults()
        {
            base.DisplayName.SetDefault("Starborn Princess");
            DisplayName.AddTranslation(7, "星光公主");
            DisplayName.AddTranslation(6, "Принцесса, рождённая в небесах");
            Main.npcFrameCount[base.NPC.type] = 6;
            //NPCID.Sets.ExtraFramesCount[base.NPC.type] = 9;
            //NPCID.Sets.AttackFrameCount[base.NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[base.NPC.type] = 400;
            NPCID.Sets.AttackType[base.NPC.type] = 0;
            NPCID.Sets.AttackTime[base.NPC.type] = 60;
            NPCID.Sets.AttackAverageChance[base.NPC.type] = 15;
            base.NPC.Happiness.SetBiomeAffection<HallowBiome>(AffectionLevel.Love).SetBiomeAffection<DesertBiome>(AffectionLevel.Hate);
            NPCID.Sets.NPCBestiaryDrawModifiers nPCBestiaryDrawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            nPCBestiaryDrawModifiers.Velocity = 1f;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = nPCBestiaryDrawModifiers;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(base.NPC.type, drawModifiers);
            Main.npcCatchable[base.NPC.type] = true;
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return !toKingStatue;
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
            base.NPC.lifeMax = 500; //give her more health!!! justice for deimos :(
            base.NPC.HitSound = HitSound;
            base.NPC.DeathSound = DeathSound;
            base.NPC.knockBackResist = 0.5f;
            NPC.catchItem = ModContent.ItemType<StarbornPrincessItem>();
            if (ModCompatibility.calamityEnabled)
            {
                NPC.buffImmune[CalamityWeakRef.BuffType.HolyFlames] = true;
                NPC.buffImmune[CalamityWeakRef.BuffType.GodSlayerInferno] = true;
            }
            //base.AnimationType = 124;
        }

        public override void FindFrame(int frameHeight)
        {
            base.NPC.frameCounter += 0.15;
            base.NPC.frameCounter %= Main.npcFrameCount[base.NPC.type];
            int frame = (int)base.NPC.frameCounter;
            base.NPC.frame.Y = frame * frameHeight;
            NPC.spriteDirection = NPC.direction;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[3]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,

            new FlavorTextBestiaryInfoElement(BestiaryTextKey)
            });
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {

            if (ModCompatibility.calamityEnabled && CalamityWeakRef.downedProv)
            {

                return !Main.player.Any(player => player.HasItem(ModContent.ItemType<StarbornPrincessItem>())) &&
                    !Main.item.Any(item => item.active && item.type == ModContent.ItemType<StarbornPrincessItem>());
            }
            else
            {
                //Mod.Logger.Info("111");
                return false;
            }
        }

        public override void AI()
        {
            

        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            ModifyLoot(npcLoot);
        }
        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Deimos" };
        }

        public override string GetChat()
        {
            WeightedRandom<string> textSelector = new WeightedRandom<string>(Main.rand);
            if (base.NPC.homeless)
            {
                EverquartzUtils.GetTextListFromKey(ChatHomelessKey).ForEach(st => textSelector.Add(st));
            }
            else
            {
                EverquartzUtils.GetTextListFromKey(ChatCommonKey).ForEach(st => textSelector.Add(st));
                if (!Main.dayTime && Main.bloodMoon)
                {
                    EverquartzUtils.GetTextListFromKey(ChatBloodMoonKey).ForEach(st => textSelector.Add(st, 5.15));
                }
                if (BirthdayParty.PartyIsUp)
                {
                    EverquartzUtils.GetTextListFromKey(ChatPartyKey).ForEach(st => textSelector.Add(st, 5.5));
                }
                if (ModCompatibility.calamityEnabled)
                {
                    if (CalamityWeakRef.downedDoG)
                    {
                        EverquartzUtils.GetTextListFromKey(ChatPostDoGKey).ForEach(st => textSelector.Add(st));
                    }
                    if (NPC.AnyNPCs(CalamityWeakRef.CalamitasNPC))
                    {
                        EverquartzUtils.GetTextListFromKey(ChatCalamitasRefKey).ForEach(st => textSelector.Add(st, (0.8)));
                    }
                }
                if (Main.player[Main.myPlayer].ZoneHallow)
                {
                    EverquartzUtils.GetTextListFromKey(ChatInHallowKey).ForEach(st => textSelector.Add(st));
                }
                int angler = NPC.FindFirstNPC(NPCID.Angler);
                if (angler != -1)
                {
                    EverquartzUtils.GetTextListFromKey(ChatAnglerRefKey).ForEach(st => textSelector.Add(st.FormatWith(Main.npc[angler].GivenName), (0.8)));
                }
            }
            string thingToSay = textSelector.Get();
            return thingToSay;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                DeathEffectClient(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height);
            }
        }

        public override void OnKill()
        {
            DeathEffectOnKill(Main.player.Where(player => player.active && player != null).Random());
        }

        public override bool CheckDead()
        {
            base.NPC.active = false;
            base.NPC.HitEffect();
            base.NPC.NPCLoot();
            base.NPC.netUpdate = true;
            return false;
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue(ShopTextKey);
            button2 = Language.GetTextValue(TransTextKey);
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            shop = firstButton;
            if (!firstButton)
            {
                Main.playerInventory = true;
                // remove the chat window...
                Main.npcChatText = "";
                // and start an instance of our UIState.
                EverquartzUI.instance.userInterface.SetState(EverquartzUI.instance.transmogrificationUI);
            }
            
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            //shop.item[nextSlot].SetDefaults(ModContent.ItemType<EverquartzItem>());
            //shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 3);
            //nextSlot++;
            //shop.item[nextSlot].SetDefaults(ModContent.ItemType<MarsBar>());
            //shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 1);
            //nextSlot++;
            //shop.item[nextSlot].SetDefaults(ModContent.ItemType<DeimosFumo>());
            //shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 1);
            //nextSlot++;
            //shop.item[nextSlot].SetDefaults(ModContent.ItemType<DivineCore>());
            //shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 5);
            //nextSlot++;


            if (ModCompatibility.calamityEnabled)
            {
                shop.AddShopItem(ref nextSlot, CalamityWeakRef.ItemType.ProfanedCrucible, Item.buyPrice(gold: 60));
                shop.AddShopItem(ref nextSlot, CalamityWeakRef.ItemType.DivineGeode, Item.buyPrice(gold: 6));
                if (CalamityWeakRef.downedDoG)
                {
                    shop.AddShopItem(ref nextSlot, CalamityWeakRef.ItemType.NightmareFuel, Item.buyPrice(gold: 12));
                    shop.AddShopItem(ref nextSlot, CalamityWeakRef.ItemType.EndothermicEnergy, Item.buyPrice(gold: 12));
                    shop.AddShopItem(ref nextSlot, CalamityWeakRef.ItemType.DarksunFragment, Item.buyPrice(gold: 12));
                }

                Player player = Main.player[Main.myPlayer];
                if (player.HasItem(CalamityWeakRef.ItemType.ElysianAegis) || player.HasItem(CalamityWeakRef.ItemType.AsgardianAegis) || CalamityWeakRef.HasElysianAegisBuff(player))
                {
                    shop.AddShopItem(ref nextSlot, CalamityWeakRef.ItemType.RuneOfKos, Item.buyPrice(platinum: 2));
                }
            }

            shop.AddShopItem(ref nextSlot, ModContent.ItemType<DivineCore>(), Item.buyPrice(platinum: 5));
            shop.AddShopItem(ref nextSlot, ModContent.ItemType<DeimosFumo>(), Item.buyPrice(gold: 1));
            shop.AddShopItem(ref nextSlot, ModContent.ItemType<SundialNimbus>(), Item.buyPrice(gold: 3));
            shop.AddShopItem(ref nextSlot, ModContent.ItemType<MarsBar>(), Item.buyPrice(gold: 1));

        }
        #endregion

        #region Utils
        public static void ModifyLoot(ILoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<SundialNimbus>()));
            loot.Add(ItemDropRule.Common(ModContent.ItemType<StarFlare>()));
            if (ModCompatibility.calamityEnabled)
            {
                loot.Add(ItemDropRule.Common(ModContent.ItemType<DeimosBow>()));
                loot.Add(ItemDropRule.Common(ModContent.ItemType<DeimosStaff>()));
            }

        }
        
        #endregion

        #region DeathEffect
        public static void DeathEffectClient(Vector2 position, int width, int height)
        {
            SoundEngine.PlaySound(DeathSound, position);
            for (int num585 = 0; num585 < 25; num585++)
            {
                int num586 = Dust.NewDust(position, width, height, 31, 0f, 0f, 100, default(Color), 2f);
                Dust dust30 = Main.dust[num586];
                Dust dust187 = dust30;
                dust187.velocity *= 1.4f;
                Main.dust[num586].noLight = true;
                Main.dust[num586].noGravity = true;
            }
        }

        public static void ItemDeathEffectClient(Vector2 position, int width, int height, int helptext)
        {
            SoundEngine.PlaySound(StarbornPrincess.HitSound, position);
            CombatText.NewText(new Rectangle((int)Math.Round(position.X), (int)Math.Round(position.Y), width, height), new Color(225, 219, 238
            ), EverquartzUtils.GetTextListFromKey(HelpListKey).ElementAtOrDefault(helptext) ?? "Error", true);
            DeathEffectClient(position, width, height);
        }



        public static void DeathEffectOnKill(Player player)
        {
            NetworkText networkText = NetworkText.FromKey(DeathMessageKey);

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(networkText.ToString(), byte.MaxValue, 25, 25);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(networkText, new Color(255, 25, 25));
            }

            if (ModCompatibility.calamityEnabled)
            {
                CalamityWeakRef.SummonProv(player);
            }
        }

        public static void ItemDeathEffectServer(Player player, int helptext)
        {

            if (Main.netMode == NetmodeID.SinglePlayer)
            {

                ItemDeathEffectClient(player.position, player.width, player.height, helptext);
            }
            else if (Main.netMode == NetmodeID.Server)
            {

                ModPacket packet = EverquartzAdventureMod.Instance.GetPacket();
                packet.Write((byte)EverquartzMessageType.DeimosItemKilled);
                packet.Write(player.whoAmI);
                packet.Write(helptext);
                packet.Send();
            }
            DeathEffectOnKill(player);
        }
        #endregion

        #region Transmogrification
        #endregion
    }
}