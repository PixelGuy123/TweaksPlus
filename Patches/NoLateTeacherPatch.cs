using HarmonyLib;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(NoLateTeacher))]
	internal class NoLateTeacherPatch
	{
		[HarmonyPatch("AssignClassRoom", [typeof(PlayerManager)])]
		[HarmonyPostfix]
		static void AdaptTimer(ref float ___classTime, RoomController ___classRoom, DijkstraMap ___dijkstraMap, SoundObject[] ___audNumbers)
		{
			if (Plugin.enableMrsPompDynamicTimer.Value)
				___classTime = Mathf.Clamp(60 * Mathf.RoundToInt((___dijkstraMap.Value(___classRoom.doors[0].aTile.position) * Plugin.mrsPompTimerFactor.Value) / 60f), 60f, 60 * (___audNumbers.Length - 1));
		}
	}
}
