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

namespace EverquartzAdventure
{
    public class TransmogrificationUI : UIState
    {
        private UIItemSlot _vanillaItemSlot;
        private List<UIItemNoSlot> _secondaryItems;
        //private UIItemNoSlot _secondaryMiddle;
        private UIItemSlot _resultItemSlot;
        List<int> testItems = new List<int>()
        {
            ItemID.MartianArmorDye,
            ItemID.Zenith,
            ItemID.Wood,
        };
        const int slotX = 50;
        const int slotY = 270;
        public override void OnInitialize()
        {
            _vanillaItemSlot = new UIItemSlot(ItemSlot.Context.BankItem, 0.85f)
            {
                Left = { Pixels = slotX },
                Top = { Pixels = slotY },
                ValidItemFunc = item => true
            };
            // Here we limit the items that can be placed in the slot. We are fine with placing an empty item in or a non-empty item that can be prefixed. Calling Prefix(-3) is the way to know if the item in question can take a prefix or not.
            Append(_vanillaItemSlot);
            _secondaryItems = new List<UIItemNoSlot>();
            UIItemNoSlot one = new UIItemNoSlot(0.5f)
            {
                Left = { Pixels = slotX + 120 },
                Top = { Pixels = slotY + 20 },
            };
            Append(one);
            _secondaryItems.Add(one);
            UIItemNoSlot two = new UIItemNoSlot(0.85f, ItemSlot.Context.BankItem)
            {
                Left = { Pixels = slotX + 140 },
                Top = { Pixels = slotY + 20 },
            };
            Append(two);
            _secondaryItems.Add(two);
            UIItemNoSlot three = new UIItemNoSlot(0.5f)
            {
                Left = { Pixels = slotX + 160 },
                Top = { Pixels = slotY + 20 },
            };
            Append(three);
            _secondaryItems.Add(three);
                //_secondaryItems = new List<UIItemNoSlot>()
                //{

                //    new UIItemNoSlot(0.9f){ 
                //        Left = { Pixels = slotX + 140 },
                //        Top = { Pixels = slotY + 20 },
                //    },
                //    new UIItemNoSlot(0.5f){
                //        Left = { Pixels = slotX + 160 },
                //        Top = { Pixels = slotY + 20 },
                //    },
                //};
                //Array.ForEach(_secondaryItems, slot => Append(slot));

                //_secondaryMiddle = new UIItemNoSlot(1)
                //{
                //    Left = { Pixels = slotX + 120 },
                //    Top = { Pixels = slotY + 20 },
                //}; 
                //Append(_secondaryMiddle);
                _resultItemSlot = new UIItemSlot(ItemSlot.Context.ChestItem, 0.85f)
            {
                Left = { Pixels = slotX + 210 },
                Top = { Pixels = slotY },
                ValidItemFunc = item => false
            };
            Append(_resultItemSlot);
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

        private bool tickPlayed;

        //private void DrawSecondariies(SpriteBatch spriteBatch)
        //{
        //    int secondaryMiddleX = slotX + 100;
        //    int secondaryMiddleY = slotY + 40;
        //    Texture2D secondaryMiddleTexture = TextureAssets.Item[testItems[0]].Value;
        //    Vector2 size = secondaryMiddleTexture.Size();
        //    bool hoveringOverMiddle = Main.mouseX > secondaryMiddleX - size.X && Main.mouseX < secondaryMiddleX + size.X && Main.mouseY > secondaryMiddleY - size.Y && Main.mouseY < secondaryMiddleY + size.Y && !PlayerInput.IgnoreMouseInterface;
        //    spriteBatch.Draw(secondaryMiddleTexture, new Vector2(secondaryMiddleX, secondaryMiddleY), null, Color.White, 0f, size, 0.8f, SpriteEffects.None, 0f);
        //    if (hoveringOverMiddle)
        //    {
        //        Main.hoverItemName = testItems[0];
        //    }
        //    }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            // This will hide the crafting menu similar to the reforge menu. For best results this UI is placed before "Vanilla: Inventory" to prevent 1 frame of the craft menu showing.
            //Main.HidePlayerCraftingMenu = true;
            Main.hidePlayerCraftingMenu = true;



            #region SecondaryMaterial
            //DrawSecondariies(spriteBatch);
            #endregion

            #region ProceedButton
            // Here we have a lot of code. This code is mainly adapted from the vanilla code for the reforge option.
            // This code draws "Place an item here" when no item is in the slot and draws the reforge cost and a reforge button when an item is in the slot.
            // This code could possibly be better as different UIElements that are added and removed, but that's not the main point of this example.
            // If you are making a UI, add UIElements in OnInitialize that act on your ItemSlot or other inputs rather than the non-UIElement approach you see below.


            if (!_vanillaItemSlot.item.IsAir)
            {
                _secondaryItems.ForEach(delegate (UIItemNoSlot slot)
                {
                    if (slot.item.IsAir)
                    {
                        slot.item.SetDefaults(_vanillaItemSlot.item.type);
                        slot.item.CloneWithModdedDataFrom(_vanillaItemSlot.item);
                    }
                });

                int awesomePrice = Item.buyPrice(0, 1, 0, 0);


                string costText = Language.GetTextValue("LegacyInterface.46") + ": ";
                string coinsText = "";
                int[] coins = Utils.CoinsSplit(awesomePrice);
                if (coins[3] > 0)
                {
                    coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinPlatinum).Hex3() + ":" + coins[3] + " " + Language.GetTextValue("LegacyInterface.15") + "] ";
                }
                if (coins[2] > 0)
                {
                    coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinGold).Hex3() + ":" + coins[2] + " " + Language.GetTextValue("LegacyInterface.16") + "] ";
                }
                if (coins[1] > 0)
                {
                    coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinSilver).Hex3() + ":" + coins[1] + " " + Language.GetTextValue("LegacyInterface.17") + "] ";
                }
                if (coins[0] > 0)
                {
                    coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinCopper).Hex3() + ":" + coins[0] + " " + Language.GetTextValue("LegacyInterface.18") + "] ";
                }
                ItemSlot.DrawSavings(Main.spriteBatch, slotX + 130, Main.instance.invBottom, true);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, costText, new Vector2(slotX + 50, slotY), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, coinsText, new Vector2(slotX + 50 + FontAssets.MouseText.Value.MeasureString(costText).X, (float)slotY), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
                int reforgeX = slotX + 70;
                int reforgeY = slotY + 40;
                bool hoveringOverReforgeButton = Main.mouseX > reforgeX - 15 && Main.mouseX < reforgeX + 15 && Main.mouseY > reforgeY - 15 && Main.mouseY < reforgeY + 15 && !PlayerInput.IgnoreMouseInterface;
                Texture2D reforgeTexture = TextureAssets.Reforge[hoveringOverReforgeButton ? 1 : 0].Value;

                spriteBatch.Draw(reforgeTexture, new Vector2(reforgeX, reforgeY), null, Color.White, 0f, reforgeTexture.Size() / 2f, 0.8f, SpriteEffects.None, 0f);
                if (hoveringOverReforgeButton)
                {
                    Main.hoverItemName = Language.GetTextValue("LegacyInterface.19");
                    if (!tickPlayed)
                    {
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                    tickPlayed = true;
                    Main.LocalPlayer.mouseInterface = true;
                    if (Main.mouseLeftRelease && Main.mouseLeft && Main.LocalPlayer.CanBuyItem(awesomePrice, -1) && ItemLoader.PreReforge(_vanillaItemSlot.item))
                    {
                        Main.LocalPlayer.BuyItem(awesomePrice, -1);
                        bool favorited = _vanillaItemSlot.item.favorited;
                        int stack = _vanillaItemSlot.item.stack;
                        Item reforgeItem = new Item();
                        reforgeItem.netDefaults(_vanillaItemSlot.item.netID);
                        reforgeItem = reforgeItem.CloneWithModdedDataFrom(_vanillaItemSlot.item);
                        // This is the main effect of this slot. Giving the Awesome prefix 90% of the time and the ReallyAwesome prefix the other 10% of the time. All for a constant 1 gold. Useless, but informative.

                        _vanillaItemSlot.item = reforgeItem.Clone();
                        _vanillaItemSlot.item.position.X = Main.LocalPlayer.position.X + (float)(Main.LocalPlayer.width / 2) - (float)(_vanillaItemSlot.item.width / 2);
                        _vanillaItemSlot.item.position.Y = Main.LocalPlayer.position.Y + (float)(Main.LocalPlayer.height / 2) - (float)(_vanillaItemSlot.item.height / 2);
                        _vanillaItemSlot.item.favorited = favorited;
                        _vanillaItemSlot.item.stack = stack;
                        ItemLoader.PostReforge(_vanillaItemSlot.item);
                        //PopupText.NewText(_vanillaItemSlot.Item, _vanillaItemSlot.Item.stack, true, false);
                        SoundEngine.PlaySound(SoundID.Item37);
                    }
                }
                else
                {
                    tickPlayed = false;
                }
            }
            else
            {
                _secondaryItems.ForEach(delegate (UIItemNoSlot slot)
                {
                    if (!slot.item.IsAir)
                    {
                        slot.item.TurnToAir();
                    }
                });

                string message = "Place an item here to Awesomeify";
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, message, new Vector2(slotX + 50, slotY), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
            }
            #endregion
        }
    }
    internal class UIItemSlot : UIElement
    {
        internal Item item;
        private readonly int _context;
        private readonly float _scale;
        internal Func<Item, bool> ValidItemFunc;

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
                if (ValidItemFunc == null || ValidItemFunc(Main.mouseItem))
                {
                    // Handle handles all the click and hover actions based on the context.
                    ItemSlot.Handle(ref item, _context);
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
            ItemSlot.Draw(spriteBatch, ref item, context, GetInnerDimensions().ToRectangle().TopLeft(), Color.White);
            Main.inventoryScale = inventoryScale;
            if (base.IsMouseHovering)
            {
                Main.hoverItemName = item.Name;
            }
        }
    }
}