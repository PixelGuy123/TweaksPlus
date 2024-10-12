using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TweaksPlus.Patches
{
	[HarmonyPatch]
	internal class AdditionalCullingPatches
	{
		[HarmonyPatch(typeof(EnvironmentController), "SetupDoor")]
		[HarmonyPostfix]
		static void AddRenderingForDoors(Door door)
		{
			if (!Plugin.enableAdditionalCulling.Value) return;

			if (door is StandardDoor stdDoor)
			{
				stdDoor.aTile.AddRenderer(stdDoor.doors[0]);
				stdDoor.bTile.AddRenderer(stdDoor.doors[1]);
				return;
			}
			if (door is SwingDoor swDoor)
			{
				swDoor.aTile.AddRenderer(swDoor.doors[0]);
				swDoor.bTile.AddRenderer(swDoor.doors[1]);
			}
		}

		[HarmonyPatch(typeof(Window), "Initialize")]
		[HarmonyPostfix]
		static void CullWindows(Window __instance)
		{
			if (!Plugin.enableAdditionalCulling.Value) return;

			__instance.aTile.AddRenderer(__instance.windows[0]);
			__instance.bTile.AddRenderer(__instance.windows[1]);
		}

		[HarmonyPatch(typeof(SwingDoorBuilder), "Build")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> GetHallSwingingDoors(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i).MatchForward(false,
				new(OpCodes.Ldloc_S, name:"V_7"),
				new(CodeInstruction.StoreField(typeof(TileBasedObject), "direction"))
				)
			.InsertAndAdvance(
				new(OpCodes.Dup),
				Transpilers.EmitDelegate((SwingDoor swDoor) => 
				{
					if (!Plugin.enableAdditionalCulling.Value) return;
					swDoor.aTile.AddRenderer(swDoor.doors[0]);
					swDoor.bTile.AddRenderer(swDoor.doors[1]);
				}))
			.InstructionEnumeration();
	}
}
