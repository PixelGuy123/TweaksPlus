using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(NPC))]
	internal class NPCPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("SentToDetention")]
		private static void StuckInDetention(NPC __instance)
		{
			if (!Plugin.enableNPCActualDetention.Value)
				return;

			IEnumerator WaitToLeave()
			{
				__instance.Navigator.Entity.SetFrozen(true);
				float cooldown = 15f;
				while (cooldown > 0f)
				{
					cooldown -= __instance.ec.EnvironmentTimeScale * Time.deltaTime;
					yield return null;
				}

				__instance.Navigator.Entity.SetFrozen(false);
				yield break;
			}

			__instance.Navigator.Entity.StartCoroutine(WaitToLeave());
		}
	}
}
