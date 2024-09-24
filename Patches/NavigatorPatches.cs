using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(Navigator))]
	internal static class NavigatorPatches
	{
		[HarmonyPatch("UpdatePotentialOpenExits")]
		[HarmonyPatch("GetFurthestPotentialExits")]
		[HarmonyPostfix]
		static void AvoidWhileWanderingOpen(ref List<OpenGroupExit> ____potentialExits, EnvironmentController ___ec)
		{
			if (!Plugin.enableNavigatorTargettingImprovement.Value) return;

			for (int i = 0; i < ____potentialExits.Count; i++)
				if (CheckCell(____potentialExits[i].cell) && CheckCell(____potentialExits[i].OutputCell(___ec)))
						____potentialExits.RemoveAt(i--);
			

			static bool CheckCell(Cell cell) => cell.room.type != RoomType.Hall && !cell.doorHere && !cell.room.entitySafeCells.Contains(cell.position);
		}

		[HarmonyPatch("WanderFlee")]
		[HarmonyPatch("WanderRandom")]
		[HarmonyPatch("WanderRounds")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> AvoidWhileWandering(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(false, [new(CodeInstruction.Call(typeof(Directions), "FillOpenDirectionsFromBin"))])
			.Advance(1)
			.InsertAndAdvance(
				new(OpCodes.Ldarg_0),
				CodeInstruction.LoadField(typeof(Navigator), "_potentialDirs"),
				new(OpCodes.Ldarg_0),
				CodeInstruction.LoadField(typeof(Navigator), "currentStartTile"),
				new(OpCodes.Ldarg_0),
				CodeInstruction.LoadField(typeof(Navigator), "ec"),
				new(OpCodes.Ldarg_0),
				CodeInstruction.LoadField(typeof(Navigator), "_gridPosition"),
				Transpilers.EmitDelegate((List<Direction> dirs, Cell startCell, EnvironmentController ec, IntVector2 pos) => // Basically clean up the potentialDirs after FillOpenDirectionsFromBin
				{
					if (!Plugin.enableNavigatorTargettingImprovement.Value || startCell.open) return;

					for (int i = 0; i < dirs.Count; i++)
					{
						var cell = ec.CellFromPosition(pos + dirs[i].ToIntVector2());
						if (cell.room.type != RoomType.Hall && !cell.doorHere && !cell.room.entitySafeCells.Contains(cell.position))
							dirs.RemoveAt(i--);
					}

				})
				)
			
			.InstructionEnumeration();

	}
}
