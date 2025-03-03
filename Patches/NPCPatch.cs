using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(NPC))]
	internal class NPCPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("SetGuilt")]
		static bool ReallyNeedsGuilt(NPC __instance) =>
			!__instance.ec.CellFromPosition(__instance.transform.position).room.
			functions.functions.Exists(x => x is RuleFreeZone); // If there is a rule free zone, no guilt needs to be set (to avoid Principal's instant reaction)

		[HarmonyPrefix]
		[HarmonyPatch("SentToDetention")]
		private static void StuckInDetention(NPC __instance)
		{
			if (!Plugin.enableNPCActualDetention.Value)
				return;

			IEnumerator WaitToLeave()
			{
				EnvironmentController ec = __instance.ec;
				yield return null;
				var detention = new NavigationState_DetentionState(__instance, 99, ec.CellFromPosition(__instance.transform.position).room);
				__instance.navigationStateMachine.ChangeState(detention);
				float cooldown = 15f;

				while (cooldown > 0f)
				{
					cooldown -= ec.EnvironmentTimeScale * Time.deltaTime;
					yield return null;
				}

				detention.End();
				yield break;
			}

			if (__instance.Navigator.isActiveAndEnabled)
				__instance.ec.StartCoroutine(WaitToLeave()); // The EnvironmentController can't just be disabled randomly... right?
		}
	}

	internal class NavigationState_DetentionState(NPC npc, int priority, RoomController office) : NavigationState(npc, priority)
	{
		readonly RoomController office = office;
		public override void Enter()
		{
			base.Enter();
			npc.Navigator.FindPath(office.RandomEventSafeCellNoGarbage().FloorWorldPosition);
			
		}

		public override void DestinationEmpty()
		{
			base.DestinationEmpty();
			npc.Navigator.FindPath(office.RandomEventSafeCellNoGarbage().FloorWorldPosition);
		}

		public void End()
		{
			priority = 0;
			npc?.behaviorStateMachine.RestoreNavigationState();
		}
	}
}
