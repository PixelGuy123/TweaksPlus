using HarmonyLib;
using TweaksPlus;

[HarmonyPatch(typeof(Beans))]
internal class BeansPatches
{
	[HarmonyPatch("HitNPC")]
	[HarmonyPatch("HitPlayer")]
	private static void Prefix(Beans __instance)
	{
		if (Plugin.enableBeansBullying.Value)
			__instance.SetGuilt(4f, "Bullying"); // Yeah
	}
}