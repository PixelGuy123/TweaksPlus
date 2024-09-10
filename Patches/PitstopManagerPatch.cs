using HarmonyLib;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(PitstopGameManager))]
	internal class PitstopManagerPatch
	{
		[HarmonyPatch("BeginPlay")]
		static void Postfix()
		{
			if (Plugin.enableItemUsageInPitstop.Value)
				Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.Disable(false); // No u
		}
	}
}
