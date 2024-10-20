using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(Ambience), "Timer", MethodType.Enumerator)]
	internal class AmbiencePatches
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(false, 
				new(OpCodes.Ldloc_1),
				new(OpCodes.Call, AccessTools.PropertyGetter(typeof(Component), "transform")),
				new(OpCodes.Ldloc_1),
				new(CodeInstruction.LoadField(typeof(Ambience), "pos")),
				new(OpCodes.Callvirt, AccessTools.PropertySetter(typeof(Transform), "position"))
				)
			.InsertAndAdvance(
				new(OpCodes.Ldloc_1),
				Transpilers.EmitDelegate((Ambience amb) =>
				{
					float ambFac = Plugin.ambienceDistancingFactor.Value;
					if (ambFac <= 0.5)
						return;
					
					ambFac *= 10f + ((amb.audMan.audioDevice.minDistance + amb.audMan.audioDevice.maxDistance) * 0.5f); // defaultUnit + average audio distance

					var posRef = Singleton<CoreGameManager>.Instance.GetPlayer(Random.Range(0, Singleton<CoreGameManager>.Instance.setPlayers)).transform.position;
					amb.pos.x = posRef.x + (Random.value >= 0.5f ? -ambFac : ambFac);
					amb.pos.z = posRef.z + (Random.value >= 0.5f ? -ambFac : ambFac);

				})
				)
			.InstructionEnumeration();
	}
}
