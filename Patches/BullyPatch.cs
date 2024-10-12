using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(Bully))]
	internal class BullyPatch
	{
		[HarmonyPatch("SentToDetention")]
		[HarmonyPostfix]
		private static void NotGetDetention(ref SpriteRenderer ___spriteToHide)
		{
			if (Plugin.enableBullyGettingDetention.Value)
				___spriteToHide.enabled = true; // yeah, he exists, but uuh not hidden now
		}

		[HarmonyPatch("StealItem", [typeof(PlayerManager)])]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> TendToGetItemInHand(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(false, 
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(Bully), "slotsToSteal")),
				new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(List<int>), "Count")),
				new(OpCodes.Ldc_I4_0)
				)
			.InsertAndAdvance(
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(Bully), "slotsToSteal")),
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(Bully), "itemsToReject")),
				new(OpCodes.Ldarg_1),
				new(Transpilers.EmitDelegate((List<int> slots, List<Items> itmsToIgnore, PlayerManager pm) =>
				{
					if (itmsToIgnore.Contains(pm.itm.items[pm.itm.selectedItem].itemType) || Random.value * 100f > Plugin.bullyItemInHandTendency.Value)
						return;

					for (int i = 0; i < slots.Count; i++)
					{
						if (slots[i] != pm.itm.selectedItem)
							slots.RemoveAt(i--);
					}
				}))
				)
			.InstructionEnumeration();

	}
}
