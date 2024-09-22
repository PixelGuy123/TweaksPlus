using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(LevelGenerator), "Generate", MethodType.Enumerator)]
	internal class GeneratorPatch
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.Start() // Make negative and positive seeds different (NPC phase)
			.MatchForward(true,
				new(OpCodes.Ldloc_2),
				new(OpCodes.Call, AccessTools.PropertyGetter(typeof(CoreGameManager), "Instance")),
				new(OpCodes.Callvirt, AccessTools.Method("CoreGameManager:Seed")),
				new(OpCodes.Newobj, AccessTools.Constructor(typeof(System.Random), [typeof(int)])),
				new(CodeInstruction.StoreField(typeof(LevelBuilder), "controlledRNG"))
				).Advance(1)
			.InsertAndAdvance(
				new(OpCodes.Ldarg_0),
				CodeInstruction.Call(typeof(GeneratorPatch), "SkipRngs", [typeof(LevelGenerator)])
				)

			.MatchForward(true, // Make negative and positive seeds different (Gen phase)
				new(OpCodes.Ldloc_2),
				new(OpCodes.Call, AccessTools.PropertyGetter(typeof(CoreGameManager), "Instance")),
				new(OpCodes.Callvirt, AccessTools.Method("CoreGameManager:Seed")),
				new(OpCodes.Ldloc_2),
				new(CodeInstruction.LoadField(typeof(LevelBuilder), "seedOffset")),
				new(OpCodes.Add),
				new(OpCodes.Newobj, AccessTools.Constructor(typeof(System.Random), [typeof(int)])),
				new(CodeInstruction.StoreField(typeof(LevelBuilder), "controlledRNG"))
				).Advance(1)
			.InsertAndAdvance(
				new(OpCodes.Ldarg_0),
				CodeInstruction.Call(typeof(GeneratorPatch), "SkipRngs", [typeof(LevelGenerator)])
				)
			.InstructionEnumeration();

		static void SkipRngs(LevelGenerator i)
		{
			if (!Plugin.enableNegativeUniqueness.Value || i.seed >= 0) return;

			int amount = i.controlledRNG.Next(2, 10);
			for (int a = 0; a < amount; a++)
			{
				i.controlledRNG.Next();
				i.FrameShouldEnd();
			}
		}
	}
}
