using HarmonyLib;
using CampfireFrenzy;
using System;
using PicnicPanic;

namespace TweaksPlus.Patches
{
	[HarmonyPatch]
	internal static class FieldTripCheatModeBecauseWhyNot
	{
		[HarmonyPatch(typeof(Minigame_Campfire), "Launch")]
		[HarmonyReversePatch]
		static void LaunchAuto(object instance) =>
			throw new NotImplementedException("haha stub");

		[HarmonyPatch(typeof(Minigame_Campfire), "Land", [typeof(FuelGroup)])]
		[HarmonyPostfix]
		static void AutoToss(Minigame_Campfire __instance, FuelGroup group)
		{
			if (Plugin.enableFieldTripCheatMode.Value && group.LandedFuelIsGood)
				LaunchAuto(__instance);
		}

		[HarmonyPatch(typeof(PlateController), "Open")]
		[HarmonyPostfix]
		static void RemoveUnwantedStuff(PlateController __instance, bool ___valid)
		{
			if (Plugin.enableFieldTripCheatMode.Value && (!___valid || __instance.IsBomb))
				__instance.Pressed();
		}
	}
}
