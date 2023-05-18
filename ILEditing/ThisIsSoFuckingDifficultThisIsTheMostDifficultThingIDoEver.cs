using MonoMod.Cil;
//using On.Terraria;
using IL.Terraria;
using Terraria;
using System;
using Terraria.ModLoader;
using static Mono.Cecil.Cil.OpCodes;
using Mono.Cecil.Cil;

namespace EverquartzAdventure.ILEditing
{
    public static class ILChanges
    {
        // I found this patch is fucking quite unnecessary ↓ fuck me  -jester
        // Hypnos, i want you...♥ Fuck me please
        private static void PrePlaceThingTilesPatch(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoFinalRet();
                ILLabel finalRetLabel = c.MarkLabel();
                //EverquartzAdventureMod.Instance.Logger.Info(c.Index);
                
                //c.GotoPrev(i => i.MatchPop());
                //EverquartzAdventureMod.Instance.Logger.Info(c.Index);
                c.GotoPrev(i => i.MatchLdloc(4) && i.Next.OpCode == Brfalse_S);

                Instruction inst = c.Instrs[c.Index].Next;
                EverquartzAdventureMod.Instance.Logger.Info($"{inst.OpCode} {((ILLabel)inst.Operand)} {((ILLabel)inst.Operand).Target.OpCode}");
                //c.GotoPrev(i => i.OpCode == Ldloc_S);
                //EverquartzAdventureMod.Instance.Logger.Info(c.Index);
                c.Index++;
                //ILLabel label = c.MarkLabel();

                c.Emit(Ldarg_0);
                c.EmitDelegate < Func < bool, Terraria.Player, bool>>((canPlace, player) => player.inventory[player.selectedItem].Everquartz()?.PrePlaceThing_Tiles(player, canPlace) ?? true);
                //c.Emit(Brtrue, label);
                //c.Emit(Ret);
            }
            catch(Exception ex) { 
            
                LogFailure("Patch PrePlaceThing_Tiles", ex.ToString());
                return;
            }
        }

        public static void LogFailure(string name, string reason)
        {
            EverquartzAdventureMod.Instance.Logger.Warn("IL edit \"" + name + "\" failed! " + reason);
        }

        internal static void Load()
        {
            IL.Terraria.Player.PlaceThing_Tiles += PrePlaceThingTilesPatch;
        }

        internal static void Unload()
        {
            IL.Terraria.Player.PlaceThing_Tiles -= PrePlaceThingTilesPatch;
        }
    }
}