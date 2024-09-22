using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(ITM_Scissors), "Use")]
	internal class ScissorsPatch
	{
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> CutCountsAsBullying(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(true,
				new(OpCodes.Ldarg_1),
				new(CodeInstruction.LoadField(typeof(PlayerManager), "jumpropes")),
				new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(List<Jumprope>), "Count")),
				new(OpCodes.Ldc_I4_0)
			)
			.Advance(2)
			.InsertAndAdvance(
				new(OpCodes.Ldarg_1),
				new(Transpilers.EmitDelegate((PlayerManager pm) => { if (Plugin.enablePlaytimeBullying.Value) pm.RuleBreak("Bullying", 5f); }))
				)
			.InstructionEnumeration();
	}
}
