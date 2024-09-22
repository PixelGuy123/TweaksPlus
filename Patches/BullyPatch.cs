using HarmonyLib;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(Bully), "SentToDetention")]
	internal class BullyPatch
	{
		private static void Postfix(ref SpriteRenderer ___spriteToHide)
		{
			if (Plugin.enableBullyGettingDetention.Value)
				___spriteToHide.enabled = true; // yeah, he exists, but uuh not hidden now
		}

	}
}
