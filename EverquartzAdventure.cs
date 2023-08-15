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
            TransmogrificationManager.LoadAllTrans();
            //Logger.Info(TransmogrificationManager.Transmogrifications.Count());
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
            }
        }

        public override void Load()
        {
            base.Load();
            Instance = this;


            ModCompatibility.censusMod = null;
            ModLoader.TryGetMod("Census", out ModCompatibility.censusMod);
            ModCompatibility.hypnosMod = null;
            ModLoader.TryGetMod("HypnosMod", out ModCompatibility.hypnosMod);

            ModCompatibility.calamityEnabled = ModLoader.HasMod("CalamityMod");
            ModCompatibility.hypnosEnabled = ModLoader.HasMod("HypnosMod");
            ModCompatibility.calRemixEnabled = ModLoader.HasMod("CalRemix");

            ILChanges.Load();

            
            //if (ModCompatibility.calamityEnabled)
            //{
            //    CalamityILChanges.Load();
            //}


        }



        public override void Unload()
        {
            base.Unload();

            //if (ModCompatibility.calamityEnabled)
            //{
            //    CalamityILChanges.Unload();
            //}
            TransmogrificationManager.UnloadAllTrans();

            ILChanges.Unload();

            ModCompatibility.calamityEnabled = false;
            ModCompatibility.hypnosEnabled = false;
            ModCompatibility.calRemixEnabled = false;

            ModCompatibility.censusMod = null;
            ModCompatibility.hypnosMod = null;

            

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



        public static List<int> UniqueNPCs => new List<int>() {
            ModContent.NPCType<StarbornPrincess>(),
        };

        public override void PreUpdateNPCs()
        {
            UniqueNPCs.ForEach(AntiDupe);
        }



        public static void AntiDupe(int type)
        {
            IEnumerable<NPC> possiblyMultipleDeimi = Main.npc.Where(npc => npc != null && npc.active && npc.type == type);
            if (possiblyMultipleDeimi.Count() > 1)
            {
                possiblyMultipleDeimi.SkipLast(1).ToList().ForEach(npc => { npc.netUpdate = true; npc.active = false; }) ;
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