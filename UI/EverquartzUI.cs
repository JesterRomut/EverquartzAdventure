using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace EverquartzAdventure.UI
{
    public class EverquartzUI: ModSystem
    {
        internal UserInterface userInterface;
        internal TransmogrificationUI transmogrificationUI;

        public static EverquartzUI instance;

        private GameTime _lastUpdateUiGameTime;

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (userInterface?.CurrentState != null)
            {
                userInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "EverquartzAdventure: userInterface",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && userInterface?.CurrentState != null)
                        {
                            userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                LoadClient();
            }
            instance = this;
            base.Load();
        }

        public override void Unload()
        {
            transmogrificationUI = null;

            instance = null;
            base.Unload();
        }
        public void LoadClient()
        {
            userInterface = new UserInterface();

            transmogrificationUI = new TransmogrificationUI();
            transmogrificationUI.Activate();
        }
    }
}