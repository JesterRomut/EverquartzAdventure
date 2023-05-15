using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using EverquartzAdventure.NPCs.TownNPCs;
using System;
using static Terraria.Player;
using EverquartzAdventure.NPCs.Hypnos;
using EverquartzAdventure.Projectiles.Hypnos;
using System.Collections;

namespace EverquartzAdventure
{
    public class EverquartzPlayer : ModPlayer
    {
        public int praisingTimer = 0;
        public bool IsPraisingHypnos => praisingTimer > 0;

        public bool mindcrashed = false;

        //public Point? lastSleepingSpot = null;

        //public override void SaveData(TagCompound tag)
        //{
        //    if (lastSleepingSpot.HasValue)
        //    {
        //        tag.Add("lastSleepingSpots", new TagCompound() { [Main.worldID.ToString()] = lastSleepingSpot.Value.ToVector2() });
        //    }

        //}

        //public override void LoadData(TagCompound tag)
        //{
        //    lastSleepingSpot = tag.GetCompound("lastSleepingSpots").Get<Vector2>(Main.worldID.ToString()).ToPoint();
        //}

        public override void PostUpdate()
        {
            //if (Player.sleeping.isSleeping)
            //{
            //    lastSleepingSpot = (Player.Bottom + new Vector2(0f, -2f)).ToTileCoordinates();
            //}
            UpdatePraisingHypnos();
        }

        //public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        //{
        //    ModPacket packet = Mod.GetPacket();
        //    packet.Write((byte)EverquartzMessageType.EverquartzSyncPlayer);
        //    packet.Write((byte)Player.whoAmI);
        //    packet.WriteVector2(lastSleepingSpot.GetValueOrDefault().ToVector2());
        //    packet.Send(toWho, fromWho);
        //}

        public override bool PreItemCheck()
        {
            if (IsPraisingHypnos)
            {
                int num9 = Player.miscCounter % 14 / 7;
                CompositeArmStretchAmount stretch = CompositeArmStretchAmount.ThreeQuarters;
                float num2 = 0.3f;
                if (num9 == 1)
                {
                    //stretch = CompositeArmStretchAmount.Full;
                    num2 = 0.35f;
                }

                Player.SetCompositeArmBack(enabled: true, stretch, (float)Math.PI * -2f * num2 * (float)Player.direction);
                Player.SetCompositeArmFront(enabled: true, stretch, (float)Math.PI * -2f * num2 * (float)Player.direction);

                return false;
            }
            return true;
        }

        public override void ResetEffects()
        {
            ResetBuffs();
        }

        public override void UpdateDead()
        {
            ResetBuffs();
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (npc.Everquartz().mindcrashed > 0)
            {
                damage -= (int)Math.Floor(damage * 0.1);
            }
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (target.Everquartz().mindcrashed > 0)
            {
                crit = true;
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.Everquartz().mindcrashed > 0)
            {
                crit = true;
            }
        }
    

        public void ResetBuffs()
        {
            mindcrashed = false;
        }

        #region Deimos

        public void HandleDeadDeimos(Player murderer)
        {
            StarbornPrincess.DeathEffectClient(murderer.position, murderer.width, murderer.height);
        }

        #endregion

        #region Hypnos
        public void InterruptPraisingHypnos()
        {
            praisingTimer = 0;
        }

        public void DonePraisingHypnos()
        {
            //client side
            NPC hypnos = NPCs.Hypnos.Hypnos.Instance;
            AergiaNeuron.AddElectricDusts(hypnos != null ? hypnos : Player);

            List<HypnosReward> rewards = NPCs.Hypnos.Hypnos.GenerateRewards();


            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                NPCs.Hypnos.Hypnos.HandleRewardsServer(Player, rewards);
            }
            else
            {
                int rewardCount = EverquartzUtils.EnumCount<HypnosReward>();
                //this.Mod.Logger.Info(rewards);
                ModPacket packet = base.Mod.GetPacket();
                packet.Write((byte)EverquartzMessageType.HypnosReward);
                packet.Write(Player.whoAmI);
                bool[] rewardBools = new bool[rewardCount];
                rewards.ForEach(reward => rewardBools[(int)reward] = true);
                BitArray bitArray = new BitArray(rewardBools);
                packet.Write(bitArray.ToByteArray());

                packet.Send();
            }

            InterruptPraisingHypnos();
        }



        public void UpdatePraisingHypnos()
        {
            if (!IsPraisingHypnos)
            {
                return;
            }
            if (Player.talkNPC == -1)
            {
                InterruptPraisingHypnos();
                return;
            }
            int num = Math.Sign(Main.npc[Player.talkNPC].Center.X - Player.Center.X);
            if (Player.controlLeft || Player.controlRight || Player.controlUp || Player.controlDown || Player.controlJump || Player.pulley || Player.mount.Active || num != Player.direction)
            {
                InterruptPraisingHypnos();
                return;
            }
            praisingTimer--;
            if (praisingTimer <= 0)
            {
                DonePraisingHypnos();
            }
        }

        #endregion

    }

}