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
using EverquartzAdventure.UI.Transmogrification;
using Terraria.ModLoader.IO;
using Terraria.GameInput;
using EverquartzAdventure.Items.Placeable.MusicBoxes;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace EverquartzAdventure
{
    public class EverquartzPlayer : ModPlayer
    {
        

        public bool mindcrashed = false;
        public bool musicBoxTrolled = false;
        //public int musicBoxTrollAttempt = 0;

        //public Point? lastSleepingSpot = null;

        public override void SaveData(TagCompound tag)
        {
            tag.Add("transmogrification", SaveTransmogrificationInfo());
        }

        public override void LoadData(TagCompound tag)
        {
            LoadTransmogrificationInfo(tag.GetCompound("transmogrification"));
        }

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
                PraisingHypnosAnimation();
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

        //public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        //{
        //    if (npc.Everquartz().mindcrashed > 0)
        //    {
        //        damage -= (int)Math.Floor(damage * 0.1);
        //    }
        //}

        //public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)/* tModPorter If you don't need the Item, consider using ModifyHitNPC instead */
        //{
        //    if (target.Everquartz().mindcrashed > 0)
        //    {
        //        modifiers.FinalDamage.Base += Player.GetWeaponDamage(item) / 2;
        //        //var debug = Player.GetWeaponDamage(item);
        //        //modifiers.SetCrit();
        //    }
        //}

        //public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)/* tModPorter If you don't need the Projectile, consider using ModifyHitNPC instead */
        //{
        //    if (target.Everquartz().mindcrashed > 0)
        //    {
        //        modifiers.FinalDamage.Base += proj.damage / 2;
        //    }
        //}


        public void ResetBuffs()
        {
            mindcrashed = false;
        }

        #region Deimos
        //public DateTime transStartTime;
        public Item transResult;
        public DateTime transEndTime;
        public int transIndex = -1;

        private TagCompound SaveTransmogrificationInfo()
        {
            TagCompound result = new TagCompound();
            if (transResult != null && !transResult.IsAir)
            {
                result.Add("transEndTime", transEndTime.ToISO8601());
                //result.Add("transTime", transStartTime.ToISO8601());
                result.Add("transResult", transResult);
            }
            if (transIndex > -1)
            {
                result.Add("transIndex", transIndex);
            }
            
            return result;
        }

        public void ResetTransmogrification()
        {
            //transStartTime = default;
            transEndTime = default;
            transResult?.TurnToAir();
            transResult = null;
        }

        private void LoadTransmogrificationInfo(TagCompound tag)
        {
            //transStartTime = tag.GetString("transTime").ToISO8601();
            transEndTime = tag.GetString("transEndTime").ToISO8601();
            transResult = tag.Get<Item>("transResult");
            transIndex = tag.GetInt("transIndex");
        }

        #endregion

        #region Hypnos

        public int praisingTimer = 0;
        public bool IsPraisingHypnos => praisingTimer > 0;
        public void InterruptPraisingHypnos()
        {
            praisingTimer = 0;
        }

        private void PraisingHypnosAnimation()
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