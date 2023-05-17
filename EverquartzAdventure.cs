using System.Collections.Generic;
using Terraria.ModLoader;
using System.Linq;
using Terraria;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using EverquartzAdventure.NPCs.TownNPCs;
using EverquartzAdventure.NPCs;
using System;
using Terraria.Audio;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI.Chat;
using Terraria.UI;
using EverquartzAdventure.Items.Critters;
using static Terraria.Player;
using Terraria.ModLoader.IO;
using EverquartzAdventure.NPCs.Hypnos;
using EverquartzAdventure.Projectiles.Hypnos;
using Terraria.Utilities;
using System.Collections;
using Terraria.GameContent;
using EverquartzAdventure.Buffs;
using EverquartzAdventure.Buffs.Hypnos;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using static Terraria.ModLoader.PlayerDrawLayer;
using ReLogic.Content;
using EverquartzAdventure.UI.Transmogrification;
using CalamityMod.UI.CalamitasEnchants;

namespace EverquartzAdventure
{
    public class EverquartzAdventureMod : Mod
    {
        public static EverquartzAdventureMod instance;

        


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
                    for(int i = 0; i < bitArray.Length; i++)
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
            ModCompatibility.censusMod = null;
            ModLoader.TryGetMod("Census", out ModCompatibility.censusMod);
            ModCompatibility.hypnosMod = null;
            ModLoader.TryGetMod("Hypnos", out ModCompatibility.hypnosMod);

            ModCompatibility.calamityEnabled = ModLoader.HasMod("CalamityMod");
            ModCompatibility.hypnosEnabled = ModLoader.HasMod("Hypnos");
            ModCompatibility.calRemixEnabled = ModLoader.HasMod("CalRemix");

            TransmogrificationManager.LoadAllTrans();

            instance = this;
            base.Load();
        }

        

        public override void Unload()
        {
            ModCompatibility.censusMod = null;
            ModCompatibility.hypnosMod = null;

            ModCompatibility.calamityEnabled = false;
            ModCompatibility.hypnosEnabled = false;
            ModCompatibility.calRemixEnabled = false;

            TransmogrificationManager.UnloadAllTrans();

            instance = null;
            base.Unload();
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

    internal static class EverquartzExtensions
    {
        internal static T Random<T>(this IEnumerable<T> li)
        {
            return li.ElementAtOrDefault(Main.rand.Next(li.Count()));
        }

        internal static void AddShopItem(this Chest shop, ref int nextSlot, int item, int price = -1)
        {
            shop.item[nextSlot].SetDefaults(item);
            if (price != -1)
            {
                shop.item[nextSlot].shopCustomPrice = price;
            }
            nextSlot++;
        }

        internal static Vector2 NearestPos(this Vector2 here, Vector2 dest, float radius)
        {
            double angle = Math.Atan2(here.Y - dest.Y, here.X - dest.X);
            return new Vector2((float)(dest.X + radius * Math.Cos(angle)), (float)(dest.Y + radius * Math.Sin(angle)));
        }

        internal static Vector2 NearestPos(this Vector2 here, double angle, float radius)
        {
            return new Vector2((float)(here.X + radius * Math.Cos(angle)), (float)(here.Y + radius * Math.Sin(angle)));
        }

        internal static bool IsAltFunctionUse(this Player player)
        {
            return player.altFunctionUse == 2;
        }
        public static void SetFoodDefault(this Item item, int buffType = BuffID.WellFed3, int buffTime = 72000, SoundStyle? useSound = null)
        {
            item.consumable = true;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatFood;
            item.useTurn = true;
            item.buffType = buffType;
            item.buffTime = buffTime;
            item.UseSound = useSound ?? SoundID.Item2;
        }

        public static Vector2 SafeDirectionTo(this Entity entity, Vector2 destination, Vector2? fallback = null)
        {
            return entity.Center.SafeDirectionTo(destination, fallback);
        }

        public static Vector2 SafeDirectionTo(this Vector2 entityCenter, Vector2 destination, Vector2? fallback = null)
        {
            if (!fallback.HasValue)
            {
                fallback = Vector2.Zero;
            }
            return (destination - entityCenter).SafeNormalize(fallback.Value);
        }

        public static NPC NearestNPC(this Vector2 position, float maxDistance, Func<NPC, bool> predicate = null)
        {
            NPC target = null;
            float distance = maxDistance;
            bool checkNPCInSight(NPC npc) => npc != null && npc.active && Vector2.Distance(position, npc.Center) < distance + ((float)(npc.width / 2) + (float)(npc.height / 2));
            foreach (NPC npc in Main.npc)
            {
                if (checkNPCInSight(npc) && (predicate == null ? predicate(npc) : true))
                {
                    distance = Vector2.Distance(position, npc.Center);
                    target = npc;
                }
            }
            return target;
        }

        public static string LocalizedDuration(this TimeSpan time, bool abbreviated, bool showAllAvailableUnits)
        {
            string text = "";
            abbreviated |= !GameCulture.FromCultureName(GameCulture.CultureName.English).IsActive;
            if (time.Days > 0)
            {
                int num = time.Days;
                //if (!showAllAvailableUnits && time > TimeSpan.FromDays(1.0))
                //{
                //    num++;
                //}

                text = text + num + (abbreviated ? (" " + Language.GetTextValue("Misc.ShortDays")) : ((num == 1) ? " day" : " days"));
                if (!showAllAvailableUnits)
                {
                    return text;
                }

                text += " ";
            }

            if (time.Hours > 0)
            {
                int num2 = time.Hours;
                //if (!showAllAvailableUnits && time > TimeSpan.FromHours(1.0))
                //{
                //    num2++;
                //}

                text = text + num2 + (abbreviated ? (" " + Language.GetTextValue("Misc.ShortHours")) : ((num2 == 1) ? " hour" : " hours"));
                if (!showAllAvailableUnits)
                {
                    return text;
                }

                text += " ";
            }

            if (time.Minutes > 0)
            {
                int num3 = time.Minutes;
                //if (!showAllAvailableUnits && time > TimeSpan.FromMinutes(1.0))
                //{
                //    num3++;
                //}

                text = text + num3 + (abbreviated ? (" " + Language.GetTextValue("Misc.ShortMinutes")) : ((num3 == 1) ? " minute" : " minutes"));
                if (!showAllAvailableUnits)
                {
                    return text;
                }

                text += " ";
            }
            if (time.Seconds > 0)
            {
                int num4 = time.Seconds;
                //if (!showAllAvailableUnits && time >= TimeSpan.FromSeconds(1.0))
                //{
                //    num4++;
                //}
                text = text + num4 + (abbreviated ? (" " + Language.GetTextValue("Misc.ShortSeconds")) : ((time.Seconds == 1) ? " second" : " seconds"));
            }

            return text;
        }

        internal static bool HasAnyBuff(this NPC npc, List<int> debuffs) {
            return debuffs.Where(npc.HasBuff).Any();
        }

        internal static bool HasAllBuffs(this NPC npc, List<int> debuffs)
        {
            return debuffs.Where(npc.HasBuff).Count() == debuffs.Count();
        }

        internal static bool Solid(this Tile tile) => tile.HasUnactuatedTile && Main.tileSolid[tile.TileType];
        internal static bool HasLiquid(this Tile tile) => tile.LiquidAmount > 0;

        public static NPC NearestEnemyPreferNoDebuff(this Vector2 position, float maxDistance, List<int> debuffs)
        {
            
            NPC target = null;
            float distance = maxDistance;

            bool checkNPCInSight(NPC npc) => position.CheckNPCInSight(npc, distance);

            bool shouldAttackAllDebuffed = !Main.npc.Where(npc => checkNPCInSight(npc) && !npc.HasAllBuffs(debuffs)).Any();
            bool shouldAttackAnyDebuffed = !Main.npc.Where(npc => checkNPCInSight(npc) && !npc.HasAnyBuff(debuffs)).Any();
            foreach (NPC npc in Main.npc)
            {

                if (checkNPCInSight(npc))
                {
                    bool allDebuffed = npc.HasAllBuffs(debuffs);
                    bool anyDebuffed = npc.HasAnyBuff(debuffs);
                    if (anyDebuffed == shouldAttackAnyDebuffed && allDebuffed == shouldAttackAllDebuffed)
                    {
                        distance = Vector2.Distance(position, npc.Center);
                        target = npc;
                    }
                    
                }
            }
            return target;
        }

        public static bool CheckNPCInSight(this Vector2 position, NPC npc, float distance) => npc != null && npc.active && npc.CanBeChasedBy() && npc.chaseable && Vector2.Distance(position, npc.Center) < distance + ((float)(npc.width / 2) + (float)(npc.height / 2));
        public static NPC NearestEnemyPreferNoMindcrashed(this Vector2 position, float maxDistance)
        {

            NPC target = null;
            float distance = maxDistance;

            bool checkNPCInSight(NPC npc) => position.CheckNPCInSight(npc, distance);

            bool shouldAttackDebuffed = !Main.npc.Where(npc => checkNPCInSight(npc) && !(npc.Everquartz().mindcrashed > 0)).Any();
            foreach (NPC npc in Main.npc)
            {

                if (checkNPCInSight(npc))
                {
                    bool debuffed = npc.Everquartz().mindcrashed > 0;
                    if (debuffed == shouldAttackDebuffed)
                    {
                        distance = Vector2.Distance(position, npc.Center);
                        target = npc;
                    }

                }
            }
            //if (target != null && target.active)
            //{
            //    CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, 1, 1), Color.White, $"{target.FullName} {target.chaseable} {target.CanBeChasedBy()} {target.active}");
            //}
            return target;
        }

        
        public static byte[] ToByteArray(this BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

        internal static int ItemCount(this Player player, int type)
        {
            int count = 0;
            for(int l = 0; l < 58; l++)
            {
                Item item = player.inventory[l];
                if (item.stack > 0 && item.type == type)
                {
                    count += item.stack;
                }
            }
            return count;
        }

        internal static void TakeItem(this Player player, int type, int amount)
        {
            int remaining = amount;
            for (int l = 0; l < 58 && remaining > 0; l++)
            {
                Item item = player.inventory[l];
                if (item.stack > 0 && item.type == type)
                {
                    if (item.stack <= remaining)
                    {
                        item.TurnToAir();
                        remaining -= item.stack;
                    }
                    else
                    {
                        item.stack -= remaining;
                        remaining = 0;
                    }
                }
            }
        }

        public static string ToISO8601(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
        }
        public static DateTime ToISO8601(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return default(DateTime);
            }
            return DateTime.ParseExact(str, "yyyyMMddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
        }

        public static EverquartzGlobalNPC Everquartz(this NPC npc) => npc.GetGlobalNPC<EverquartzGlobalNPC>();

        internal static EverquartzPlayer Everquartz(this Player player) => player.GetModPlayer<EverquartzPlayer>();
    }

    internal static class EverquartzUtils
    {
        internal static bool ActiveTiles(int startX, int endX, int startY, int endY)
        {
            for (int i = startX; i < endX + 1; i++)
            {
                for (int j = startY; j < endY + 1; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        return false;
                    }
                    if (!Main.tile[i, j].HasUnactuatedTile || !Main.tileSolid[Main.tile[i, j].TileType])
                    {
                        Dust.NewDustPerfect(new Point(i, j).ToWorldCoordinates(), DustID.Water, Vector2.Zero);
                        return false;
                    }
                }
            }
            return true;
        }

        internal static bool TileCapable(int tileX, int tileY)
        {
            if (tileX < 0 || tileY < 0 || tileX >= Main.maxTilesX || tileY >= Main.maxTilesY)
            {
                //EverquartzAdventureMod.Instance.Logger.Info("not pass: tileX < 0 || tileY < 0 || tileX > Main.maxTilesX || tileY > Main.maxTilesY");
                return false;
            }

            //Dust.NewDustPerfect(new Vector2(tileX * 16, tileY * 16), DustID.Water, Vector2.Zero);

            if (!ActiveTiles(tileX, tileX + 1, tileY + 3, tileY + 3))//!Main.tile[tileX, tileY + 4].Solid() && !Main.tile[tileX + 1, tileY + 4].Solid())
            {
                //EverquartzAdventureMod.Instance.Logger.Info("not pass: !Collision.SolidTiles(tileX, tileX + 1, tileY + 4, tileY + 4)");
                return false;
            }
            if (Main.tile[tileX, tileY].HasLiquid() || Main.tile[tileX, tileY + 1].HasLiquid() || Main.tile[tileX, tileY + 2].HasLiquid() || ActiveTiles(tileX, tileX + 1, tileY, tileY + 2))
            {
                //EverquartzAdventureMod.Instance.Logger.Info("not pass: hasliquid or");
                return false;
            }
            return true;
        }

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

        internal static int EnumCount<T> () where T: Enum
        {
            return Enum.GetNames(typeof(T)).Length;
        }

        public static Color ColorSwap(Color firstColor, Color secondColor, float seconds)
        {
            float colorMePurple = (float)((Math.Sin((double)((float)Math.PI * 2f / seconds) * (double)Main.GlobalTimeWrappedHourly) + 1.0) * 0.5);
            return Color.Lerp(firstColor, secondColor, colorMePurple);
        }

        public static Color ColorSwap(Color firstColor, Color secondColor, Color thirdColor, float seconds)
        { // 0 < colormepurple < 1
            float colorMePurple = (float)((Math.Sin((double)((float)Math.PI * 2f / seconds) * (double)Main.GlobalTimeWrappedHourly) + 1.0) * 0.5);
            //ModContent.GetInstance<EverquartzAdventureMod>().Logger.Info(colorMePurple);
            if (colorMePurple < 0.33f)
            {
                return Color.Lerp(firstColor, secondColor, colorMePurple * 3);
            }
            else if (colorMePurple < 0.66f)
            {
                return Color.Lerp(secondColor, thirdColor, (colorMePurple - 0.33f) * 3);
            }
            else
            {
                return Color.Lerp(thirdColor, firstColor, (colorMePurple - 0.66f) * 3);
            }

        }

        public static Color ColorSwap(List<Color> colors, float seconds)
        {
            float colorMePurple = (float)((Math.Sin((double)((float)Math.PI * 2f / seconds) * (double)Main.GlobalTimeWrappedHourly) + 1.0) * 0.5);
            //ModContent.GetInstance<EverquartzAdventureMod>().Logger.Info(colorMePurple);
            int count = colors.Count();

            for(int i = 1; i <= count; i++)
            {
                double level = (double)i / (double)count;
                //EverquartzAdventureMod.Instance.Logger.Info($"{i} {count} {level} {(double)(i - 1) / count}");
                if (colorMePurple < level)
                {
                    double lerpAmount = (double)(i - 1) / count;
                    if (i == count)
                    {
                        return Color.Lerp(colors[i - 1], colors[0], (float)(((double)colorMePurple - lerpAmount) * count));
                    }
                    return Color.Lerp(colors[i - 1], colors[i], (float)(((double)colorMePurple - lerpAmount) * count));
                }
            }
            return Color.White;
        }
    }
}