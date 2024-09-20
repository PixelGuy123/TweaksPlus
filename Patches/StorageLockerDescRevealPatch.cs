using HarmonyLib;

namespace TweaksPlus.Patches
{
	[HarmonyPatch]
	internal class StorageLockerDescRevealPatch
	{
		[HarmonyPatch(typeof(StorageLocker), "Start")]
		[HarmonyPrefix]
		static void AllDescs(ref Pickup[] ___pickup)
		{
			if (Plugin.enableItemDescRevealInStorageLocker.Value)
				___pickup.Do(x => x.showDescription = true);
		}

		[HarmonyPatch(typeof(Pickup), "ClickableSighted")]
		[HarmonyPrefix]
		static bool OnlyShowIfNotNothing(Pickup __instance) =>
			!Plugin.enableItemDescRevealInStorageLocker.Value || (__instance.item && __instance.item.itemType != Items.None);
	}
}
