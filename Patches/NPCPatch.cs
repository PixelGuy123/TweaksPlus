using HarmonyLib;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

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
				yield return null;
				var detention = new NavigationState_DetentionState(__instance, 99, __instance.ec.CellFromPosition(__instance.transform.position).room);
				__instance.behaviorStateMachine.ChangeNavigationState(detention);
				float cooldown = 15f;
				while (cooldown > 0f)
				{
					cooldown -= __instance.ec.EnvironmentTimeScale * Time.deltaTime;
					yield return null;
				}

				detention.End();
				yield break;
			}

			__instance.Navigator.Entity.StartCoroutine(WaitToLeave());
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
			npc.behaviorStateMachine.RestoreNavigationState();
		}
	}
}
