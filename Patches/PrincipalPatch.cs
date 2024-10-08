﻿using HarmonyLib;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch]
	internal class PrincipalPatch
	{
		[HarmonyPatch(typeof(Principal_ChasingNpc), "OnStateTriggerStay")]
		[HarmonyPrefix]
		static bool ActualNPCDetention(Collider other, ref NPC ___targetedNpc, ref Principal ___principal)
		{
			if (!Plugin.enablePrincipalNPCLecture.Value)
				return true;

			if (other.transform == ___targetedNpc.transform)
			{
				int num = Random.Range(0, ___principal.ec.offices.Count); // Stuff from the method itself
				___targetedNpc.Navigator.Entity.Teleport(___principal.ec.offices[num].RandomEntitySafeCellNoGarbage().FloorWorldPosition);
				___targetedNpc.SentToDetention();
				___targetedNpc.ClearGuilt();

				// Actual detention below
				var scolds = ___principal.audScolds; //(SoundObject[])audScolds.GetValue(___principal);
				var times = ___principal.audTimes; // (SoundObject[])audTimes.GetValue(___principal);
				var detention = ___principal.audDetention; //(SoundObject)audDetention.GetValue(___principal);

				___principal.Navigator.Entity.Teleport(___principal.ec.offices[num].RandomEntitySafeCellNoGarbage().FloorWorldPosition);
				var man = ___principal.audMan; //(AudioManager)audMan.GetValue(___principal);
				man.QueueAudio(times[0]);
				man.QueueAudio(detention);
				man.QueueAudio(scolds[Random.Range(0, scolds.Length)]);
				___principal.behaviorStateMachine.ChangeState(new Principal_Detention(___principal, 3f));

			}
			return false;
		}
	}
}
