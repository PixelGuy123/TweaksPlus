using HarmonyLib;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(MapIcon), "UpdatePosition", [typeof(Map)])]
    class MapIconPatch
    {
		static void Postfix(MapIcon __instance, ref Transform ___transform)
		{
			if (Plugin.enableNullMapTileFix.Value && __instance is NoLateIcon && !___transform.parent.gameObject.activeSelf) // A simple fix that disconnects the icon from a potential null tile
				__instance.transform.SetParent(null, true);
		}
    }
}
