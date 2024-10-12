using HarmonyLib;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(PlaceholderWinManager), "BeginPlay")]
	internal class PlaceholderWinManagerPatch
	{
		static void Postfix(MovementModifier ___moveMod)
		{
			if (!Plugin.enableFreeWinMovement.Value) return;
			Singleton<CoreGameManager>.Instance.GetPlayer(0).Am.moveMods.Remove(___moveMod);
			Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.Disable(false);
		}
	}
}
