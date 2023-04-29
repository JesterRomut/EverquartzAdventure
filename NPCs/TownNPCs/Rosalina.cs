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
using System.Linq;
using CalamityMod.NPCs.Providence;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace EverquartzAdventure
{
    internal static partial class CalamityWeakRef
    {
        internal static bool IsProvDefeated() => DownedBossSystem.downedProvidence;
        
        internal static bool IsDoGDefeated() => DownedBossSystem.downedDoG;
        
        internal static bool IsAnyCalamitas() => NPC.AnyNPCs(ModContent.NPCType<WITCH>());
        
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
        public override void SetStaticDefaults()
        {
            base.DisplayName.SetDefault("Starborn Princess");
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
            NPCID.Sets.BossBestiaryPriority.Add(base.Type);
            Main.npcCatchable[base.NPC.type] = true;
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
            base.NPC.HitSound = SoundID.FemaleHit;
            base.NPC.DeathSound = SoundID.NPCDeath6;
            base.NPC.knockBackResist = 0.5f;
            NPC.catchItem = ModContent.ItemType<StarbornPrincessItem>();
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
            
            new FlavorTextBestiaryInfoElement("WE MUST CONSUME HYPNOS FLESH")
            });
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {

            if(ModCompatibility.calamityEnabled && CalamityWeakRef.IsProvDefeated())
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
            Func<NPC, bool> pred = (npc => npc.type == ModContent.NPCType<StarbornPrincess>() && npc.whoAmI != NPC.whoAmI);
            if (Main.npc.Any(pred))
            {
                Main.npc.Where(pred).ToList().ForEach(npc => npc.active = false);
            }
                
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
        }

        public static void ModifyLoot(ILoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<EverquartzItem>()));
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
                textSelector.Add("I've always been homeless, so it's sort of the norm for me.");
                textSelector.Add("Do you have a house? I'd like to see it!");
                textSelector.Add("Not to ask for too much but... can I have a house? Is that too much to ask? Sorry...");
            }
            else
            {
                textSelector.Add("Did you hear about that one artist? I think they are called something like Everquartz... well- I think they're weird personally.");
                textSelector.Add("I really like reading books and I like studying on how to warp myself to different timelines. It's an interesting sight to see, seeing yourself from the past or seeing yourself in another reality. I've seen many realities, all come in different shapes and sizes. All have different orders and different ways of running their socities. It's sooooo interesting!");
                textSelector.Add("I hate both of my parents to be honest, they're both as bad as each other... sort of. I can't remember.");
                textSelector.Add("What is your opinion on chicken nuggets?");
                textSelector.Add("*starts doing the cha cha slide* Cha cha real smooth!");
                textSelector.Add("When I was 'born', I was just a mere essence existing that relied on both my mother and my father. I don't know how I was created... but if you haven't already figured that out, my parents are The Profaned Goddess and that stupid god eating creature. He's stupid, we're all stupid. I think that concludes this conversation.");
                if (!Main.dayTime)
                {
                    if (Main.bloodMoon)
                    {
                        textSelector.Add("Mortals and sinners like YOU should not be allowed to roam the planet in such a mess. If my mother was around, I would have personally took you to her to get a cleansing. Being cleansed isn't the most pretty sight, so be LUCKY that she is dead... I'm not too happy about that.", 5.15);
                        textSelector.Add("If the world was cleansed, my mother would be happy. You just so happened to ruin it... I do not have pity for you, sinner.", 5.15);
                        textSelector.Add("BURN, BURN, BURN! Cleanse the world of its otherworldly sins!", 5.15);
                        textSelector.Add("I promised my mother that I would continue on my legacy as a such a divine essence, and that includes to try cleanse this world of its disgusting sins. I probably keep going on about it too much, but it's true. I must continue on to cleanse the world. My mother would be so happy to hear that I would try to continue my legacy on but guess who killed her?! See what I mean?!", 5.15);
                        textSelector.Add("See the horns on my head? They're from goddesses. GODDESSES.", 5.15);
                    }
                }
                if (BirthdayParty.PartyIsUp)
                {
                    textSelector.Add("Wow! Parties are so much fun. I love hopping around and dancing and all that confetti is so fun! It's so bright and colorful! I love it!", 5.5);
                    textSelector.Add("My mother would always disapprove of parties... so having to experience one in person now is a little odd for me, but I'm loving it!", 5.5);
                }
                if (ModCompatibility.calamityEnabled)
                {
                    if (CalamityWeakRef.IsDoGDefeated())
                    {
                        textSelector.Add("My mother was an interesting character, but I just never felt with her... the idea of burning the world just never stood right with me, but I never really verbally spoke about it because I was afraid my mother would KILL me for it. I wonder what it would've been like if my dad took me instead. I feel like he would've used me as a sentinel or something. Yikes.", (0.8));
                        textSelector.Add("Considering how weak you originally were, I do wonder if your growth will ever reach a limit. I'm curious!", (0.8));
                        textSelector.Add("My father? Oh, well he wasn't the best character around... I hadn't really known him for that long considering I always stayed with my mother, but I heard he was just... bad. I don't know how else to describe it, but I don't like him. I would've preferred to stay with my mother, but considering she is dead- I am not too sure who to stay with. I guess that's why I stayed with you, because you have shelter... I'm alone now. I don't have anyone to rely on anymore.", (0.8));
                        textSelector.Add("It's a bit weird to say this, but thank you for defeating both of my parents. The world is much more great now without those two around. Keeping those two apart was hard work. Let's just say it would've been a universal catastrophe if this was in my earlier stages of my existence. I am glad I don't have to deal with the stress about keeping up my dad's Delicious Meat addiction. Though, I'm a bit sad that I don't have anyone to rely on now.", (0.8));
                    }
                    if (CalamityWeakRef.IsAnyCalamitas())
                    {
                        textSelector.Add("That girl, Calamitas I think her name is... she looks very pretty. I'll respect her privacy, though. ");
                        textSelector.Add("Could you please ask Calamitas something for me? Thank you!");
                    }
                }
                if (Main.player[Main.myPlayer].ZoneHallow)
                {
                    textSelector.Add("I think the Hallow is quite a deceiving place. It's quite unique, actually. I really do like it, but man those unicorns are lethal. Have you not seen them?!");
                    textSelector.Add("You would think that a biome full of fairies and unicorns would be a little kinder sometimes...");
                }
                int angler = NPC.FindFirstNPC(NPCID.Angler);
                if (angler != -1)
                {
                    textSelector.Add($"Did you hear about {Main.npc[angler].GivenName}? Yeah, that short kid with the weird hat and stuff? Well I don't like him.");
                }
            }
            string thingToSay = textSelector.Get();
            if (Main.rand.NextBool(4444))
            {
                thingToSay = "WE MUST CONSUME HYPNOS FLESH.";
            }
            return thingToSay;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                DeathEffectClient(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height);
            }
        }

        public static void DeathEffectClient(Vector2 position, int width, int height)
        {
            for (int num585 = 0; num585 < 25; num585++)
            {
                int num586 = Dust.NewDust(position,width, height, 31, 0f, 0f, 100, default(Color), 2f);
                Dust dust30 = Main.dust[num586];
                Dust dust187 = dust30;
                dust187.velocity *= 1.4f;
                Main.dust[num586].noLight = true;
                Main.dust[num586].noGravity = true;
            }
        }

        public override void OnKill()
        {
            if (ModCompatibility.calamityEnabled)
            {
                CalamityWeakRef.SummonProv(Main.player.Random());
            }
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Deimo's prime shop";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            shop = true;
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<EverquartzItem>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 3);
            nextSlot++;
        }
    }
    }