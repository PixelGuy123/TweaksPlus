using HarmonyLib;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(Map))]
	internal static class MapFillPatch
	{
		//	[HarmonyPatch("Find", [typeof(int), typeof(int), typeof(int), typeof(RoomController)])]
		//	[HarmonyReversePatch(HarmonyReversePatchType.Original)]
		//	static void Find(int posX, int posZ, int bin, RoomController room) =>
		//		throw new System.NotImplementedException("stub");

		static void Find(ref MapTile[,] tiles, ref bool[,] foundTiles, Sprite[] mapTileSprite, int posX, int posZ, int bin, RoomController room) // Workaround because reverse patch throws a crash
		{
			if (tiles[posX, posZ] != null)
			{
				tiles[posX, posZ].SpriteRenderer.sprite = mapTileSprite[bin];
				tiles[posX, posZ].SpriteRenderer.color = room.color;
				if (room.mapMaterial != null)
				{
					tiles[posX, posZ].SpriteRenderer.sharedMaterial = room.mapMaterial;
				}
				tiles[posX, posZ].Reveal();
				foundTiles[posX, posZ] = true;
			}
		}

		[HarmonyPatch("Find", [typeof(int), typeof(int), typeof(int), typeof(RoomController)])]
		[HarmonyPrefix]
		static bool FillupRoomIfNeeded(int posX, int posZ, RoomController room, ref MapTile[,] ___tiles, ref bool[,] ___foundTiles, Sprite[] ___mapTileSprite)
		{
			if (!Plugin.enableAutoMapFillCheck.Value || room.ec.mainHall == room || (___tiles[posX, posZ]?.Found ?? true))
				return true;

			for (int i = 0; i < room.TileCount; i++)
			{
				var cell = room.TileAtIndex(i);
				Find(ref ___tiles, ref ___foundTiles, ___mapTileSprite, cell.position.x, cell.position.z, cell.ConstBin, room);
			}
			return false;
		}
	}
}
