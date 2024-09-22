using HarmonyLib;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch]
    class ChalkFacePatch
    {
		[HarmonyPatch(typeof(ChalkFace_Idle), "Enter")]
		[HarmonyPostfix]
		static void ActuallyCancelMaBoy(ChalkFace ___chalkles)
		{
			if (Plugin.enableChalklesInstaDisable.Value)
				___chalkles.charge = 0; // Literally resets it
		}

		[HarmonyPatch(typeof(ChalkFace_Charging), "Enter")]
		[HarmonyPostfix]
		static void AdaptTimer(ChalkFace ___chalkles, Chalkboard ___chalkboard) =>
			___chalkles.setTime = Mathf.Max(1f, Mathf.Sqrt((___chalkboard.Room.size.x ^2) + (___chalkboard.Room.size.z ^2)) * 1.65f);
		
    }
}
