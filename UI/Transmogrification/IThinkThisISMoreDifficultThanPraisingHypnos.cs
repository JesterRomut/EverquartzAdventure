using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.GameContent;
using ReLogic.Content;
using Terraria.Audio;
using EverquartzAdventure.NPCs.TownNPCs;
using Terraria.DataStructures;
using EverquartzAdventure.UI;
using System.Collections.Generic;
using ReLogic.Utilities;
using System.Linq;
using EverquartzAdventure.UI.Transmogrification;
using ReLogic.Graphics;
using static System.Net.Mime.MediaTypeNames;
using EverquartzAdventure.Buffs.Hypnos;
using static Terraria.ModLoader.PlayerDrawLayer;
using EverquartzAdventure.Items;

namespace EverquartzAdventure
{
    public class TransmogrificationUI : UIState
    {
        #region LanguageKeys
        public static string PlaceItemKey => "Mods.EverquartzAdventure.UI.Transmogrification.PlaceItem";
        public static string ProceedKey => "Mods.EverquartzAdventure.UI.Transmogrification.Proceed";
        public static string TransmogrifingKey => "Mods.EverquartzAdventure.UI.Transmogrification.Transmogrifing";
        public static string DoneKey => "Mods.EverquartzAdventure.UI.Transmogrification.Done";
        #endregion
        private UIItemSlot _vanillaItemSlot;
        private List<UIItemNoSlot> _secondaryItems;
        //private UIItemNoSlot _secondaryMiddle;
        private UIItemSlot _resultItemSlot;
        private UIItemNoSlot _resultPreview;
        //List<int?> testItems = new List<int?>()
        //{
        //    ItemID.MartianArmorDye,
        //    ItemID.Zenith,
        //    ItemID.Wood,
        //};
        List<TransmogrificationRecipe> currenRecipes;
        public int index = 0;
        const int slotX = 50;
        const int slotY = 270;

        #region Overrides
        public override void OnInitialize()
        {
            _vanillaItemSlot = new UIItemSlot(ItemSlot.Context.BankItem, 0.85f)
            {
                Left = { Pixels = slotX },
                Top = { Pixels = slotY },
                ValidItemFunc = item => TransmogrificationManager.CanTrans(item.type),
            };
            // Here we limit the items that can be placed in the slot. We are fine with placing an empty item in or a non-empty item that can be prefixed. Calling Prefix(-3) is the way to know if the item in question can take a prefix or not.
            Append(_vanillaItemSlot);
            _secondaryItems = new List<UIItemNoSlot>();
            UIItemNoSlot one = new UIItemNoSlot(0.5f)
            {
                Left = { Pixels = slotX + 140 },
                Top = { Pixels = slotY + 25 },
            };
            Append(one);
            _secondaryItems.Add(one);
            UIItemNoSlot two = new UIItemNoSlot(0.85f)
            {
                Left = { Pixels = slotX + 155 },
                Top = { Pixels = slotY + 15 },
            };
            Append(two);
            _secondaryItems.Add(two);
            UIItemNoSlot three = new UIItemNoSlot(0.5f)
            {
                Left = { Pixels = slotX + 185 },
                Top = { Pixels = slotY + 27 },
            };
            Append(three);
            _secondaryItems.Add(three);
            _resultItemSlot = new UIItemSlot(ItemSlot.Context.ChestItem, 0.85f)
            {
                Left = { Pixels = slotX + 230 },
                Top = { Pixels = slotY },
                ValidItemFunc = item => !item.IsAir && Main.mouseItem.IsAir,
            };
            Append(_resultItemSlot);
            _resultPreview = new UIItemNoSlot(0.85f)
            {
                Left = { Pixels = slotX + 230 },
                Top = { Pixels = slotY },
                DrawColor = () => new Color(1, 1, 1, 0.1f),
            };
            Append(_resultPreview);

            index = 0;
            try
            {
                int idx = Main.LocalPlayer.Everquartz().transIndex;
                if (idx > -1)
                {
                    index = idx;
                }
            }catch (IndexOutOfRangeException)
            {

            }
            
            
            currenRecipes = new List<TransmogrificationRecipe>();
        }

        // OnDeactivate is called when the UserInterface switches to a different state. In this mod, we switch between no state (null) and this state (ExamplePersonUI).
        // Using OnDeactivate is useful for clearing out Item slots and returning them to the player, as we do here.
        public override void OnDeactivate()
        {
            if (!_vanillaItemSlot.item.IsAir)
            {
                // QuickSpawnClonedItem will preserve mod data of the item. QuickSpawnItem will just spawn a fresh version of the item, losing the prefix.
                Main.LocalPlayer.QuickSpawnClonedItem(Main.LocalPlayer.GetSource_GiftOrReward(), _vanillaItemSlot.item, _vanillaItemSlot.item.stack);
                // Now that we've spawned the item back onto the player, we reset the item by turning it into air.
                _vanillaItemSlot.item.TurnToAir();
            }
            _secondaryItems.ForEach(delegate (UIItemNoSlot slot)
            {
                if (!slot.item.IsAir)
                {
                    slot.item.TurnToAir();
                }
            });
            if (!_resultItemSlot.item.IsAir)
            {
                Main.LocalPlayer.QuickSpawnClonedItem(Main.LocalPlayer.GetSource_GiftOrReward(), _resultItemSlot.item, _resultItemSlot.item.stack);
                _resultItemSlot.item.TurnToAir();
                ResetTransmogrification();
            }
            if (!_resultPreview.item.IsAir)
            {
                _resultPreview.item.TurnToAir();
            }

            Main.LocalPlayer.Everquartz().transIndex = index;
            // Note that in ExamplePerson we call .SetState(new UI.ExamplePersonUI());, thereby creating a new instance of this UIState each time. 
            // You could go with a different design, keeping around the same UIState instance if you wanted. This would preserve the UIState between opening and closing. Up to you.
        }



        // Update is called on a UIState while it is the active state of the UserInterface.
        // We use Update to handle automatically closing our UI when the player is no longer talking to our Example Person NPC.
        public override void Update(GameTime gameTime)
        {
            // Don't delete this or the UIElements attached to this UIState will cease to function.
            base.Update(gameTime);

            // talkNPC is the index of the NPC the player is currently talking to. By checking talkNPC, we can tell when the player switches to another NPC or closes the NPC chat dialog.
            if (Main.LocalPlayer.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].type != ModContent.NPCType<StarbornPrincess>())
            {
                // When that happens, we can set the state of our UserInterface to null, thereby closing this UIState. This will trigger OnDeactivate above.
                EverquartzUI.instance.userInterface.SetState(null);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Main.hidePlayerCraftingMenu = true;

            Item item = Main.LocalPlayer.Everquartz().transResult;
            if (item != null && !item.IsAir)
            {
                DrawTransmogrifing(spriteBatch);
            }
            else
            {
                DrawInsertItem(spriteBatch);
            }
        }
        #endregion

        public static void DrawItemAura(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            float pulse = Main.GlobalTimeWrappedHourly * 0.75f % 1f;
            float outwardnessFactor = MathHelper.Lerp(0.9f, 1.3f, pulse);
            Color color = EverquartzUtils.ColorSwap(new Color(255, 215, 159), new Color(246, 128, 159), new Color(160, 99, 185), 1f) * (1f - pulse);//* 0.27f;
            Texture2D asset = TextureAssets.Item[item.type].Value;
            Vector2 baseDrawPosition = position + frame.Size() * scale / 2;
            drawColor.A = ((byte)0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.Additive, (SamplerState)null, (DepthStencilState)null, (RasterizerState)null, (Effect)null, Main.UIScaleMatrix);
            for (int i = 0; i < 5; i += 2)
            {
                float drawScale = scale * outwardnessFactor;
                Vector2 drawPosition = baseDrawPosition + ((float)Math.PI * 2f * (float)i / 4f).ToRotationVector2() * 4f;
                spriteBatch.Draw(asset, drawPosition, (Rectangle?)frame, color, 0f, frame.Size() * 0.5f, drawScale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, (SamplerState)null, (DepthStencilState)null, (RasterizerState)null, (Effect)null, Main.UIScaleMatrix);
        }

        private bool tickPlayed = false;
        private bool resultGenerated = false;

        private void InteractWithPrev()
        {
            index--;
            NormalizeIndex();
        }

        private void InteractWithNext()
        {
            index++;
            NormalizeIndex();
        }

        private void InteractWithProceed(TransmogrificationRecipe recipe, int transAmount)
        {

            //bool favorited = _vanillaItemSlot.item.favorited;
            //_resultItemSlot.item.SetDefaults(recipe.ResultItemType);
            //_resultItemSlot.item.position.X = Main.LocalPlayer.position.X + (float)(Main.LocalPlayer.width / 2) - (float)(_resultItemSlot.item.width / 2);
            //_resultItemSlot.item.position.Y = Main.LocalPlayer.position.Y + (float)(Main.LocalPlayer.height / 2) - (float)(_resultItemSlot.item.height / 2);
            //_resultItemSlot.item.favorited = favorited;
            //_resultItemSlot.item.stack = _vanillaItemSlot.item.stack * recipe.ResultItemStack;
            Player player = Main.LocalPlayer;
            
            
            EverquartzPlayer modPlayer = Main.LocalPlayer.Everquartz();

            DateTime time = DateTime.UtcNow;
            time += new TimeSpan(0, 0, recipe.TimeInSeconds);
            modPlayer.transEndTime = time;

            Item result = new Item();
            result.SetDefaults(recipe.ResultItemType);
            //result = result.CloneWithModdedDataFrom(_vanillaItemSlot.item);
            result.stack = recipe.ResultItemStack;

            modPlayer.transResult = result.Clone();

            _vanillaItemSlot.item.stack -= transAmount;
            if (_vanillaItemSlot.item.stack <= 0)
            {
                _vanillaItemSlot.item.TurnToAir();
            }
            player.TakeItem(recipe.SecondaryMaterialType, recipe.SecondatyMaterialStack * transAmount);
            //PopupText.NewText(_vanillaItemSlot.Item, _vanillaItemSlot.Item.stack, true, false);
            SoundEngine.PlaySound(SoundID.Item2);
        }

        private void NormalizeIndex()
        {
            if (index >= currenRecipes.Count)
            {
                index = currenRecipes.Count - 1;
            }
            if (index < 0)
            {
                index = 0;
            }
        }

        private void SetupItemSlot(int itemType, int itemStack, ref UIItemNoSlot slot)
        {
            if (itemType != -1)
            {
                if (slot.item.IsAir || slot.item.type != itemType)
                {
                    slot.item.SetDefaults(itemType);
                }
                if (!slot.item.IsAir && slot.item.stack != itemStack)
                {
                    slot.item.stack = itemStack;
                }
            }
            else
            {
                slot.item.TurnToAir();
            }
        }

        private void ClearItemSlot(ref UIItemNoSlot slot)
        {
            if (!slot.item.IsAir)
            {
                slot.item.TurnToAir();
            }
        }
        private void ClearItemSlot(UIItemNoSlot slot)
        {
            ClearItemSlot(ref slot);
        }

        private void DrawInsertItem(SpriteBatch spriteBatch)
        {
            if (!_vanillaItemSlot.item.IsAir && _resultItemSlot.item.IsAir)
            {
                currenRecipes = TransmogrificationManager.FindAvaliableTransByMainIngredient(_vanillaItemSlot.item.type, Main.LocalPlayer)
                    .Concat(TransmogrificationManager.FindUnavaliableTransByMainIngredient(_vanillaItemSlot.item.type, Main.LocalPlayer)).ToList();
                NormalizeIndex();
                
                for (int i = 0; i < 3; i++)
                {
                    UIItemNoSlot slot = _secondaryItems[i];
                    TransmogrificationRecipe recipe = currenRecipes.ElementAtOrDefault(index + i - 1);
                    int amount = Math.Min(_vanillaItemSlot.item.stack, TransmogrificationManager.MaxTransmogrificationAmount(Main.LocalPlayer, recipe));
                    if (amount <= 0)
                    {
                        amount = 1;
                        
                        slot.DrawColor = () => new Color(1, 1, 1, 0.1f);
                    }
                    else
                    {
                        slot.DrawColor = null;
                    }
                    SetupItemSlot(recipe.SecondaryMaterialType, recipe.SecondatyMaterialStack * amount, ref slot);
                }
                TransmogrificationRecipe currenRecipe = currenRecipes.ElementAtOrDefault(index);
                int transAmount = transAmount = Math.Min(_vanillaItemSlot.item.stack, TransmogrificationManager.MaxTransmogrificationAmount(Main.LocalPlayer, currenRecipe)); ;
                bool unavaliable = false;
                if (transAmount <= 0)
                {
                    unavaliable = true;
                    transAmount = 1;
                }
                SetupItemSlot(currenRecipe.ResultItemType, currenRecipe.ResultItemStack * transAmount, ref _resultPreview);

                //DrawSecondariies(spriteBatch);
                Texture2D prevTexture = TextureAssets.ScrollLeftButton.Value;
                int prevX = slotX + 100;
                int prevY = slotY + 40;
                spriteBatch.Draw(prevTexture, new Vector2(prevX, prevY), null, Color.White, 0f, prevTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
                bool hoveringOverPrev = Main.mouseX > prevX - prevTexture.Width && Main.mouseX < prevX + prevTexture.Width && Main.mouseY > prevY - prevTexture.Height && Main.mouseY < prevY + prevTexture.Height && !PlayerInput.IgnoreMouseInterface;
                if (hoveringOverPrev)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    if (Main.mouseLeftRelease && Main.mouseLeft)
                    {
                        InteractWithPrev();
                    }

                }
                Texture2D nextTexture = TextureAssets.ScrollRightButton.Value;
                int nextX = slotX + 120;
                int nextY = slotY + 40;
                spriteBatch.Draw(nextTexture, new Vector2(nextX, nextY), null, Color.White, 0f, nextTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
                bool hoveringOverNext = Main.mouseX > nextX - nextTexture.Width && Main.mouseX < nextX + nextTexture.Width && Main.mouseY > nextY - nextTexture.Height && Main.mouseY < nextY + nextTexture.Height && !PlayerInput.IgnoreMouseInterface;
                if (hoveringOverNext)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    if (Main.mouseLeftRelease && Main.mouseLeft)
                    {
                        InteractWithNext();
                    }

                }
                int timeCost = currenRecipe.TimeInSeconds * transAmount;

                string costText = Language.GetTextValue("LegacyInterface.46") + ": ";
                string coinsText = Lang.LocalizedDuration(new TimeSpan(0, 0, timeCost), abbreviated: true, showAllAvailableUnits: false);

                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, costText, new Vector2(slotX + 50, slotY), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, coinsText, new Vector2(slotX + 50 + FontAssets.MouseText.Value.MeasureString(costText).X, (float)slotY), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
                if (!unavaliable)
                {
                    
                    int reforgeX = slotX + 70;
                    int reforgeY = slotY + 40;
                    bool hoveringOverReforgeButton = Main.mouseX > reforgeX - 15 && Main.mouseX < reforgeX + 15 && Main.mouseY > reforgeY - 15 && Main.mouseY < reforgeY + 15 && !PlayerInput.IgnoreMouseInterface;
                    Texture2D reforgeTexture = TextureAssets.Reforge[hoveringOverReforgeButton ? 1 : 0].Value;

                    spriteBatch.Draw(reforgeTexture, new Vector2(reforgeX, reforgeY), null, Color.White, 0f, reforgeTexture.Size() / 2f, 0.8f, SpriteEffects.None, 0f);
                    if (hoveringOverReforgeButton)
                    {
                        Main.hoverItemName = Language.GetTextValue(ProceedKey);
                        if (!tickPlayed)
                        {
                            SoundEngine.PlaySound(SoundID.MenuTick);
                        }
                        tickPlayed = true;
                        Main.LocalPlayer.mouseInterface = true;
                        if (Main.mouseLeftRelease && Main.mouseLeft)
                        {
                            InteractWithProceed(currenRecipe, transAmount);
                        }
                    }
                    else
                    {
                        tickPlayed = false;
                    }
                }
                //int awesomePrice = Item.buyPrice(0, 1, 0, 0);
                
                //int[] coins = Utils.CoinsSplit(awesomePrice);
                //if (coins[3] > 0)
                //{
                //    coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinPlatinum).Hex3() + ":" + coins[3] + " " + Language.GetTextValue("LegacyInterface.15") + "] ";
                //}
                //if (coins[2] > 0)
                //{
                //    coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinGold).Hex3() + ":" + coins[2] + " " + Language.GetTextValue("LegacyInterface.16") + "] ";
                //}
                //if (coins[1] > 0)
                //{
                //    coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinSilver).Hex3() + ":" + coins[1] + " " + Language.GetTextValue("LegacyInterface.17") + "] ";
                //}
                //if (coins[0] > 0)
                //{
                //    coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinCopper).Hex3() + ":" + coins[0] + " " + Language.GetTextValue("LegacyInterface.18") + "] ";
                //}
                //ItemSlot.DrawSavings(Main.spriteBatch, slotX + 130, Main.instance.invBottom, true);
                
            }
            else
            {
                _secondaryItems.ForEach(ClearItemSlot);
                ClearItemSlot(ref _resultPreview);

                string message = Language.GetTextValue(PlaceItemKey);
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, message, new Vector2(slotX + 50, slotY), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
            }
        }

        private void ResetTransmogrification()
        {
            Main.LocalPlayer.Everquartz().ResetTransmogrification();
            resultGenerated = false;
        }

        private void DrawTransmogrifing(SpriteBatch spriteBatch)
        {
            _secondaryItems.ForEach(ClearItemSlot);
            EverquartzPlayer modPlayer = Main.LocalPlayer.Everquartz();
            
            string message = Language.GetTextValue(TransmogrifingKey);
            long ticks =  modPlayer.transEndTime.Ticks - DateTime.UtcNow.Ticks;
            string timeRemaining = "";
            if (ticks <= 0)
            {
                ClearItemSlot(ref _resultPreview);
                timeRemaining = Language.GetTextValue(DoneKey);
                if (_resultItemSlot.item.IsAir)
                {
                    if (resultGenerated)
                    {
                        ResetTransmogrification();
                    }
                    else
                    {
                        _resultItemSlot.item = modPlayer.transResult.Clone();
                        _resultItemSlot.item.position.X = Main.LocalPlayer.position.X + (float)(Main.LocalPlayer.width / 2) - (float)(_resultItemSlot.item.width / 2);
                        _resultItemSlot.item.position.Y = Main.LocalPlayer.position.Y + (float)(Main.LocalPlayer.height / 2) - (float)(_resultItemSlot.item.height / 2);
                        resultGenerated = true;
                    }
                    
                    
                }
            }
            else
            {
                SetupItemSlot(modPlayer.transResult.type, modPlayer.transResult.stack, ref _resultPreview);
                timeRemaining = Lang.LocalizedDuration(TimeSpan.FromTicks(ticks), abbreviated: true, showAllAvailableUnits: false);
            }
            
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, message, new Vector2(slotX + 50, slotY), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, timeRemaining, new Vector2(slotX + 50, slotY + 20), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
        }

        
    }
    internal class UIItemSlot : UIElement
    {
        internal Item item;
        private readonly int _context;
        private readonly float _scale;
        internal Func<Item, bool> ValidItemFunc;
        internal Action<Item> ItemChangedAction;

        public static Asset<Texture2D> defaultBackgroundTexture = TextureAssets.InventoryBack9;
        public bool hide = false;

        public UIItemSlot(int context = ItemSlot.Context.BankItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;
            item = new Item();
            item.SetDefaults(0);
            hide = false;

            Width.Set(defaultBackgroundTexture.Width() * scale, 0f);
            Height.Set(defaultBackgroundTexture.Height() * scale, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (hide) return;
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();

            if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;

                if (ValidItemFunc == null || Main.mouseItem.IsAir || ValidItemFunc(Main.mouseItem))
                {
                    int oldItemType = item.type;
                    // Handle handles all the click and hover actions based on the context.
                    ItemSlot.Handle(ref item, _context);
                    if (oldItemType != item.type && ItemChangedAction != null)
                    {
                        ItemChangedAction(item);
                    }
                }
            }
            // Draw draws the slot itself and Item. Depending on context, the color will change, as will drawing other things like stack counts.
            ItemSlot.Draw(spriteBatch, ref item, _context, rectangle.TopLeft());
            Main.inventoryScale = oldScale;
        }
    }
    internal class UIItemNoSlot : UIElement
    {
        internal float scale = 0.75f;
        internal int context = 14;

        public int ItemType => item?.type ?? -1;

        public Item item;

        internal Func<Color> DrawColor;

        public UIItemNoSlot(float scale = 0.75f, int context = ItemSlot.Context.ChatItem)
        {
            this.scale = scale;
            this.context = context;
            this.item = new Item();
            Width.Set(32f * scale, 0f);
            Height.Set(32f * scale, 0f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (item == null) return;
            base.Draw(spriteBatch);
            //Vector2 position = GetInnerDimensions().Position();
            //float num = 1f;
            //float num2 = 1f;
            //if (Main.netMode != NetmodeID.Server && !Main.dedServ)
            //{
            //    Texture2D texture2D = TextureAssets.Item[item.type].Value;
            //    Rectangle rectangle = ((Main.itemAnimations[item.type] == null) ? texture2D.Frame() : Main.itemAnimations[item.type].GetFrame(texture2D));
            //    if (rectangle.Height > 32)
            //    {
            //        num2 = 32f / (float)rectangle.Height;
            //    }
            //}
            //num2 *= scale;
            //num *= num2;
            //if (num > 0.75f)
            //{
            //    num = 0.75f;
            //}
            float inventoryScale = Main.inventoryScale;
            Main.inventoryScale = scale;
            ItemSlot.Draw(spriteBatch, ref item, context, GetInnerDimensions().ToRectangle().TopLeft(), DrawColor != null ? DrawColor() : Color.White);
            Main.inventoryScale = inventoryScale;
            if (base.IsMouseHovering)
            {
                Main.hoverItemName = item.Name;
            }
        }
    }
}