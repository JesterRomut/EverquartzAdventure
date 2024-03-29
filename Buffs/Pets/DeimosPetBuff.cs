﻿using Terraria;
using Terraria.ModLoader;
using EverquartzAdventure.Projectiles.Pets;

namespace EverquartzAdventure.Buffs.Pets
{
	public class DeimosPetBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
            // DisplayName.SetDefault("Jar Creature");
			//DisplayName.AddTranslation(7, "缸中生物");
			//DisplayName.AddTranslation(6, "Тварь В Банке");
            Main.vanityPet[((ModBuff)this).Type] = true;
            // Description.SetDefault("It is not snow.");
			//Description.AddTranslation(7, "不是雪（悲）");
			//Description.AddTranslation(6, "Это не снег.");
		}

		public override void Update(Player player, ref int buffIndex) { // This method gets called every frame your buff is active on your player.
			bool unused = false;
			player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<DeimosPetProjectile>());
		}
	}
}