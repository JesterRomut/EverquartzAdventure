using System;
using System.Collections.Generic;
using System.Linq;
using EverquartzAdventure.Items;
using EverquartzAdventure.Items.Critters;
using EverquartzAdventure.Items.Pets;
using EverquartzAdventure.Items.Placeable.MusicBoxes;
using EverquartzAdventure.NPCs.Hypnos;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static EverquartzAdventure.CalamityWeakRef;

namespace EverquartzAdventure.UI.Transmogrification
{
    public struct TransmogrificationRecipe : TagSerializable
    {
        public int MainIngredientType { get; private set; } = -1;
        public int SecondaryMaterialType { get; private set; } = -1;
        public int SecondatyMaterialStack { get; private set; } = -1;
        public int ResultItemType { get; private set; } = -1;
        public int ResultItemStack { get; private set; } = -1;
        public int TimeInSeconds { get; private set; } = -1;
        public TransmogrificationRecipe(int mainIngredientType, int secondaryMaterialType, int secondaryMaterialStack, int resultItemType, int resultItemStack, int timeInSeconds)
        {
            MainIngredientType = mainIngredientType;
            SecondaryMaterialType = secondaryMaterialType;
            SecondatyMaterialStack = secondaryMaterialStack;
            ResultItemType = resultItemType;
            ResultItemStack = resultItemStack;
            TimeInSeconds = timeInSeconds;
        }

        public static TransmogrificationRecipe Invalid => new TransmogrificationRecipe(-1, -1, -1, -1, -1, -1);

        public bool HasEnoughSecondaryMaterial(Player player)
        {
            return player.ItemCount(SecondaryMaterialType) >= SecondatyMaterialStack;
        }

        public TagCompound SerializeData()
        {
            return new TagCompound
            {
                ["mainIngredientType"] = MainIngredientType,
                ["secondaryMaterialType"] = SecondaryMaterialType,
                ["secondaryMaterialStack"] = SecondatyMaterialStack,
                ["resultItemType"] = ResultItemType,
                ["resultItemStack"] = ResultItemStack,
                ["timeInSeconds"] = TimeInSeconds,
            };
        }

        public static readonly Func<TagCompound, TransmogrificationRecipe> DESERIALIZER = Load;
        public static TransmogrificationRecipe Load(TagCompound tag)
        {
            TransmogrificationRecipe recipe = new TransmogrificationRecipe(
                tag.GetInt("mainIngredientType"),
                tag.GetInt("secondaryMaterialType"),
                tag.GetInt("secondaryMaterialStack"),
                tag.GetInt("resultItemType"),
                tag.GetInt("resultItemStack"),
                tag.GetInt("timeInSeconds")
                );
            return recipe;
        }

        public bool IsValid => MainIngredientType != -1 && SecondaryMaterialType != -1 && SecondatyMaterialStack != -1 && ResultItemType != -1 && ResultItemStack != -1 && TimeInSeconds != -1;

        public override bool Equals(Object obj)
        {
            if (!(obj is TransmogrificationRecipe)) return false;

            TransmogrificationRecipe p = (TransmogrificationRecipe)obj;
            return MainIngredientType == p.MainIngredientType && SecondaryMaterialType == p.SecondaryMaterialType && SecondatyMaterialStack == p.SecondatyMaterialStack && ResultItemType == p.ResultItemType && ResultItemStack == p.ResultItemStack && TimeInSeconds == p.TimeInSeconds;
        }
        public override int GetHashCode()
        {
            return MainIngredientType.GetHashCode() + SecondaryMaterialType.GetHashCode() + SecondatyMaterialStack.GetHashCode() + ResultItemType.GetHashCode() + ResultItemStack.GetHashCode() + TimeInSeconds.GetHashCode();
        }
    }
    public static class TransmogrificationManager
    {
        public static List<TransmogrificationRecipe> Transmogrifications { get; internal set; }
        //TODO mod.call
        public static bool CanTrans(int type) => Transmogrifications.Any(trans => trans.MainIngredientType == type);
        public static List<TransmogrificationRecipe> FindAllTransByMainIngredient(int type) => Transmogrifications.Where(trans => trans.MainIngredientType == type).ToList();
        public static List<TransmogrificationRecipe> FindAvaliableTransByMainIngredient(int type, Player player) => Transmogrifications.Where(trans => trans.MainIngredientType == type && player.ItemCount(trans.SecondaryMaterialType) >= trans.SecondatyMaterialStack).ToList();
        public static List<TransmogrificationRecipe> FindUnavaliableTransByMainIngredient(int type, Player player) => Transmogrifications.Where(trans => trans.MainIngredientType == type && player.ItemCount(trans.SecondaryMaterialType) < trans.SecondatyMaterialStack).ToList();
        public static int MaxTransmogrificationAmount(Player player, TransmogrificationRecipe recipe)
        {
            if (recipe.SecondatyMaterialStack == 0)
            {
                return 0;
            }
            return player.ItemCount(recipe.SecondaryMaterialType) / recipe.SecondatyMaterialStack;

        }
        
        public static void AddFromModCall(IEnumerable<object> parameters)
        {
            // THIS IS HELL
            const string correctUsage = "Correct usage: int mainIngredientType, int secondaryMaterialType, int secondaryMaterialStack, int resultItemType, int resultItemStack, int timeInSeconds";
            if (parameters.Count() != 6)
            {
                throw new ArgumentNullException(correctUsage);
            }
            if (parameters.ElementAt(0) is int mainIngredientType)
            {
                if (parameters.ElementAt(1) is int secondaryMaterialType)
                {
                    if (parameters.ElementAt(2) is int secondaryMaterialStack)
                    {
                        if (parameters.ElementAt(3) is int resultItemType)
                        {
                            if (parameters.ElementAt(4) is int resultItemStack)
                            {
                                if (parameters.ElementAt(5) is int timeInSeconds)
                                {
                                    Transmogrifications.Add(new TransmogrificationRecipe(mainIngredientType, secondaryMaterialType, secondaryMaterialStack, resultItemType, resultItemStack, timeInSeconds));
                                }
                                else
                                {
                                    throw new ArgumentNullException(correctUsage);
                                }
                            }
                            else
                            {
                                throw new ArgumentNullException(correctUsage);
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException(correctUsage);
                        }
                    }
                    else
                    {
                        throw new ArgumentNullException(correctUsage);
                    }
                }
                else
                {
                    throw new ArgumentNullException(correctUsage);
                }
            }
            else
            {
                throw new ArgumentNullException(correctUsage);
            }
        }

        internal static void LoadAllTrans()
        {
            Transmogrifications = new List<TransmogrificationRecipe>
            {
                new TransmogrificationRecipe(ItemID.MusicBox, ModContent.ItemType<DeimosFumo>(), 10, ModContent.ItemType<MelanieMartinezMusicBox>(), 1, 300)
                //new TransmogrificationRecipe(ModContent.ItemType<DeimosFumo>(), ItemID.FallenStar, 10, ItemID.StarWrath, 1, 0),
                //new TransmogrificationRecipe(ModContent.ItemType<DeimosFumo>(), ModContent.ItemType<Indulgence>(), 10, ModContent.ItemType<DeimosPetItem>(), 2, 10),
                //new TransmogrificationRecipe(ModContent.ItemType<DeimosFumo>(), ModContent.ItemType<Indulgence>(), 10, ModContent.ItemType<StarbornPrincessItem>(), 3, 20),
                //new TransmogrificationRecipe(ModContent.ItemType<DeimosFumo>(), ModContent.ItemType<Indulgence>(), 10, ItemID.Zenith, 1, 0)
            };
            if (ModCompatibility.calamityEnabled)
            {
                Transmogrifications.Add(new TransmogrificationRecipe(ItemType.AstralBlaster, ItemType.SeaPrism, 10, ItemType.ClockGatlignum, 1, 20));
                Transmogrifications.Add(new TransmogrificationRecipe(ItemType.Oracle, ItemID.FallenStar, 40, ItemType.TheMicrowave, 1, 30));
                Transmogrifications.Add(new TransmogrificationRecipe(ItemType.PoleWarper, ItemType.EndothermicEnergy, 5, ItemID.NorthPole, 1, 60));
                Transmogrifications.Add(new TransmogrificationRecipe(ItemType.RottingEyeball, ItemType.RottenMatter, 22, ItemType.BloodyVein, 1, 1800));
                Transmogrifications.Add(new TransmogrificationRecipe(ItemType.LeviathanAmbergris, ItemType.Nucleogenesis, 1, ItemID.Shrimp, 1, 20));
                Transmogrifications.Add(new TransmogrificationRecipe(ItemType.CryoStone, ItemType.EssenceofSunlight, 1, ItemID.WaterBucket, 1, 0));
                Transmogrifications.Add(new TransmogrificationRecipe(ItemType.ShardofAntumbra, ItemID.SoulofLight, 5, ItemType.LightGodsBrilliance, 1, 60));
                if (ModCompatibility.calRemixEnabled)
                {
                    Transmogrifications.Add(new TransmogrificationRecipe(CalRemixWeakRef.ItemType.YharimBar, ItemType.UnholyEssence, 20, ItemType.DivineGeode, 20, 60));
                    Transmogrifications.Add(new TransmogrificationRecipe(CalRemixWeakRef.ItemType.YharimBar, ItemType.Phantoplasm, 10, ItemType.RuinousSoul, 10, 180));
                    Transmogrifications.Add(new TransmogrificationRecipe(CalRemixWeakRef.ItemType.YharimBar, ItemType.EndothermicEnergy, 20, ItemType.NightmareFuel, 20, 60));
                    Transmogrifications.Add(new TransmogrificationRecipe(CalRemixWeakRef.ItemType.YharimBar, ItemType.NightmareFuel, 20, ItemType.EndothermicEnergy, 21, 60));
                    Transmogrifications.Add(new TransmogrificationRecipe(CalRemixWeakRef.ItemType.YharimBar, ItemID.SoulofNight, 20, ItemType.DarksunFragment, 20, 60));
                    Transmogrifications.Add(new TransmogrificationRecipe(CalRemixWeakRef.ItemType.YharimBar, ItemType.AuricOre, 5, ItemType.YharonSoulFragment, 5, 300));
                    Transmogrifications.Add(new TransmogrificationRecipe(CalRemixWeakRef.ItemType.YharimBar, ItemType.AshesofAnnihilation, 5, ItemType.ExoPrism, 5, 600));
                    Transmogrifications.Add(new TransmogrificationRecipe(CalRemixWeakRef.ItemType.YharimBar, ItemType.ExoPrism, 5, ItemType.AshesofAnnihilation, 5, 600));
                }
            }

        }
        internal static void UnloadAllTrans()
        {
            Transmogrifications = null;
        }
    }
}
