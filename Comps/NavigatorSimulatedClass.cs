using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;

namespace TweaksPlus.Comps
{
	public static class NavigatorSimulatedClass
	{
		private static Cell _startTile;
		private static Cell _targetTile;
		static List<Cell> _path;
		static NavMeshHit _startHit, _targetHit;
		static NavMeshPath _navMeshPath = null;

		public static List<Vector3> FindNavPath(this EnvironmentController ec, Transform caller, Vector3 startPos, Vector3 targetPos)
		{
			_navMeshPath ??= new();

			List<Vector3> destinationPoints = [];
			_startTile = ec.CellFromPosition(startPos);
			_targetTile = ec.CellFromPosition(targetPos);

			ec.FindPathWithExtraAttributes(_startTile, _targetTile, PathType.Nav, out _path, out bool flag);
			if (flag)
				ConvertPath(ec, _path, destinationPoints, targetPos, caller);



			return destinationPoints;
		}


		static void ConvertPath(EnvironmentController ec, List<Cell> path, List<Vector3> destinationPoints, Vector3 targetPos, Transform transform)
		{
			Cell cell = null, cell2 = null;
			
			bool flag = false;
			destinationPoints.Clear();

			while (path.Count > 0)
			{
				if (path[0].open)
				{
					if (flag)
					{
						if (path[0].openTiles.Contains(cell2))
						{
							cell2 = path[0];
						}
						else
						{
							ec.BuildNavPath(cell, cell2, targetPos, transform, destinationPoints);
							cell = path[0];
							cell2 = path[0];
						}
					}
					else
					{
						flag = true;
						cell = path[0];
						cell2 = path[0];
					}
					if (path.Count == 1)
					{
						ec.BuildNavPath(cell, cell2, targetPos, transform, destinationPoints);
					}
				}
				else
				{
					if (flag)
					{
						flag = false;
						ec.BuildNavPath(cell, cell2, targetPos, transform, destinationPoints);
					}
					destinationPoints.Add(path[0].FloorWorldPosition);
				}
				path.RemoveAt(0);
			}
			if (destinationPoints.Count > 1 && (destinationPoints[1] - transform.position).magnitude < (destinationPoints[1] - destinationPoints[0]).magnitude)
			{
				destinationPoints.RemoveAt(0);
			}
			//if (preciseTarget)
			//{
			//	destinationPoints.Add(Vector3.right * targetPos.x + Vector3.forward * targetPos.z);
			//}
			if (destinationPoints.Count > 2 && (destinationPoints[destinationPoints.Count - 3] - destinationPoints[destinationPoints.Count - 1]).magnitude < (destinationPoints[destinationPoints.Count - 3] - destinationPoints[destinationPoints.Count - 2]).magnitude)
			{
				destinationPoints.RemoveAt(destinationPoints.Count - 2);
			}
		}

		static void BuildNavPath(this EnvironmentController ec, Cell firstOpenTile, Cell lastOpenTile, Vector3 targetPosition, Transform transform, List<Vector3> destinationPoints)
		{
			if (ec.CellFromPosition(transform.position) != firstOpenTile || !NavMesh.SamplePosition(transform.position.ZeroOutY(), out _, 1f, -1))
			{
				NavMesh.SamplePosition(firstOpenTile.FloorWorldPosition, out _startHit, 10f, -1);
			}
			else
			{
				NavMesh.SamplePosition(transform.position.ZeroOutY(), out _startHit, 10f, -1);
			}
			if (ec.CellFromPosition(targetPosition) != lastOpenTile || !NavMesh.SamplePosition(targetPosition.ZeroOutY(), out _, 1f, -1))
			{
				NavMesh.SamplePosition(lastOpenTile.FloorWorldPosition, out _targetHit, 10f, -1);
			}
			else
			{
				NavMesh.SamplePosition(targetPosition.ZeroOutY(), out _targetHit, 10f, -1);
			}
			NavMesh.CalculatePath(_startHit.position, _targetHit.position, -1, _navMeshPath);
			foreach (Vector3 vector in _navMeshPath.corners)
			{
				destinationPoints.Add(Vector3.zero + Vector3.right * vector.x + Vector3.forward * vector.z);
			}
		}

	}

	[HarmonyPatch(typeof(EnvironmentController))]
	internal static class ModifiedFindPath_Data{
		readonly internal static List<VentController> vents = [];
		[HarmonyPatch("BeginPlay")]
		[HarmonyPrefix]
		static void GetAllVentsAvailable()
		{
			vents.Clear();
			vents.AddRange(Object.FindObjectsOfType<VentController>());
		}
	}

	[HarmonyPatch(typeof(EnvironmentController))]
	public static class ModifiedFindPath
	{
		[HarmonyTargetMethod]
		static MethodInfo GetEnvironmentControllerFindPath() =>
			AccessTools.Method(
						typeof(EnvironmentController), "FindPath", 
						[
							typeof(Cell),
							typeof(Cell),
							typeof(PathType),
							typeof(List<Cell>).MakeByRefType(),
							typeof(bool).MakeByRefType()
						]);

		[HarmonyPatch]
		[HarmonyReversePatch(HarmonyReversePatchType.Original)]
		public static void FindPathWithExtraAttributes(this EnvironmentController instance, Cell startTile, Cell targetTile, PathType pathType, out List<Cell> path, out bool success)
		{
			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> i) =>
				new CodeMatcher(i) // Make a transpiler that imitates the GetNavNeighbor, but insert an extra parameter which is the EnvironmentController instance
				.MatchForward(false,
				new CodeMatch(CodeInstruction.Call(typeof(EnvironmentController), "GetNavNeighbors"))
				)
				.SetInstruction(Transpilers.EmitDelegate((Cell tile, List<Cell> list, PathType pathType, EnvironmentController ec) =>
				{
					list.Clear();
					for (int i = 0; i < 4; i++)
					{
						bool validCell = ec.ValidCell(tile, out tile);
						if (validCell)
						{
							list.Add(tile);
							continue;
						}

						if (tile.Navigable(Directions.FromInt(i), pathType) && ec.ContainsCoordinates(tile.position + Directions.Vectors[i]) && (tile.Null || !ec.CellFromPosition(tile.position + Directions.Vectors[i]).Null))
						{
							list.Add(ec.CellFromPosition(tile.position + Directions.Vectors[i]));
						}
					}
				}))
				.Insert(new CodeInstruction(OpCodes.Ldarg_0))

				.MatchForward(false,
				new CodeMatch(CodeInstruction.Call(typeof(EnvironmentController), "GetDistance", [typeof(Cell), typeof(Cell)])))
				.SetInstruction(CodeInstruction.Call(typeof(ModifiedFindPath), "GetDistance")) // Changes the distance for the one from here

				.MatchForward(false,
				new CodeMatch(CodeInstruction.Call(typeof(EnvironmentController), "GetDistance", [typeof(Cell), typeof(Cell)])))
				.SetInstruction(CodeInstruction.Call(typeof(ModifiedFindPath), "GetDistance")) // Changes the distance for the one from here
				.InstructionEnumeration();

			var _ = Transpiler;

			throw new System.NotImplementedException("stub");
		}

		public static int GetDistance(EnvironmentController ec, Cell tileA, Cell tileB)
		{
			if (ModifiedFindPath_Data.vents.Count != 0)
			{
				int ventDistance = -1;
				VentController closestVent = null;

				for (int i = 0; i < ModifiedFindPath_Data.vents.Count; i++)
				{
					int foundDistance = ec.GetDistance(ec.CellFromPosition(ModifiedFindPath_Data.vents[i].startPosition), tileA);
					if (ventDistance == -1 || foundDistance < ventDistance)
					{
						closestVent = ModifiedFindPath_Data.vents[i];
						ventDistance = foundDistance;
					}
				}

				if (closestVent != null)
				{
					int tileDistance = ec.GetDistance(tileA, tileB);
					int ventExitDistance = ec.GetDistance(tileB, ec.CellFromPosition(closestVent.points[closestVent.points.Length - 1]));

					return ventExitDistance > tileDistance ? tileDistance : ventExitDistance;
				}
			}

			int num = Mathf.Abs(tileA.position.x - tileB.position.x);
			int num2 = Mathf.Abs(tileA.position.z - tileB.position.z);
			if (num > num2)
			{
				return 14 * num2 + 10 * (num - num2);
			}
			return 14 * num + 10 * (num2 - num);
		}

		static bool ValidCell(this EnvironmentController ec, Cell inputCell, out Cell cell)
		{
			cell = inputCell;
			var vent = inputCell.ObjectBase.GetComponentInChildren<VentController>();
			if (vent)
			{
				cell = ec.CellFromPosition(vent.points[vent.points.Length - 1]);
				return true;
			}

			return false;
		}
	}
}
