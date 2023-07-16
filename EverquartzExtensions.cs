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
using Terraria.Audio;
using Terraria.Localization;
using EverquartzAdventure.Items.Critters;
using Terraria.ModLoader.IO;
using EverquartzAdventure.NPCs.Hypnos;
using System.Collections;
using EverquartzAdventure.ILEditing;
using EverquartzAdventure.UI.Transmogrification;
using EverquartzAdventure.Items;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace EverquartzAdventure
{
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

        internal static bool HasAnyBuff(this NPC npc, List<int> debuffs)
        {
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
            for (int l = 0; l < 58; l++)
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

        internal static ILCursor GotoFinalRet(this ILCursor cursor)
        {
            while (cursor.TryGotoNext((Instruction c) => c.MatchRet()))
            {
            }

            return cursor;
        }

        //internal static ILCursor GotoLast(this ILCursor cursor, MoveType moveType = MoveType.Before, params Func<Instruction, bool>[] predicates)
        //{
        //    while (true)
        //    {
        //        ILCursor lastCursor = cursor;
        //        if (!cursor.TryGotoNext(moveType, predicates))
        //        {
        //            return lastCursor;
        //        }
        //    }

        //}

        public static string ToISO8601(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        }
        public static DateTime ToISO8601(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return default(DateTime);
            }
            return DateTime.ParseExact(str, "yyyyMMddHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        }

        internal static EverquartzItem ModItem(this Item item)
        {
            try
            {
                return (EverquartzItem)item.ModItem;
            }
            catch (InvalidCastException)
            {
                return null;
            }


        }

        internal static EverquartzGlobalNPC Everquartz(this NPC npc)
        {
            if (npc.TryGetGlobalNPC(out EverquartzGlobalNPC globalNPC))
            {
                return globalNPC;
            }
            return null;
        }

        internal static EverquartzNPC ModNPC(this NPC npc)
        {
            if (npc.ModNPC is EverquartzNPC enpc) {
                return enpc;
            }
            return null;
        }

        internal static EverquartzPlayer Everquartz(this Player player)
        {
            if (player.TryGetModPlayer(out EverquartzPlayer modPlayer))
            {
                return modPlayer;
            }
            return null;

        }
    }

}