using System.Collections.Generic;
using Terraria.ModLoader;
using System.Linq;
using Terraria;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria.ID;
using EverquartzAdventure.NPCs.TownNPCs;
using EverquartzAdventure.NPCs;
using System;
using Terraria.Localization;
using EverquartzAdventure.Items.Critters;
using Terraria.ModLoader.IO;
using EverquartzAdventure.NPCs.Hypnos;
using System.Collections;
using EverquartzAdventure.ILEditing;
using EverquartzAdventure.UI.Transmogrification;

namespace EverquartzAdventure
{
    public class EverquartzAdventureMod : Mod
    {
        public static EverquartzAdventureMod Instance { get; private set; }




        public override void PostSetupContent()
        {
            //ModCompatibility.calamityEnabled = ModLoader.HasMod("CalamityMod");
            //ModLoader.TryGetMod("Census", out Mod censusMod);
            //if (censusMod != null)
            //{
            //    censusMod.Call("TownNPCCondition", ModContent.NPCType<StarbornPrincess>(), "Brutally murder her mom");
            //}
            TryDoCensusSupport();
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
                case EverquartzMessageType.ReleaseProvCore:
                    Player player = Main.player[reader.ReadInt32()];
                    DivineCore.ReleaseProvCoreServer(player);
                    break;
                case EverquartzMessageType.HypnosReward:
                    Player priest = Main.player[reader.ReadInt32()];
                    byte[] byteArray = reader.ReadBytes((EverquartzUtils.EnumCount<HypnosReward>() - 1) / 8 + 1);
                    BitArray bitArray = new BitArray(byteArray);
                    List<HypnosReward> rewards = new List<HypnosReward>();
                    for (int i = 0; i < bitArray.Length; i++)
                    {
                        if (bitArray.Get(i))
                        {
                            rewards.Add((HypnosReward)i);
                        }
                    }
                    NPCs.Hypnos.Hypnos.HandleRewardsServer(priest, rewards);
                    break;
                case EverquartzMessageType.HypnoCoinAdd:
                    NPCs.Hypnos.Hypnos.HandleHypnoCoinAddServer();
                    break;
                case EverquartzMessageType.HypnosDeparted:
                    NPC hypnos = NPCs.Hypnos.Hypnos.Instance;
                    if (hypnos != null)
                    {
                        NPCs.Hypnos.Hypnos.HandleDepartHypnosUniversal(hypnos);
                    }
                    break;
                    //case EverquartzMessageType.EverquartzSyncPlayer:
                    //    byte playernumber = reader.ReadByte();
                    //    EverquartzPlayer ePlayer = Main.player[playernumber].GetModPlayer<EverquartzPlayer>();
                    //    ePlayer.lastSleepingSpot = reader.ReadVector2().ToPoint();
                    //    break;
            }
        }

        public override void Load()
        {
            base.Load();
            Instance = this;

            ILChanges.Load();

            TransmogrificationManager.LoadAllTrans();

            ModCompatibility.censusMod = null;
            ModLoader.TryGetMod("Census", out ModCompatibility.censusMod);
            ModCompatibility.hypnosMod = null;
            ModLoader.TryGetMod("Hypnos", out ModCompatibility.hypnosMod);

            ModCompatibility.calamityEnabled = ModLoader.HasMod("CalamityMod");
            ModCompatibility.hypnosEnabled = ModLoader.HasMod("Hypnos");
            ModCompatibility.calRemixEnabled = ModLoader.HasMod("CalRemix");

            if (ModCompatibility.calamityEnabled)
            {
                CalamityILChanges.Load();
            }

            
        }



        public override void Unload()
        {
            base.Unload();

            if (ModCompatibility.calamityEnabled)
            {
                CalamityILChanges.Unload();
            }

            ModCompatibility.calamityEnabled = false;
            ModCompatibility.hypnosEnabled = false;
            ModCompatibility.calRemixEnabled = false;

            ModCompatibility.censusMod = null;
            ModCompatibility.hypnosMod = null;

            TransmogrificationManager.UnloadAllTrans();

            ILChanges.Unload();

            Instance = null;
            
        }

        public override object Call(params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return null;
            }
            if (!(args[0] is string argStr))
            {
                return null;
            }
            switch (argStr)
            {
                case "Transmogrification":
                case "AddTransmogrification":
                case "AddTrans":
                case "RegisterTransmogrification":
                case "RegisterTrans":
                    TransmogrificationManager.AddFromModCall(args.Skip(1));
                    return null;
                default:
                    return null;
            }
        }

        private void TryDoCensusSupport()
        {
            Mod censusMod = ModCompatibility.censusMod;
            if (censusMod != null)
            {
                censusMod.Call("TownNPCCondition", ModContent.NPCType<StarbornPrincess>(), Language.GetTextValue(StarbornPrincess.CensusConditionKey));
            }
        }

    }


    public class EverquartzSystem : ModSystem
    {


        public override void OnWorldLoad()
        {
            NPCs.Hypnos.Hypnos.hypnoCoins = 0;
            NPCs.Hypnos.Hypnos.timePassed = 0;
            NPCs.Hypnos.Hypnos.spawnTime = Double.MaxValue;
        }
        public override void LoadWorldData(TagCompound tag)
        {
            NPCs.Hypnos.Hypnos.Load(tag.GetCompound("hypnos"));
        }
        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("hypnos", NPCs.Hypnos.Hypnos.Save());
        }

        public override void PreUpdateWorld()
        {
            NPCs.Hypnos.Hypnos.UpdateTravelingMerchant();
        }

        public override void PreUpdateNPCs()
        {
            EverquartzGlobalNPC.UniqueNPCs.ForEach(AntiDupe);
        }



        public static void AntiDupe(int type)
        {
            IEnumerable<NPC> possiblyMultipleDeimi = Main.npc.Where(npc => npc != null && npc.active && npc.type == type);
            if (possiblyMultipleDeimi.Count() > 1)
            {
                possiblyMultipleDeimi.ToList().ForEach(npc => npc.netUpdate = true);
                possiblyMultipleDeimi.SkipLast(1).ToList().ForEach(npc => npc.active = false);
            }
        }
    }




    public enum EverquartzMessageType
    {
        DeimosItemKilled, // id, player.whoAmI, helptext
        ReleaseProvCore, // id, player.whoAmI
        HypnosReward, // id, player.whoAmI, rewards(bytes)
        HypnoCoinAdd, // id
        HypnosDeparted, // id
        //EverquartzSyncPlayer // id, player.whoAmI (see EverquartzPlayer.SyncPlayer)
    }

    public static class ModCompatibility
    {
        public static bool calamityEnabled = false;
        public static bool hypnosEnabled = false;
        public static bool calRemixEnabled = false;
        public static Mod censusMod;
        public static Mod hypnosMod;
        private static int? hypnosBossType = null;
        public static int? HypnosBossType
        {
            get
            {
                if (!hypnosBossType.HasValue)
                {
                    ModNPC hyNPC = null;
                    hypnosMod?.TryFind<ModNPC>("HypnosBoss", out hyNPC);
                    hypnosBossType = hyNPC?.Type;

                }
                return hypnosBossType;
            }
            set
            {
                hypnosBossType = value;
            }
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    internal static partial class CalamityWeakRef
    {

    }

    [JITWhenModsEnabled("Hypnos")]
    internal static partial class HypnosWeakRef
    {

    }

    [JITWhenModsEnabled("CalRemix")]
    internal static partial class CalRemixWeakRef
    {

    }

    
    
}