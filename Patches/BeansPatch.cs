using HarmonyLib;
using TweaksPlus;

[HarmonyPatch(typeof(Beans))]
internal class BeansPatches
{
	[HarmonyPatch("HitNPC")]
	[HarmonyPatch("HitPlayer")]
	[HarmonyPrefix]
	private static void BullyForSpit(Beans __instance)
	{
		if (Plugin.enableBeansBullying.Value)
			__instance.SetGuilt(4f, "Bullying"); // Yeah
	}

	[HarmonyPatch("Spit")]
	[HarmonyPrefix]
	static void GumSquished(Beans __instance, Gum ___gum)
	{
		if (!Plugin.enableGumVisualChange.Value) return;

		if (__instance.Navigator.Entity.Squished)
			___gum.entity.Squish(999f);
		else
			___gum.entity.Unsquish();
	}
}