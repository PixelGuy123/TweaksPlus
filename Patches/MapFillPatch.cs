using HarmonyLib;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(Map))]
	internal static class MapFillPatch
	{
		[HarmonyPatch("Find", [typeof(int), typeof(int), typeof(int), typeof(RoomController)])]
		[HarmonyReversePatch(HarmonyReversePatchType.Original)]
		static void Find(object instance, int posX, int posZ, int bin, RoomController room) =>
			throw new System.NotImplementedException("stub");

		[HarmonyPatch("Find", [typeof(int), typeof(int), typeof(int), typeof(RoomController)])]
		[HarmonyPrefix]
		static bool FillupRoomIfNeeded(Map __instance, int posX, int posZ, RoomController room, ref MapTile[,] ___tiles, ref bool[,] ___foundTiles, Sprite[] ___mapTileSprite)
		{
			if (!Plugin.enableAutoMapFillCheck.Value || room.ec.mainHall == room || (___tiles[posX, posZ]?.Found ?? true))
				return true;

			for (int i = 0; i < room.TileCount; i++)
			{
				var cell = room.TileAtIndex(i);
				Find(__instance, cell.position.x, cell.position.z, cell.ConstBin, room);
			}
			return false;
		}
	}
}
