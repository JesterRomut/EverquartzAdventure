using CalamityMod.NPCs;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System.Reflection;
using System;
using static Mono.Cecil.Cil.OpCodes;
using Terraria;
using EverquartzAdventure.Buffs.Hypnos;
using Terraria.ModLoader;

namespace EverquartzAdventure.ILEditing
{
    [JITWhenModsEnabled("CalamityMod")]
    internal static class CalamityILChanges
    {
        internal static void LogFailure(string name, string reason) => ILEditingUtils.LogFailure("CalamityMod", name, reason);

        // this is a test for il editing, it's damn successful
        private static void ThisIsTotallyForModCompatibility(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.Goto(0);
                ILLabel label = c.MarkLabel();
                c.Goto(0);
                c.Emit(Ldarg_1);
                c.EmitDelegate<Func<NPC, bool>>(npc => (npc.Everquartz()?.mindcrashed ?? 0) > 0);
                //c.EmitDelegate<Action>(() => EverquartzAdventureMod.Instance.Logger.Info("111"));
                c.Emit(Brfalse, label);
                c.Emit(Ldarg_2);
                c.Emit(Ret);
            }
            catch(Exception ex)
            {
                LogFailure("ApplyDR", ex.ToString());
                return;
            }
        }

        internal static void Load()
        {
            HookEndpointManager.Modify<ILContext.Manipulator>(typeof(CalamityGlobalNPC).GetMethod("ApplyDR", BindingFlags.Instance | BindingFlags.NonPublic), ThisIsTotallyForModCompatibility);
        }
        internal static void Unload()
        {
            HookEndpointManager.Unmodify<ILContext.Manipulator>(typeof(CalamityGlobalNPC).GetMethod("ApplyDR", BindingFlags.Instance | BindingFlags.NonPublic), ThisIsTotallyForModCompatibility);
        }
    }
}