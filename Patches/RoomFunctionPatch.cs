using HarmonyLib;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(RoomFunction))]
    class RoomFunctionPatch
    {
		[HarmonyPatch("OnNpcStay")]
		[HarmonyPrefix]
		static void RuleFreeZoneControl(RoomFunction __instance, NPC npc)
		{
			if (Plugin.enableRuleFreeZoneForNPCs.Value && __instance is RuleFreeZone)
				npc.ClearGuilt();
		}
    }
}
