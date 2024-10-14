using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch]
	internal static class AdditionalCullingPatches
	{
		internal static void CreateRendererContainer(this Transform obj)
		{
			var parent = obj.parent;
			while (parent != null)
			{

#pragma warning disable Harmony003 // Harmony non-ref patch parameters modified
				obj = parent;
#pragma warning restore Harmony003 // Harmony non-ref patch parameters modified
				parent = obj.parent;
			}
			if (obj.GetComponent<RendererContainer>() || obj.transform is RectTransform || notAllowedTypes.Any(x => obj.GetComponentInChildren(x)))
				return;

			var rend = obj.gameObject.AddComponent<RendererContainer>();
			rend.renderers = obj.GetComponentsInChildren<Renderer>();
		}

		internal static List<Type> notAllowedTypes = [typeof(Pickup), typeof(Tile), typeof(Elevator)];

		[HarmonyPatch(typeof(CullingManager), "PrepareOcclusionCalculations")]
		[HarmonyPostfix]
		static void FindAllRendererContainers(EnvironmentController ___ec)
		{
			if (!Plugin.enableAdditionalCulling.Value) return;

			UnityEngine.Object.FindObjectsOfType<RendererContainer>().Do(x =>
			{
				var cell = ___ec.CellFromPosition(x.transform.position);
				if (!cell.Null)
				{
					x.renderers.Do(z =>
					{
						if (!cell.renderers.Contains(z))
							cell.AddRenderer(z);
					});
				}
			});
		}

		[HarmonyPatch(typeof(Chunk), "Render")]
		[HarmonyFinalizer]
		static Exception IfCrashesReviewStuff(Exception __exception, Chunk __instance, List<Renderer> ___renderers, List<Cell> ___cells, bool value)
		{
			if (__exception != null)
			{
				Plugin.LogWarning($"Chunk ({__instance.Id}) has detected an invalid renderer. Cleaning up invalid renderers...");
				___renderers.RemoveAll(x => !x);
				___cells.ForEach(cell => cell.renderers.RemoveAll(x => !x));
			}

			Render(__instance, value);

			return null;
		}

		[HarmonyPatch(typeof(Chunk), "Render")]
		[HarmonyReversePatch(HarmonyReversePatchType.Original)]
		static void Render(object instance, bool value) =>
			throw new NotImplementedException("stub");

		/*
		[HarmonyPatch(typeof(EnvironmentController), "SetupDoor")]
		[HarmonyPostfix]
		static void AddRenderingForDoors(Door door)
		{
			if (!Plugin.enableAdditionalCulling.Value) return;

			if (door is StandardDoor stdDoor)
			{
				stdDoor.aTile.AddRenderer(stdDoor.doors[0]);
				stdDoor.bTile.AddRenderer(stdDoor.doors[1]);
				return;
			}
			if (door is SwingDoor swDoor)
			{
				swDoor.aTile.AddRenderer(swDoor.doors[0]);
				swDoor.bTile.AddRenderer(swDoor.doors[1]);
			}
		}

		[HarmonyPatch(typeof(Window), "Initialize")]
		[HarmonyPostfix]
		static void CullWindows(Window __instance)
		{
			if (!Plugin.enableAdditionalCulling.Value) return;

			__instance.aTile.AddRenderer(__instance.windows[0]);
			__instance.bTile.AddRenderer(__instance.windows[1]);
		}

		[HarmonyPatch(typeof(SwingDoorBuilder), "Build")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> GetHallSwingingDoors(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i).MatchForward(false,
				new(OpCodes.Ldloc_S, name:"V_7"),
				new(CodeInstruction.StoreField(typeof(TileBasedObject), "direction"))
				)
			.InsertAndAdvance(
				new(OpCodes.Dup),
				Transpilers.EmitDelegate((SwingDoor swDoor) => 
				{
					if (!Plugin.enableAdditionalCulling.Value) return;
					swDoor.aTile.AddRenderer(swDoor.doors[0]);
					swDoor.bTile.AddRenderer(swDoor.doors[1]);
				}))
			.InstructionEnumeration();

		[HarmonyPatch(typeof(BeltBuilder), "Build", [typeof(EnvironmentController), typeof(List<Cell>), typeof(Direction), typeof(BeltManager)])]
		[HarmonyPostfix]
		static void CullConveyor(EnvironmentController ec, BeltManager beltManager) =>
			beltManager.belts.ForEach(x => ec.CellFromPosition(x.transform.position).AddRenderer(x));

		[HarmonyPatch(typeof(GameButton), nameof(GameButton.BuildInArea))]
		[HarmonyPostfix]
		static void CullButtons(GameButton __result, EnvironmentController ec)
		{
			var cell = ec.CellFromPosition(__result.transform.position);
			__result.GetComponentsInChildren<Renderer>().Do(cell.AddRenderer);
		}
		*/

	}
}
