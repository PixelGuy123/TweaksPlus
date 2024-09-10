using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TweaksPlus.Patches
{
	//[HarmonyPatch(typeof(MainMenu))]
	//internal class Fun
	//{
	//	[HarmonyPatch("OnEnable")]
	//	[HarmonyPrefix]
	//	static bool DoIt(ref bool ___yes, ref float ___timeToDoIt)
	//	{
	//		___yes = true;
	//		___timeToDoIt = 97;
	//		return false;
	//	}

	//	[HarmonyPatch("Update")]
	//	[HarmonyPrefix]
	//	static void NoTracker(ref Vector2 ___lastCursorPosition) =>
	//		___lastCursorPosition = CursorController.Instance.position;

	//	[HarmonyPatch("DoIt")]
	//	[HarmonyTranspiler]
	//	static IEnumerable<CodeInstruction> DoItNow(IEnumerable<CodeInstruction> i) =>
	//		new CodeMatcher(i)
	//		.MatchForward(false, 
	//			[new(OpCodes.Ldc_R4, 99f)]
	//			)
	//		.Set(OpCodes.Ldc_R4, 1f)
	//		.InstructionEnumeration();

	//	[HarmonyPatch("Start")]
	//	[HarmonyPrefix]
	//	static void GetMyConsoler()
	//	{
	//		var co = Resources.FindObjectsOfTypeAll<Console>()[0];
	//		Debug.LogWarning("INIATING CONSOLE DATA EXTRACTION:\n");
	//		foreach (var line in co.lineData)
	//		{
	//			Debug.Log("-------------------------------");
	//			Debug.Log($"Line Color: {line.color}");
	//			Debug.Log($"Line type: {line.textType}\n--");
	//			Debug.Log(line.text);
	//		}
	//	}
	//}
}
