using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch]
    class ChalkFacePatch
    {
		[HarmonyPatch(typeof(ChalkFace_Idle), "Enter")]
		[HarmonyPostfix]
		static void ActuallyCancelMaBoy(ChalkFace ___chalkles)
		{
			if (Plugin.enableChalklesInstaDisable.Value)
				___chalkles.charge = 0; // Literally resets it
		}

		[HarmonyPatch(typeof(ChalkFace), "AdvanceTimer")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> ChalklesBalancedSpawn(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(true, 
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(ChalkFace), "charge")),
				new(OpCodes.Call, AccessTools.PropertyGetter(typeof(Time), "deltaTime")),
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(NPC), "ec")),
				new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(EnvironmentController), "NpcTimeScale")),
				new(OpCodes.Mul)
				)
			.Advance(1)
			.InsertAndAdvance(
				new(OpCodes.Ldarg_0),
				Transpilers.EmitDelegate((ChalkFace x) =>
				{
					if (!Plugin.enableChalklesProportionalSpawn.Value) return 1f;
					var size = x.ec.CellFromPosition(x.transform.position).room.size;
					return Mathf.Min(1f, 1f / Mathf.Sqrt(size.x + size.z) * 2f);
				}),
				new(OpCodes.Mul)
				)
			.InstructionEnumeration();
    }
}
