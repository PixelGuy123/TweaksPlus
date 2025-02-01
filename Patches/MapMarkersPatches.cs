using HarmonyLib;
using MTM101BaldAPI.AssetTools;
using System.Collections.Generic;
using TweaksPlus.Comps;
using UnityEngine;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(MapMarker))]
	static class PathfindingMarker
	{
		[HarmonyPatch("Initialize")]
		[HarmonyPostfix]
		static void AddPathfind(MapMarker __instance, Map map, Renderer ___environmentMarker)
		{
			if (!Plugin.enableMarkersWithPathfinding.Value || map.Ec.CellFromPosition(___environmentMarker.transform.position).Null || !Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.Entity.InBounds)
				return;

			var path = map.Ec.FindNavPath(
				Singleton<CoreGameManager>.Instance.GetPlayer(0).transform,
				Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position,
				___environmentMarker.transform.position
				);

			if (path.Count == 0)
				return;

			var line = new GameObject("Line_" + __instance.name)
			{
				layer = LayerMask.NameToLayer("Overlay") // Make sure to be above everything
			};
			line.transform.SetParent(__instance.transform);
			line.transform.position = ___environmentMarker.transform.position;

			var lineRend = line.AddComponent<LineRenderer>(); // Makes a line renderer
			lineRend.enabled = ___environmentMarker.enabled;
			lineRend.widthMultiplier = 0.45f;
			lineRend.useWorldSpace = true;
			lineRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			lineRend.textureMode = LineTextureMode.Stretch;

			const float y = 0.1f;

			List<Vector3> positions = []; // set positions from path
			for (int i = 0; i < path.Count - 1; i++)
			{
				Vector3 pos = new(path[i].x, y, path[i].z);
				positions.Add(pos);

				if (map.Ec.CellFromPosition(pos).HasObjectBase)
				{
					var vent = map.Ec.CellFromPosition(pos).ObjectBase.GetComponentInChildren<VentController>();
					if (vent)
					{
						for (int z = 0; z < vent.points.Length; z++)
						{
							positions.Add(
								new
								(
									(vent.points[z].x * 10f) + 5f,
									vent.height - 0.95f,
									(vent.points[z].z * 10f) + 5f
								));
						}
						positions.Add(new((vent.points[vent.points.Length - 1].x * 10f) + 5f, y, (vent.points[vent.points.Length - 1].z * 10f) + 5f));
					}
				}
			}

			positions[positions.Count - 1] = new(___environmentMarker.transform.position.x, y, ___environmentMarker.transform.position.z);
			lineRend.positionCount = positions.Count; // position count

			lineRend.SetPositions([.. positions]);

			var sprite = ((SpriteRenderer)___environmentMarker).sprite;

			if (!colorPairs.TryGetValue(sprite, out var firstFoundColor)) // Colors are cached too
			{
				var tex = sprite.texture;

				if (!readableCopies.TryGetValue(sprite, out Texture2D readableTexture)) // If the texture wasn't made readable, it's automatically made readable and stored in memory
																						// to be re-used
				{
					Texture2D readableCopy = tex;
					if (!readableCopy.isReadable && !readableTextureCopies.TryGetValue(tex, out readableCopy))
					{
						readableCopy = tex.MakeReadableCopy(true);
						readableTextureCopies.Add(tex, readableCopy); // Make a readable texture
					}
					readableTexture = readableCopy.GetCroppedTexture(sprite.rect); // Get the actual texture out of the sprite that is rendering it
					readableCopies.Add(sprite, readableTexture); // Cache everything, so this process is done once
				}


				var colors = readableTexture.GetPixels(); // get an average color for the line
				firstFoundColor = Color.white;
				for (int i = 0; i < colors.Length; i++)
				{
					if (colors[i].a == 1f && colors[i] != Color.white)
					{
						firstFoundColor = colors[i]; // Can't take average because the colors are very inaccurate
						break;
					}
				}
				colorPairs.Add(sprite, firstFoundColor);
			}

			lineRend.material.shader = Shader.Find("Shader Graphs/Standard");
			lineRend.material.SetColor("_TextureColor", firstFoundColor);
		}

		[HarmonyPatch("ShowMarker")]
		[HarmonyPostfix]
		static void ShowPathfindLine(MapMarker __instance, bool enabled)
		{
			if (!Plugin.enableMarkersWithPathfinding.Value)
				return;

			var rend = __instance.GetComponentInChildren<LineRenderer>();
			if (rend)
				rend.enabled = enabled;
		}

		[HarmonyPatch(typeof(Map), "Update")]
		[HarmonyPrefix]
		static void CheckForDeletion(EnvironmentController ___ec, bool ___advancedMode, ref List<MapMarker> ___markers)
		{
			if (!Plugin.enableMarkerAutoDisable.Value || ___advancedMode) return;

			for (int i = 0; i < ___markers.Count; i++)
			{
				if (___ec.CellFromPosition(___markers[i].environmentMarker.transform.position) == ___ec.CellFromPosition(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position))
					___markers[i--].Delete();
			}
		}

		readonly static Dictionary<Sprite, Texture2D> readableCopies = [];
		readonly static Dictionary<Texture2D, Texture2D> readableTextureCopies = [];
		readonly static Dictionary<Sprite, Color> colorPairs = [];
#pragma warning disable Harmony003 // Harmony non-ref patch parameters modified
		static Texture2D GetCroppedTexture(this Texture2D texture, Rect spriteRect)
		{
			Texture2D croppedTexture = new((int)spriteRect.width, (int)spriteRect.height);


			Color[] pixels = texture.GetPixels((int)spriteRect.x, (int)spriteRect.y, (int)spriteRect.width, (int)spriteRect.height);
			croppedTexture.SetPixels(pixels);
			croppedTexture.Apply();

			return croppedTexture;
		}
#pragma warning restore Harmony003 // Harmony non-ref patch parameters modified
	}
}
