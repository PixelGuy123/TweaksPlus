using HarmonyLib;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(LockdownDoor), "Initialize")]
	internal class LockdownDoorPatch
	{
		static void Postfix(ref float ___speed)
		{
			if (Plugin.randomizedLockdownDoorSpeed.Value == 1f) return;

			___speed = UnityEngine.Random.Range(1f, Plugin.randomizedLockdownDoorSpeed.Value);
		}
	}
}
