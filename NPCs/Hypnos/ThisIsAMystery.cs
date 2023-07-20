using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Humanizer;
using Terraria.Utilities;
using Terraria.GameContent.Events;
using Hypnos;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.Audio;
using Terraria.GameContent.Personalities;
using CalamityMod;
using Terraria.DataStructures;
using static Terraria.ModLoader.PlayerDrawLayer;
using System.Collections;
using System.IO;
using Terraria.ModLoader.IO;
using EverquartzAdventure.NPCs.TownNPCs;
using EverquartzAdventure.Projectiles.Hypnos;

namespace EverquartzAdventure
{
    internal static partial class HypnosWeakRef
    {
        internal static bool downedHypnos => HypnosWorld.downedHypnos;

    }

    internal static partial class CalamityWeakRef
    {
        internal static bool downedExoMechs => DownedBossSystem.downedExoMechs;

    }
}

namespace EverquartzAdventure.NPCs.Hypnos
{
    public enum HypnosReward
    {
        Coins,
        ExoPrisms,
        Eucharist,
        Hypnotize,
        Euthanasia
    }

    [AutoloadHead]
    [LegacyName(new string[] { "HYPNO" })]
    public class Hypnos : ModNPC
    {
        #region ExtraAssets
        public static readonly SoundStyle IPutTheSoundFileInLocalBecauseICouldntKnowCalamitysPathOfThis = new SoundStyle("EverquartzAdventure/Sounds/ExoMechs/ExoLaserShoot"); 
        public static readonly SoundStyle ICannotFindThisEither = new SoundStyle("EverquartzAdventure/Sounds/ExoMechs/ExoHit", 4)
        {
            Volume = 0.4f
        };


        public static readonly Asset<Texture2D> eyepatchTex = ModContent.Request<Texture2D>("EverquartzAdventure/NPCs/Hypnos/Eyepatch");
        public static readonly Asset<Texture2D> glowTex = ModContent.Request<Texture2D>("EverquartzAdventure/NPCs/Hypnos/Hypnos_Glow");

        #endregion

        #region LanguageKeys
        public static string ButtonTextKey => "Mods.EverquartzAdventure.NPCs.Hypnos.ButtonText";
        public static string BestiaryTextKey => "Mods.EverquartzAdventure.NPCs.Hypnos.BestiaryText";

        public static string ChatCommonKey => "Mods.EverquartzAdventure.NPCs.Hypnos.Chat.Common";
        public static string ChatBloodMoonKey => "Mods.EverquartzAdventure.NPCs.Hypnos.Chat.BloodMoon";
        public static string ChatPartyKey => "Mods.EverquartzAdventure.NPCs.Hypnos.Chat.Party";
        public static string ChatPreHypnosKey => "Mods.EverquartzAdventure.NPCs.Hypnos.Chat.PreHypnos";
        public static string ChatPostHypnosKey => "Mods.EverquartzAdventure.NPCs.Hypnos.Chat.PostHypnos";
        public static string ChatPrayKey => "Mods.EverquartzAdventure.NPCs.Hypnos.Chat.Pray";
        public static string ChatPrayWithoutMoneyKey => "Mods.EverquartzAdventure.NPCs.Hypnos.Chat.PrayWithoutMoney";
        public static string ChatDeimosRefKey => "Mods.EverquartzAdventure.NPCs.Hypnos.Chat.DeimosRef";
        #endregion LanguageKeys

        #region Consts
        public static readonly double despawnTime = 28000;
        
        #endregion

        #region ServerSideVariables
        public static double timePassed = 0;
        public static double spawnTime = double.MaxValue;
        public static int hypnoCoins = 0;

        #endregion ServerSideVariables

        #region UniversalVariables
        //public bool observingSleepingPlayer = false;
        #endregion

        #region InstanceManagement
        public static int instance = -1;
        public static NPC Instance
        {
            get
            {
                if (instance == -1)
                {
                    return null;
                }
                NPC hypnos = Main.npc.ElementAtOrDefault(instance);
                if (hypnos == default || !hypnos.active)
                {
                    instance = -1;
                    return null;
                }
                return hypnos;
            }
        }
        #endregion InstanceManagement

        #region HandleInternet
        public static void HandleDepartHypnosUniversal(NPC hypnos)
        {
            hypnos.active = false;
            hypnos.netSkip = -1;
            hypnos.life = 0;
            //hypnos = null;
            timePassed = 0;
        }
        public static void HandleHypnoCoinAddServer()
        {
            hypnoCoins++;
        }

        public static void HandleRewardsServer(Player player, List<HypnosReward> rewards)
        {

            player.PutItemInInventoryFromItemUsage(ModContent.ItemType<Indulgence>(), player.selectedItem);
            //Item.NewItem(player.GetSource_GiftOrReward(), player.Center, ModContent.ItemType<Indulgence>(), noGrabDelay: true);
            rewards.ForEach(reward => HandleRewardServer(player, reward));
        }

        public static void HandleRewardServer(Player player, HypnosReward reward)
        {

            Vector2 position = Instance == null ? Instance.Center : player.Center;
            //ModContent.GetInstance<EverquartzAdventureMod>().Logger.Info(reward);

            void SpawnItem(int type, int stack = 1) { Item.NewItem(player.GetSource_GiftOrReward(), position, type, stack); }
            switch (reward)
            {
                case HypnosReward.Coins:
                    SpawnItem(ItemID.GoldCoin, Main.rand.Next(1, 3));
                    break;
                case HypnosReward.ExoPrisms:
                    if (ModCompatibility.calamityEnabled)
                    {
                        SpawnItem(CalamityWeakRef.ItemType.ExoPrism, Main.rand.Next(3, 7));
                    }
                    break;
                case HypnosReward.Eucharist:
                    SpawnItem(ItemID.GoldenDelight);
                    break;
                case HypnosReward.Hypnotize:
                    player.AddBuff(BuffID.Webbed, 240);
                    //player.AddBuff(ModContent.BuffType<Mindcrashed>(), 1200);
                    break;
                case HypnosReward.Euthanasia:
                    player.Hurt(PlayerDeathReason.ByOther(10), 200, 0);
                    break;
            }
        }
        #endregion

        #region Overrides
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            if (AergiaNeuron.AllNeurons.Count >= 12)
            {
                return;
            }
            projType = ModContent.ProjectileType<AergiaNeuron>();
            attackDelay = 1;
            NPC.localAI[3] = 0;
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {

            damage = 1;
            knockback = 0;
        }

        //public override void SendExtraAI(BinaryWriter writer)
        //{
        //    writer.Write(observingSleepingPlayer);

        //}

        //public override void ReceiveExtraAI(BinaryReader reader)
        //{
        //    observingSleepingPlayer = reader.ReadBoolean();
        //}



        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Soul of the Eternal Intellect of Infinite Verboten Knowledge");
            //DisplayName.AddTranslation(7, "无限禁忌知识的永恒智慧之魂");
            //DisplayName.AddTranslation(6, "Душа Вечного Интелекта Бесконечных Запрещённых Знаний"); // is that a meme item? or from community remix? - blitz
            ////                                                                                          ↑it's from hypnocord
            //NPCID.Sets.ActsLikeTownNPC[Type] = true;
            //NPCID.Sets.SpawnsWithCustomName[Type] = true;
            Main.npcFrameCount[base.NPC.type] = 13;

            NPCID.Sets.TownNPCBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            //NPCID.Sets.BossBestiaryPriority.Add(base.Type);

            NPCID.Sets.AttackType[Type] = 2;
            NPCID.Sets.MagicAuraColor[Type] = Color.Purple;
            NPCID.Sets.AttackTime[Type] = 100;
            NPCID.Sets.DangerDetectRange[Type] = 500;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[2]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
            new FlavorTextBestiaryInfoElement(BestiaryTextKey)
            });
        }

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.townNPC = true;
            NPC.width = 15;
            NPC.height = 22;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 200;
            NPC.defense = 90;
            NPC.homeTileX = -1;
            NPC.homeTileY = -1;
            NPC.homeless = true;

            NPC.HitSound = ICannotFindThisEither;
            NPC.DeathSound = SoundID.Item14;
            NPC.knockBackResist = 0.5f;
            TownNPCStayingHomeless = true;
            NPC.trapImmune = true;
            base.NPC.lavaImmune = true;
            //NPC.rarity = 2;//设置稀有度
            //AnimationType = npcID;

            //base.NPC.Happiness.SetBiomeAffection<OceanBiome>(AffectionLevel.Dislike);

            NPC.lifeMax = 1320000;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)/* tModPorter Suggestion: Copy the implementation of NPC.SpawnAllowed_Merchant in vanilla if you to count money, and be sure to set a flag when unlocked, so you don't count every tick. */
        {
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            if (NPC.velocity.Y > 0)
            {
                NPC.frame.Y = frameHeight;
                base.NPC.frameCounter = 0;
                return;
            }
            if (NPC.velocity.X == 0)
            {
                NPC.frame.Y = 0;
                base.NPC.frameCounter = 0;
            }
            else
            {
                base.NPC.frameCounter += 0.3;
                base.NPC.frameCounter %= Main.npcFrameCount[base.NPC.type] - 2;
                int frame = (int)base.NPC.frameCounter;
                base.NPC.frame.Y = (frame + 2) * frameHeight;
            }



        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return false;
        }

        public override bool CanChat()
        {
            return true;
        }
        public override void OnKill()
        {
            instance = -1;
            DropCoins();
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue(ButtonTextKey, Lang.inter[16].Value);
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            Player player = Main.player[Main.myPlayer];



            Pray(player);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                instance = -1;
                SoundEngine.PlaySound(NPC.DeathSound, NPC.position);
                for (int num585 = 0; num585 < 25; num585++)
                {
                    int num586 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default(Color), 2f);
                    Dust dust30 = Main.dust[num586];
                    Dust dust187 = dust30;
                    dust187.velocity *= 1.4f;
                    Main.dust[num586].noLight = true;
                    Main.dust[num586].noGravity = true;
                }
            }
        }
        public override bool CheckDead()
        {
            NPC.active = false;
            return true;
        }

        public override string GetChat()
        {
            WeightedRandom<string> textSelector = new WeightedRandom<string>(Main.rand);
            EverquartzUtils.GetTextListFromKey(ChatCommonKey).ForEach(st => textSelector.Add(st));
            if (!Main.dayTime && Main.bloodMoon)
            {
                EverquartzUtils.GetTextListFromKey(ChatBloodMoonKey).ForEach(st => textSelector.Add(st, 5.15));
            }
            if (BirthdayParty.PartyIsUp)
            {
                EverquartzUtils.GetTextListFromKey(ChatPartyKey).ForEach(st => textSelector.Add(st, 5.5));
            }
            if (ModCompatibility.hypnosEnabled && HypnosWeakRef.downedHypnos)
            {
                EverquartzUtils.GetTextListFromKey(ChatPostHypnosKey).ForEach(st => textSelector.Add(st));

            }
            else
            {
                EverquartzUtils.GetTextListFromKey(ChatPreHypnosKey).ForEach(st => textSelector.Add(st));
            }
            if (NPC.AnyNPCs(ModContent.NPCType<StarbornPrincess>()))
            {
                EverquartzUtils.GetTextListFromKey(ChatDeimosRefKey).ForEach(st => textSelector.Add(st));
            }

            string thingToSay = textSelector.Get();
            return thingToSay;
        }



        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 position = NPC.Center - screenPos + new Vector2(3 + (NPC.spriteDirection == 1 ? -7 : 0), -9f);
            spriteBatch.Draw
            (
                TextureAssets.Npc[base.NPC.type].Value,
                position,
                NPC.frame,
                drawColor,
                NPC.rotation,
                NPC.frame.Size() / 2,
                NPC.scale,
                NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
            );

            Draw(glowTex.Value, spriteBatch, screenPos, position, Color.White);
            if (NPC.spriteDirection == 1)
            {
                Draw(eyepatchTex.Value, spriteBatch, screenPos, position, drawColor);
            }

            return false;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Hypnos" };
        }
        #endregion

        #region AI
        public override void AI()
        {
            instance = NPC.whoAmI;
            //CombatText.NewText(NPC.Hitbox, Color.White, timePassed.ToString());
            NPC.homeless = true;

            if (NPC.homeTileX == -1 || NPC.homeTileY == -1)
            {
                FindHomeTile(out int tileX, out int tileY);
                FindHomeTileAndSpawnPointTraveling(tileX, tileY, out NPC.homeTileX, out NPC.homeTileY, out int _, out int _);
            }

            if (Main.rand.NextBool(200))
            {
                
                if (NPC.homeTileX != -1 &&  NPC.homeTileY != -1)
                {
                    Point homePoint = new Point(NPC.homeTileX, NPC.homeTileY - 3);

                    Vector2 home = homePoint.ToWorldCoordinates(0, 0);
                    //Dust.NewDustPerfect(home, DustID.Electric, Vector2.Zero);
                    //EverquartzAdventureMod.Instance.Logger.Info($"{Vector2.Distance(NPC.position, home) > 500f} && {EverquartzUtils.TileCapable(homePoint.X, homePoint.Y)} && {!IsNpcOnscreen(NPC.Center)} && {!IsNpcOnscreen(home)}");
                    if (Vector2.Distance(NPC.position, home) > 1000f && EverquartzUtils.TileCapable(homePoint.X, homePoint.Y) && !IsNpcOnscreen(NPC.Center) && !IsNpcOnscreen(home))
                    {
                        NPC.position = home;
                        NPC.velocity = Vector2.Zero;
                        NPC.netUpdate = true;

                    }
                }
                
            }

            // massive linq
            //if (Main.rand.NextBool(3))
            //{
            //    //IEnumerable<Player> sleepingPlayers = Main.player.Where(p => p != null && p.active && p.sleeping.isSleeping);
            //    //if (sleepingPlayers.Any())
            //    //{
            //    //    if (!observingSleepingPlayer)
            //    //    {
            //    //        observingSleepingPlayer = true;

            //    //        FindBestPraisingTileWhilePlayerSleeping(sleepingPlayers.Random(), out NPC.homeTileX, out NPC.homeTileY);
            //    //        NPC.netUpdate = true;
            //    //    }

            //    //}
            //    //else
            //    //{
            //    //    if (observingSleepingPlayer)
            //    //    {
            //    //        observingSleepingPlayer = false;

            //    //        FindBestPraisingTileFromNearestTownNPC(ref NPC.homeTileX, ref NPC.homeTileY);
            //    //        NPC.netUpdate = true;
            //    //    }
            //    //}

            //    NPC.homeless = true;
            //    Func<NPC, bool> pred = (npc => npc.type == ModContent.NPCType<Hypnos>() && npc.whoAmI != NPC.whoAmI);
            //    if (Main.npc.Any(pred))
            //    {
            //        Main.npc.Where(pred).ToList().ForEach(npc => npc.active = false);
            //    }
            //}


        }




        #endregion

        #region Traveling
        public static bool ShouldDespawn => timePassed >= despawnTime;
        public static void UpdateTravelingMerchant()
        {
            NPC hypnos = Instance;
            // Find an Explorer if there's one spawned in the world
            if (hypnos != null && ShouldDespawn && !IsNpcOnscreen(hypnos.Center)) // If it's past the despawn time and the NPC isn't onscreen
            {
                // Here we despawn the NPC and send a message stating that the NPC has despawned
                string fullName = hypnos.FullName;

                HandleDepartHypnosUniversal(hypnos);
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(Lang.misc[35].Key, fullName), 50, 125);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = EverquartzAdventureMod.Instance.GetPacket();
                    packet.Write((byte)EverquartzMessageType.HypnosDeparted);
                    packet.Send();
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[35].Key, fullName), new Color(50, 125, 255));
                }


                timePassed = 0;
            }

            if (hypnos != null)
            {
                timePassed++;
            }
            else
            {
                timePassed = 0;
            }



            // Main.time is set to 0 each morning, and only for one update. Sundialling will never skip past time 0 so this is the place for 'on new day' code
            if (Main.dayTime && Main.time == 0)
            {
                // insert code here to change the spawn chance based on other conditions (say, npcs which have arrived, or milestones the player has passed)
                // You can also add a day counter here to prevent the merchant from possibly spawning multiple days in a row.

                // NPC won't spawn today if it stayed all night
                if (hypnos == null && NPC.downedMoonlord && Main.rand.NextBool(5))
                { // 4 = 25% Chance
                  // Here we can make it so the NPC doesnt spawn at the EXACT same time every time it does spawn
                    spawnTime = GetRandomSpawnTime(1, 54000); // minTime = 6:00am, maxTime = 7:30am
                    //if (Main.netMode == NetmodeID.Server)
                    //{
                    //    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"debug: spawntime = {spawnTime}"), Color.Purple);
                    //}
                }
                else
                {
                    spawnTime = double.MaxValue; // no spawn today
                }
            }

            // Spawn the traveler if the spawn conditions are met (time of day, no events, no sundial)
            if (hypnos == null && CanSpawnNow())
            {
                FindHomeTile(out int tileX, out int tileY);

                //FindSpawnPoint(new Point(homeX, homeY), out int bestX, out int bestY);
                FindHomeTileAndSpawnPointTraveling(tileX, tileY, out int homeX, out int homeY, out int spawnX, out int spawnY);

                //EverquartzAdventureMod.Instance.Logger.Info($"({homeX}, {homeY}) ({bestX}, {bestY})");

                hypnos = NPC.NewNPCDirect(Terraria.Entity.GetSource_TownSpawn(), spawnX, spawnY, ModContent.NPCType<Hypnos>(), 1); // Spawning at the world spawn

                hypnos.homeless = true;
                hypnos.direction = spawnX / 16 >= homeX ? -1 : 1;
                hypnos.netUpdate = true;
                hypnos.homeTileX = homeX;
                hypnos.homeTileY = homeY;

                // Prevents the traveler from spawning again the same day
                spawnTime = double.MaxValue;
                timePassed = 0;

                // Annouce that the traveler has spawned in!
                if (Main.netMode == NetmodeID.SinglePlayer) Main.NewText(Language.GetTextValue("Announcement.HasArrived", hypnos.FullName), 50, 125, 255);
                else ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasArrived", hypnos.FullName), new Color(50, 125, 255));
            }
        }

        private static bool CanSpawnNow()
        {
            // can't spawn if any events are running

            // can't spawn if the sundial is active
            if (Main.IsFastForwardingTime()/* tModPorter Note: Removed. Suggestion: IsFastForwardingTime(), fastForwardTimeToDawn or fastForwardTimeToDusk */)
                return false;

            // can spawn if daytime, and between the spawn and despawn times
            return Main.dayTime && Main.time >= spawnTime;
        }

        private static void FindHomeTile(out int homeTileX, out int homeTileY)
        {
            List<Player> players = Main.player.Where(p => p != null && p.active).ToList();
            players.ForEach(player => player.FindSpawn());
            IEnumerable<Player> playersChangedSpawn = players.Where(p => p.SpawnX != -1 && p.SpawnY != -1);
            if (playersChangedSpawn.Any())
            {
                Player random = playersChangedSpawn.Random();
                homeTileX = random.SpawnX;
                homeTileY = random.SpawnY;
                return;
            }
            IEnumerable<NPC> npcs = Main.npc.Where(npc => npc.active && npc.townNPC && npc.type != NPCID.OldMan && !npc.homeless && !IsNpcOnscreen(new Point(npc.homeTileX, npc.homeTileY).ToVector2()));
            if (npcs.Any())
            {
                NPC randomNPC = npcs.Random();
                homeTileX = randomNPC.homeTileX;
                homeTileY = randomNPC.homeTileY;
                return;
            }
            homeTileX = Main.spawnTileX;
            homeTileY = Main.spawnTileY;
        }

        public static void FindHomeTileAndSpawnPointTraveling(int homeTileX, int homeTileY, out int homeX, out int homeY, out int spawnX, out int spawnY)
        {
            int bestX = homeTileX;
            int bestY = homeTileY;
            int minValue = bestX;
            int num7 = bestX;
            int num8 = bestY;
            int num9 = bestX;
            while (num9 > bestX - 10 && (WorldGen.SolidTile(num9, num8) || Main.tileSolidTop[Main.tile[num9, num8].TileType]) && (!Main.tile[num9, num8 - 1].HasTile || !Main.tileSolid[Main.tile[num9, num8 - 1].TileType] || Main.tileSolidTop[Main.tile[num9, num8 - 1].TileType]) && (!Main.tile[num9, num8 - 2].HasTile || !Main.tileSolid[Main.tile[num9, num8 - 2].TileType] || Main.tileSolidTop[Main.tile[num9, num8 - 2].TileType]) && (!Main.tile[num9, num8 - 3].HasTile || !Main.tileSolid[Main.tile[num9, num8 - 3].TileType] || Main.tileSolidTop[Main.tile[num9, num8 - 3].TileType]))
            {
                minValue = num9;
                num9--;
            }
            for (int k = bestX; k < bestX + 10 && (WorldGen.SolidTile(k, num8) || Main.tileSolidTop[Main.tile[k, num8].TileType]) && (!Main.tile[k, num8 - 1].HasTile || !Main.tileSolid[Main.tile[k, num8 - 1].TileType] || Main.tileSolidTop[Main.tile[k, num8 - 1].TileType]) && (!Main.tile[k, num8 - 2].HasTile || !Main.tileSolid[Main.tile[k, num8 - 2].TileType] || Main.tileSolidTop[Main.tile[k, num8 - 2].TileType]) && (!Main.tile[k, num8 - 3].HasTile || !Main.tileSolid[Main.tile[k, num8 - 3].TileType] || Main.tileSolidTop[Main.tile[k, num8 - 3].TileType]); k++)
            {
                num7 = k;
            }
            for (int l = 0; l < 30; l++)
            {
                int num10 = Main.rand.Next(minValue, num7 + 1);
                if (l < 20)
                {
                    if (num10 < bestX - 1 || num10 > bestX + 1)
                    {
                        bestX = num10;
                        break;
                    }
                }
                else if (num10 != bestX)
                {
                    bestX = num10;
                    break;
                }
            }
            int num11 = bestX;
            int num12 = bestY;
            bool flag = false;
            if (!flag && !((double)num12 > Main.worldSurface))
            {
                //Rectangle value = default(Rectangle);
                for (int m = 20; m < 500; m++)
                {
                    for (int n = 0; n < 2; n++)
                    {
                        num11 = ((n != 0) ? (bestX - m * 2) : (bestX + m * 2));
                        if (num11 > 10 && num11 < Main.maxTilesX - 10)
                        {
                            int num13 = bestY - m;
                            double num2 = bestY + m;
                            if (num13 < 10)
                            {
                                num13 = 10;
                            }
                            if (num2 > Main.worldSurface)
                            {
                                num2 = Main.worldSurface;
                            }
                            for (int num3 = num13; (double)num3 < num2; num3++)
                            {
                                num12 = num3;
                                if (!Main.tile[num11, num12].HasUnactuatedTile || !Main.tileSolid[Main.tile[num11, num12].TileType])
                                {
                                    continue;
                                }
                                if (Main.tile[num11, num12 - 3].LiquidAmount != 0 || Main.tile[num11, num12 - 2].LiquidAmount != 0 || Main.tile[num11, num12 - 1].LiquidAmount != 0 || Collision.SolidTiles(num11 - 1, num11 + 1, num12 - 3, num12 - 1))
                                {
                                    break;
                                }
                                flag = true;
                                Rectangle value = new Rectangle(num11 * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, num12 * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                for (int num4 = 0; num4 < 255; num4++)
                                {
                                    if (Main.player[num4].active)
                                    {
                                        Rectangle val = new Rectangle((int)Main.player[num4].position.X, (int)Main.player[num4].position.Y, Main.player[num4].width, Main.player[num4].height);
                                        if (val.Intersects(value))
                                        {
                                            flag = false;
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            homeX = bestX;
            homeY = bestY;
            spawnX = num11 * 16;
            spawnY = num12 * 16;
        }

        //public static bool CheckSpwanTile(int tileX, int tileY)
        //{
        //    //EverquartzAdventureMod.Instance.Logger.Info($"({tileX},{tileY})");




        //    return EverquartzUtils.TileCapable(tileX, tileY) && !IsNpcOnscreen(new Point(tileX, tileY - 1).ToWorldCoordinates());
        //}

        //public static void FindSpawnPoint(Point pos, out int bestX, out int bestY)
        //{
        //    FindSpawnPoint(pos, pos, out bestX, out bestY);
        //}

        //private static void FindSpawnPoint(Point start, Point end, out int bestX, out int bestY, bool clockwise = true)
        //{
        //    int direction = clockwise.ToDirectionInt();
        //    //distance
        //    if (start.X == end.X && start.Y == end.Y)
        //    {
        //        FindSpawnPoint(new Point(start.X, start.Y - 1), new Point(start.X + direction, start.Y - 1), out bestX, out bestY);
        //        return;
        //    }

        //    if (start.X < end.X)
        //    {
        //        int level = end.X - start.X;
        //        if (level > Main.maxTilesX)
        //        {
        //            bestX = -1;
        //            bestY = -1;
        //            return;
        //        }
        //        for (int x = start.X; x < end.X; x++)
        //        {
        //            if (CheckSpwanTile(x, start.Y))
        //            {
        //                bestX = x;
        //                bestY = start.Y;
        //                return;
        //            }
        //        }
        //        FindSpawnPoint(new Point(end.X, end.Y + direction), new Point(end.X, end.Y + (level + 1) * direction), out bestX, out bestY);
        //        return;
        //    }
        //    else if (start.X > end.X)
        //    {
        //        int level = start.X - end.X;
        //        if (level > Main.maxTilesX)
        //        {
        //            bestX = -1;
        //            bestY = -1;
        //            return;
        //        }
        //        for (int x = start.X; x >= end.X; x--)
        //        {
        //            if (CheckSpwanTile(x, start.Y))
        //            {
        //                bestX = x;
        //                bestY = start.Y;
        //                return;
        //            }
        //        }
        //        FindSpawnPoint(new Point(end.X, end.Y - direction), new Point(end.X, end.Y - (level + 1) * direction), out bestX, out bestY);
        //        return;
        //    }
        //    else if (start.Y < end.Y)
        //    {
        //        int level = end.Y - start.Y;
        //        if (level > Main.maxTilesX)
        //        {
        //            bestX = -1;
        //            bestY = -1;
        //            return;
        //        }
        //        for (int y = start.Y; y < end.Y; y++)
        //        {
        //            if (CheckSpwanTile(start.X, y))
        //            {
        //                bestX = start.X;
        //                bestY = y;
        //                return;
        //            }
        //        }
        //        FindSpawnPoint(new Point(end.X - direction, end.Y), new Point(end.X - (level + 1) * direction, end.Y), out bestX, out bestY);
        //        return;
        //    }
        //    else if (start.Y > end.Y)
        //    {
        //        int level = start.Y - end.Y;
        //        if (level > Main.maxTilesX)
        //        {
        //            bestX = -1;
        //            bestY = -1;
        //            return;
        //        }
        //        for (int y = start.Y; y >= end.Y; y--)
        //        {
        //            if (CheckSpwanTile(start.X, y))
        //            {
        //                bestX = start.X;
        //                bestY = y;
        //                return;
        //            }
        //        }
        //        FindSpawnPoint(new Point(end.X, end.Y - 1), new Point(end.X + (level + 2) * direction, end.Y - 1), out bestX, out bestY);
        //        return;
        //    }
        //    else
        //    {
        //        bestX = -1;
        //        bestY = -1;
        //        return;
        //    }

        //}

        private static bool IsNpcOnscreen(Vector2 center)
        {
            int w = NPC.sWidth + NPC.safeRangeX * 2;
            int h = NPC.sHeight + NPC.safeRangeY * 2;
            Rectangle npcScreenRect = new Rectangle((int)center.X - w / 2, (int)center.Y - h / 2, w, h);
            foreach (Player player in Main.player)
            {
                // If any player is close enough to the traveling merchant, it will prevent the npc from despawning
                if (player != null && player.active && player.getRect().Intersects(npcScreenRect)) return true;
            }
            return false;
        }

        public static double GetRandomSpawnTime(double minTime, double maxTime)
        {
            // A simple formula to get a random time between two chosen times
            return (maxTime - minTime) * Main.rand.NextDouble() + minTime;
        }
        #endregion

        #region SaveLoad

        public static TagCompound Save()
        {
            return new TagCompound()
            {
                ["spawnTime"] = spawnTime,
                ["timePassed"] = timePassed,
                ["hypnoCoins"] = hypnoCoins
            };
        }

        public static void Load(TagCompound tag)
        {
            spawnTime = tag.GetDouble("spawnTime");
            timePassed = tag.GetDouble("timePassed");
            hypnoCoins = tag.GetInt("hypnoCoins");
        }
        #endregion

        #region MiscUtils
        private void Draw(Texture2D texture, SpriteBatch spriteBatch, Vector2 screenPos, Vector2? position = null, Color? drawColor = null)
        {
            spriteBatch.Draw
            (
                texture,
                position ?? (NPC.Center - screenPos - new Vector2(0, 8f)),
                new Rectangle(0, 0, texture.Width, texture.Height),
                drawColor ?? Color.White,
                NPC.rotation,
                NPC.frame.Size() / 2,
                NPC.scale,
                NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
            );
        }

        public void Kill()
        {
            NPC.life = 0;
            NPC.checkDead();
        }

        public void DropCoins()
        {
            Item.NewItem(NPC.GetSource_Death(), NPC.Center, ItemID.GoldCoin, hypnoCoins);
            hypnoCoins = 0;
        }

        public void KillWithCoins()
        {
            DropCoins();
            Kill();
        }




        #endregion

        #region Pray
        public static List<HypnosReward> GenerateRewards()
        {
            List<HypnosReward> rewards = new List<HypnosReward>();
            if (Main.rand.NextBool(3))
            {
                rewards.Add(HypnosReward.Coins);
            }
            if (Main.rand.NextBool(10) && ((ModCompatibility.calamityEnabled && CalamityWeakRef.downedExoMechs) || (ModCompatibility.hypnosEnabled && HypnosWeakRef.downedHypnos)))
            {
                rewards.Add(HypnosReward.ExoPrisms);
            }
            if (Main.rand.NextBool(50))
            {
                rewards.Add(HypnosReward.Eucharist);
            }
            if (Main.rand.NextBool(666))
            {
                rewards.Add(HypnosReward.Hypnotize);
            }
            if (Main.rand.NextBool(6666))
            {
                rewards.Add(HypnosReward.Euthanasia);
            }
            return rewards;
        }
        public string GetPrayChat(bool hasEnoughMoney)
        {

            WeightedRandom<string> textSelector = new WeightedRandom<string>(Main.rand);
            if (hasEnoughMoney)
            {
                EverquartzUtils.GetTextListFromKey(ChatPrayKey).ForEach(st => textSelector.Add(st));
            }
            else
            {
                EverquartzUtils.GetTextListFromKey(ChatPrayWithoutMoneyKey).ForEach(st => textSelector.Add(st));
            }
            string thingToSay = textSelector.Get();
            return thingToSay;
        }



        public void GetPrayInfo(Player player, out int targetDirection, out Vector2 playerPositionWhenPetting)
        {
            targetDirection = ((NPC.Center.X > player.Center.X) ? 1 : (-1));
            int num = 36;
            playerPositionWhenPetting = NPC.Bottom + new Vector2((float)(-targetDirection * num), 0f);
            playerPositionWhenPetting = playerPositionWhenPetting.Floor();
        }
        public void Pray(Player player)
        {
            if (player.Everquartz().IsPraisingHypnos || player.CCed)
            {
                return;
            }
            GetPrayInfo(player, out int targetDirection, out Vector2 playerPositionWhenPetting);
            Vector2 offset = playerPositionWhenPetting - player.Bottom;
            if (!player.CanSnapToPosition(offset) || !WorldGen.SolidTileAllowBottomSlope((int)playerPositionWhenPetting.X / 16, (int)playerPositionWhenPetting.Y / 16))
            {
                return;
            }

            bool buyResult = player.BuyItem(Item.buyPrice(gold: 1));
            if (buyResult)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    HandleHypnoCoinAddServer();
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)EverquartzMessageType.HypnoCoinAdd);
                    packet.Send();
                }
                player.StopVanityActions();
                player.RemoveAllGrapplingHooks();
                if (player.mount.Active)
                {
                    player.mount.Dismount(player);
                }
                player.Bottom = playerPositionWhenPetting;
                player.ChangeDir(targetDirection);
                player.Everquartz().praisingTimer = 100;
                player.velocity = Vector2.Zero;
                player.gravDir = 1f;
            }



            Main.npcChatText = GetPrayChat(buyResult);
        }
        #endregion

    }

    public class Indulgence : ModItem
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Indulgence");
            //DisplayName.AddTranslation(7, "赎罪券");
            //DisplayName.AddTranslation(6, "Снисхождение");

            //// Tooltip.SetDefault("You are atoned from your sins");
            //Tooltip.AddTranslation(7, "你已经免除了你的罪");
            //Tooltip.AddTranslation(6, "Вы искуплены от своих грехов");
        }
        public override void SetDefaults()
        {
            Item.width = 518;
            Item.height = 324;
            Item.value = 0;
            Item.rare = ItemRarityID.Gray;
            Item.maxStack = 9999;
        }
    }
}
