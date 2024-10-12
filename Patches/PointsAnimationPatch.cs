using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(PointsAnimation))]
	internal class PointsAnimationPatch
	{
		[HarmonyPatch("Adder", MethodType.Enumerator)]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> AdderIsFaster(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i).MatchForward(true,
				new(OpCodes.Ldc_I4_5), // Instead of replacing this instruction (because Harmony doesn't like it for some reason), I just change it afterwards
				new(OpCodes.Stloc_2)
				)
			.Advance(1)
			.InsertAndAdvance(
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(weirdAdderType, "value")),
				Transpilers.EmitDelegate((int value) =>
				{
					if (!Plugin.enableProportionalYTPAdder.Value)
						return 5;

					return System.Math.Max(1, Mathf.Abs(Mathf.FloorToInt((Singleton<CoreGameManager>.Instance.GetPoints(0) - value) * 0.025f)));
				}),
				new(OpCodes.Stloc_2)
				)
			.InstructionEnumeration();

		readonly static System.Type weirdAdderType = System.Type.GetType("PointsAnimation+<Adder>d__15, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"); // Weird private class from Generator lol
	}

	
}
