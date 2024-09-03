using HarmonyLib;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(PlayerClick))]
	internal static class IItemAcceptorDisplay
	{
		[HarmonyPatch("Update")]
		static bool Prefix(PlayerClick __instance, PlayerManager ___pm, ref Ray ____ray, ref RaycastHit ___hit, ref float ___reach, ref LayerMask ___clickLayers, ref bool ___seesClickable)
		{
			if (!Plugin.enableItemAcceptorCheck.Value || ___pm.plm.Entity.InteractionDisabled ) return true;

			____ray.origin = __instance.transform.position;
			____ray.direction = Singleton<CoreGameManager>.Instance.GetCamera(___pm.playerNumber).transform.forward;

			if (!Physics.Raycast(____ray, out ___hit, ___reach, ___clickLayers))
				return true;
			

			var acc = ___hit.transform.GetComponent<IItemAcceptor>();
			if (acc != null && acc.ItemFits(___pm.itm.items[___pm.itm.selectedItem].itemType))
			{
				if (!___seesClickable)
					Singleton<CoreGameManager>.Instance.GetHud(___pm.playerNumber).UpdateReticle(true);
				___seesClickable = true;
				return false;
			}

			return true;
		}
	}
}
