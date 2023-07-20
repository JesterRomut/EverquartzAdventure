﻿using MonoMod.Cil;
using System;
using static Mono.Cecil.Cil.OpCodes;
using Mono.Cecil.Cil;

namespace EverquartzAdventure.ILEditing
{
    internal static class ILEditingUtils
    {
        public static void LogFailure(string modName, string name, string reason)
        {
            EverquartzAdventureMod.Instance.Logger.Warn($"{modName} IL Edit for {name} failed: {reason}");
        }
    }
    internal static class ILChanges
    {
        internal static void LogFailure(string name, string reason) => ILEditingUtils.LogFailure("Vanilla", name, reason);

        // I found this patch is fucking quite unnecessary ↓ fuck me  -jester
        // Hypnos, i want you...♥ Fuck me please
        //private static void PrePlaceThingTilesPatch(ILContext il)
        //{
        //    try
        //    {
        //        ILCursor c = new ILCursor(il);
        //        c.GotoFinalRet();
        //        //ILLabel finalRetLabel = c.MarkLabel();
        //        //EverquartzAdventureMod.Instance.Logger.Info(c.Index);

        //        //c.GotoPrev(i => i.MatchPop());
        //        //EverquartzAdventureMod.Instance.Logger.Info(c.Index);
        //        c.GotoPrev(i => i.OpCode == Brfalse_S && ((ILLabel)i.Operand).Target.OpCode == Ret);
        //        c.GotoPrev(i => i.MatchLdloc(4));

        //        //Instruction inst = c.Instrs[c.Index].Next;
        //        //EverquartzAdventureMod.Instance.Logger.Info($"{inst.OpCode} {((ILLabel)inst.Operand)} {((ILLabel)inst.Operand).Target.OpCode}");
        //        //c.GotoPrev(i => i.OpCode == Ldloc_S);
        //        //EverquartzAdventureMod.Instance.Logger.Info(c.Index);
        //        c.Index++;
        //        //ILLabel label = c.MarkLabel();

        //        c.Emit(Ldarg_0);
        //        c.EmitDelegate<Func<bool, Terraria.Player, bool>>((canPlace, player) => player.inventory[player.selectedItem].ModItem()?.PrePlaceThing_Tiles(player, canPlace) ?? true);
        //        //c.Emit(Brtrue, label);
        //        //c.Emit(Ret);
        //    }
        //    catch (Exception ex)
        //    {

        //        LogFailure("PrePlaceThing_Tiles", ex.ToString());
        //        return;
        //    }
        //}

        private static void TownNPCCustomDeathMessage(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(i => i.OpCode == Stloc_S && ((VariableDefinition)i.Operand).Index == 6);
                c.GotoPrev(i => i.OpCode == Ldc_I4_1);
                //ILLabel label1 = c.MarkLabel();
                //c.GotoPrev(i => i.MatchLdsfld<Terraria.Lang>(nameof(Terraria.Lang.misc)));
                //ILLabel label = c.MarkLabel();
                //c.GotoLabel(label1);

                c.Emit(Ldarg_0);
                c.EmitDelegate<Func<Terraria.NPC, bool>>(npc => string.IsNullOrEmpty(npc.ModNPC()?.TownNPCDeathMessageKey));
                c.Emit(Brtrue, c.DefineLabel());
                int brtruePos = c.Index - 1;
                c.Emit(Pop);
                c.Emit(Ldarg_0);
                c.EmitDelegate<Func<Terraria.NPC, string>>(npc => npc.ModNPC()?.TownNPCDeathMessageKey ?? "err");

                ILLabel label = c.MarkLabel();

                c.Index = brtruePos;
                c.Next.Operand = label;

                c.GotoLabel(label);

                
                //c.Emit(Ldarg_0);
                //c.EmitDelegate<Func<Terraria.NPC, bool>>(npc => string.IsNullOrEmpty(npc.ModNPC()?.TownNPCDeathMessageKey));
                //c.Emit(Brtrue, label);
                //c.Emit(Ldarg_0);
                //c.EmitDelegate<Func<Terraria.NPC, string>>(npc => npc.ModNPC()?.TownNPCDeathMessageKey ?? "");
                //c.Emit(Br, label1);

                //c.Goto(0);

                //while (c.TryGotoNext(i => i.MatchLdcI4(25) && i.Previous.MatchLdcI4(25) && i.Previous.Previous.MatchLdcI4(255)))
                ////while (c.TryGotoNext(i => i.MatchLdcI4(255) && i.Next.MatchLdcI4(25) && i.Next.Next.MatchLdcI4(25)))
                //{
                //    //EverquartzAdventureMod.Instance.Logger.Info($"{c.Index}");
                //    //c.Index++;
                //    c.GotoNext();
                //    ILLabel label2 = c.MarkLabel();
                //    //c.GotoNext();
                //    //c.GotoNext();
                //    //c.GotoNext();
                //    ////Instruction inst = c.Instrs[c.Index];
                //    ////EverquartzAdventureMod.Instance.Logger.Info($"{inst.OpCode} {inst.Next.OpCode}");
                //    //ILLabel label3 = c.MarkLabel();

                //    c.GotoLabel(label2);
                //    //c.Emit(Ldarg_0);
                //    //c.EmitDelegate<Func<Terraria.NPC, bool>>(npc => npc.ModNPC() != null && npc.ModNPC().TownNPCDeathMessageColor.HasValue);
                //    //c.Emit(Brfalse, label2);
                //    //c.Emit(Ldarg_0);
                //    //c.EmitDelegate<Func<Terraria.NPC, int>>(npc => npc.ModNPC().TownNPCDeathMessageColor.Value.R);
                //    //c.Emit(Ldarg_0);
                //    //c.EmitDelegate<Func<Terraria.NPC, int>>(npc => npc.ModNPC().TownNPCDeathMessageColor.Value.G);
                //    //c.Emit(Ldarg_0);
                //    //c.EmitDelegate<Func<Terraria.NPC, int>>(npc => npc.ModNPC().TownNPCDeathMessageColor.Value.B);
                //    //c.Emit(Br, label3);
                //    //c.GotoLabel(label3);

                //    //c.GotoLabel(label2); // try replace all index-- to gotolabel

                //    //inst = c.Instrs[c.Index];
                //    //EverquartzAdventureMod.Instance.Logger.Info($"{inst.OpCode} {inst.Next.OpCode}");
                //    c.Emit(Ldarg_0);
                //    c.EmitDelegate<Func<Terraria.NPC, bool>>(npc => npc.ModNPC() != null && npc.ModNPC().TownNPCDeathMessageColor.HasValue);
                //    c.Emit(Brfalse, label2);
                //    c.Emit(Pop);
                //    c.Emit(Pop);
                //    c.Emit(Pop);
                //    c.Emit(Ldarg_0);
                //    c.EmitDelegate<Func<Terraria.NPC, int>>(npc => npc.ModNPC().TownNPCDeathMessageColor.Value.R);
                //    c.Emit(Ldarg_0);
                //    c.EmitDelegate<Func<Terraria.NPC, int>>(npc => npc.ModNPC().TownNPCDeathMessageColor.Value.G);
                //    c.Emit(Ldarg_0);
                //    c.EmitDelegate<Func<Terraria.NPC, int>>(npc => npc.ModNPC().TownNPCDeathMessageColor.Value.B);
                //}

            }
            catch (Exception ex)
            {

                LogFailure("TownNPCCustomDeathMessage", ex.ToString());
                return;
            }
        }

        //private static void IAmGoingToHell(ILContext il)
        //{
        //    try
        //    {
        //        ILCursor c = new ILCursor(il);
        //        c.Goto(0);
        //        ILLabel label = c.MarkLabel();
        //        c.Goto(0);
        //        c.Emit(Ldarg_1);
        //        c.EmitDelegate<Func<Terraria.NPC, bool>>(npc => (npc.Everquartz()?.mindcrashed ?? 0) > 0);
        //        //c.EmitDelegate<Action>(() => EverquartzAdventureMod.Instance.Logger.Info("111"));
        //        c.Emit(Brfalse, label);
        //        c.Emit(Ldc_I4_1);
        //        c.Emit(Ret);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogFailure("StrikeNPC", ex.ToString());
        //        return;
        //    }
        //}

        internal static void Load()
        {
            //IL.Terraria.Player.PlaceThing_Tiles += PrePlaceThingTilesPatch;
            Terraria.IL_NPC.checkDead += TownNPCCustomDeathMessage;
            //HookEndpointManager.Modify<ILContext.Manipulator>(typeof(NPCLoader).GetMethod("StrikeNPC", BindingFlags.Static | BindingFlags.Public), IAmGoingToHell);
        }

        internal static void Unload()
        {
            //IL.Terraria.Player.PlaceThing_Tiles -= PrePlaceThingTilesPatch;
            Terraria.IL_NPC.checkDead -= TownNPCCustomDeathMessage;
            //HookEndpointManager.Unmodify<ILContext.Manipulator>(typeof(NPCLoader).GetMethod("StrikeNPC", BindingFlags.Static | BindingFlags.Public), IAmGoingToHell);
        }
    }
}