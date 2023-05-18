using MonoMod.Cil;
//using On.Terraria;
using IL.Terraria;
using Terraria;
using System;
using Terraria.ModLoader;
using static Mono.Cecil.Cil.OpCodes;
using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System.Reflection;

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
        private static void PrePlaceThingTilesPatch(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoFinalRet();
                //ILLabel finalRetLabel = c.MarkLabel();
                //EverquartzAdventureMod.Instance.Logger.Info(c.Index);

                //c.GotoPrev(i => i.MatchPop());
                //EverquartzAdventureMod.Instance.Logger.Info(c.Index);
                c.GotoPrev(i => i.OpCode == Brfalse_S && ((ILLabel)i.Operand).Target.OpCode == Ret);
                c.GotoPrev(i => i.MatchLdloc(4));

                //Instruction inst = c.Instrs[c.Index].Next;
                //EverquartzAdventureMod.Instance.Logger.Info($"{inst.OpCode} {((ILLabel)inst.Operand)} {((ILLabel)inst.Operand).Target.OpCode}");
                //c.GotoPrev(i => i.OpCode == Ldloc_S);
                //EverquartzAdventureMod.Instance.Logger.Info(c.Index);
                c.Index++;
                //ILLabel label = c.MarkLabel();

                c.Emit(Ldarg_0);
                c.EmitDelegate<Func<bool, Terraria.Player, bool>>((canPlace, player) => player.inventory[player.selectedItem].Everquartz()?.PrePlaceThing_Tiles(player, canPlace) ?? true);
                //c.Emit(Brtrue, label);
                //c.Emit(Ret);
            }
            catch (Exception ex)
            {

                LogFailure("PrePlaceThing_Tiles", ex.ToString());
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
            IL.Terraria.Player.PlaceThing_Tiles += PrePlaceThingTilesPatch;
            //HookEndpointManager.Modify<ILContext.Manipulator>(typeof(NPCLoader).GetMethod("StrikeNPC", BindingFlags.Static | BindingFlags.Public), IAmGoingToHell);
        }

        internal static void Unload()
        {
            IL.Terraria.Player.PlaceThing_Tiles -= PrePlaceThingTilesPatch;
            //HookEndpointManager.Unmodify<ILContext.Manipulator>(typeof(NPCLoader).GetMethod("StrikeNPC", BindingFlags.Static | BindingFlags.Public), IAmGoingToHell);
        }
    }
}