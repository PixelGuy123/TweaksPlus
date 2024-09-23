using HarmonyLib;
using TweaksPlus.Comps;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch]
    class ChalkFacePatch
    {
		[HarmonyPatch(typeof(ChalkFace), "Initialize")]
		[HarmonyPrefix]
		static void AddComponent(ChalkFace __instance) =>
			__instance.gameObject.AddComponent<ChalkFaceTimerFix>().chalkles = __instance;

		[HarmonyPatch(typeof(ChalkFace_Idle), "Enter")]
		[HarmonyPostfix]
		static void ActuallyCancelMaBoy(ChalkFace ___chalkles)
		{
			if (Plugin.enableChalklesInstaDisable.Value)
				___chalkles.charge = 0; // Literally resets it

			___chalkles.GetComponent<ChalkFaceTimerFix>().SetTimerFix(false);
		}

		[HarmonyPatch(typeof(ChalkFace_Charging), "Enter")]
		[HarmonyPrefix]
		static void AdaptTimer(ChalkFace ___chalkles, Chalkboard ___chalkboard)
		{
			if (Plugin.enableChalklesProportionalSpawn.Value)
			{
				float prevTimer = ___chalkles.setTime;
				___chalkles.setTime = Mathf.Max(15f, Mathf.Sqrt((___chalkboard.Room.size.x ^ 2) + (___chalkboard.Room.size.z ^ 2)) * Plugin.chalklesSizeFactor.value);
				if (___chalkles.charge > 0f)
					___chalkles.charge -= prevTimer - ___chalkles.setTime;

				___chalkles.GetComponent<ChalkFaceTimerFix>().SetTimerFix(true);
			}
			
		}

		[HarmonyPatch(typeof(ChalkFace_Laughing), "Enter")]
		[HarmonyPrefix]
		static void DisableTimerFix(ChalkFace ___chalkles) =>
			___chalkles.GetComponent<ChalkFaceTimerFix>().SetTimerFix(false);

	}
}
