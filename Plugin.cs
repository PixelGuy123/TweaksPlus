using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using MTM101BaldAPI.Registers;
using System.Collections;
using System.Collections.Generic;
using TweaksPlus.Patches;
using UnityEngine;
using MTM101BaldAPI;

namespace TweaksPlus
{
	[BepInPlugin(guid, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	[BepInDependency("mtm101.rulerp.bbplus.baldidevapi", BepInDependency.DependencyFlags.HardDependency)]
	public class Plugin : BaseUnityPlugin
	{
		public const string guid = "pixelguy.pixelmodding.baldiplus.tweaksplus";

		internal static ConfigEntry<bool> enableAutoMapFillCheck, enableChalklesInstaDisable, enableChalklesProportionalSpawn,
			enableItemUsageInPitstop, enableItemDescRevealInStorageLocker, enableMrsPompDynamicTimer, enableNavigatorTargettingImprovement,
			enableHappyBaldiFix, enableNegativeUniqueness, enablePlaytimeBullying, enablePrincipalNPCLecture, enableBullyGettingDetention,
			enableNPCActualDetention, enableRuleFreeZoneForNPCs, enableNullMapTileFix, enableBeansBullying, enableAdditionalCulling, enableFreeWinMovement, enableProportionalYTPAdder,
			enableGumVisualChange, enableMarkersWithPathfinding, enableMarkerAutoDisable, enableNonSquaryLayouts;

		internal static ConfigEntry<bool> enableFieldTripCheatMode;

		internal static ConfigEntry<bool> enableNPCWeightBalance, enableItemWeightBalance, enableRandomEventWeightBalance, enableDebugLogs;

		internal static ConfigEntry<float> mrsPompTimerFactor, chalklesSizeFactor, bullyItemInHandTendency, ambienceDistancingFactor, randomizedLockdownDoorSpeed;
		internal static Plugin plug;
#pragma warning disable IDE0051 // Remover membros privados não utilizados
		private void Awake()
#pragma warning restore IDE0051 // Remover membros privados não utilizados
		{
			plug = this;
			Harmony h = new("pixelguy.pixelmodding.baldiplus.tweaksplus");
			h.PatchAllConditionals();

			enableAutoMapFillCheck = Config.Bind(mainSec, "Enable map forced fillup", true, "If True, for any room that you enter, all the map tiles will be forcefully revealed, regardless of their shape.");
			enableChalklesInstaDisable = Config.Bind(mainSec, "Enable Chalkles instant disable", true, "If True, the moment you leave a room with Chalkles will instantly reset him (like it used to be before v0.4).");

			enableChalklesProportionalSpawn = Config.Bind(mainSec, "Enable Chalkles proportional spawn", true, "If True, Chalkles\' spawn time will be proportional to the estimated size of the room it is currently in.");
			chalklesSizeFactor = Config.Bind(mainSec, "Chalkles charge factor", 1.65f, "Determines how long will take for Chalkles to charge by getting the magnitude of the size of the room multiplied by this constant/factor.");

			enableItemUsageInPitstop = Config.Bind(mainSec, "Enable item usage in the Pitstop", true, "If True, the player will be able of using any item from their inventory in the PitStop (feature implemented since 0.7)");
			enableItemDescRevealInStorageLocker = Config.Bind(mainSec, "Enable item description in storage locker", true, "If True, the item description will be displayed in a Storage Locker.");

			enableMrsPompDynamicTimer = Config.Bind(mainSec, "Enable Mrs Pomp dynamic timer", true, "If True, Mrs Pomp\'s timer will be proportional to how far away you are from the classroom.");
			mrsPompTimerFactor = Config.Bind(mainSec, "Mrs Pomp distance factor", 3.6f, "Determines how long will be the timer by the distance multiplied by this constant/factor.");

			enableNavigatorTargettingImprovement = Config.Bind(mainSec, "Enable Navigator improvement", true, "If True, the npc navigators will always avoid non-safe entity cells.");
			enableHappyBaldiFix = Config.Bind(mainSec, "Enable Happy Baldi fix", true, "If True, Happy Baldi no longer crashes if Baldi is missing.");
			enablePlaytimeBullying = Config.Bind(mainSec, "Enable Playtime bullying", true, "If True, cutting Playtime\'s rope will count as bullying for five seconds.");
			enablePrincipalNPCLecture = Config.Bind(mainSec, "Enable Principal lecture on NPCs", true, "If True, Principal will always give lecture to NPCs after giving them *actual* detention.");
			enableBullyGettingDetention = Config.Bind(mainSec, "Enable Bully detention", true, "If True, Bully will have actual detention when caught.");
			enableNPCActualDetention = Config.Bind(mainSec, "Enable NPC actual detention", true, "If True, NPCs will always be stuck in the office when sent to detention.");
			enableRuleFreeZoneForNPCs = Config.Bind(mainSec, "Enable rule free zone for npcs", true, "If True, NPCs will have their guilt cleared in Rule Free zones (like Playground).");

			enableNullMapTileFix = Config.Bind(mainSec, "Null map tile fix", true, "If True, Mrs Pomp\'s icon will no longer be invisible if the center of the room is an empty tile.");
			enableBeansBullying = Config.Bind(mainSec, "Beans bullying", true, "If True, Beans will be breaking the bullying rule for spitting gums and hitting them at somebody.");
			enableAdditionalCulling = Config.Bind(mainSec, "Additional culling", true, "If True, basically everything that is visible will be properly culled by the game (can be dangerous if they change places by another mod).");
			enableFreeWinMovement = Config.Bind(mainSec, "Move in YAY floor", true, "If True, you can move during the win sequence (lol).");
			enableProportionalYTPAdder = Config.Bind(mainSec, "YTP animation speed", true, "If True, the animation you get when receiving ytps will be proportional to your gain (bigger the difference, faster it is).");
			enableGumVisualChange = Config.Bind(mainSec, "Gum Squished Visual", true, "If True, if Beans spits out the gum while squished, the gum will look squished too (similarly to how Cloud Copter does) and so, untouchable.");

			bullyItemInHandTendency = Config.Bind(mainSec, "Bully item in hand tendency factor", 25f,
				"Percentage (0% - 100%) that determines the tendency of Bully to pickup the item selected in the inventory instead of any other (100% means he\'ll always choose your item in hand; 0% means no tendency behavior).");
			bullyItemInHandTendency.Value = Mathf.Clamp(bullyItemInHandTendency.Value, 0f, 100f);

			enableNPCWeightBalance = Config.Bind(balanceSec, "NPC Balance", true,
				"If True, the weights of every npc will be balanced based on the amount of npcs possible, in a floor, that originates from X mod.");
			enableItemWeightBalance = Config.Bind(balanceSec, "Item Balance", true,
				"If True, the weights of every item will be balanced based on the amount of items possible, in a floor, that originates from X mod.");
			enableRandomEventWeightBalance = Config.Bind(balanceSec, "RandomEvent Balance", true,
				"If True, the weights of every event will be balanced based on the amount of random events possible, in a floor, that originates from X mod.");

			enableDebugLogs = Config.Bind("Debugging", "Debug Logs", false,
				"If True, some debug logs will appear in BepInEx Console.");

			ambienceDistancingFactor = Config.Bind(mainSec, "Ambience Distancing Factor", 1.25f,
				"The ambience will always appear close to you. This factor sets how long the ambience noises will be far from you (factor * game\'s tile unit [10]). Setting to something lower than 0.5 will let it work by how the game does it.");

			randomizedLockdownDoorSpeed = Config.Bind(mainSec, "Max Lockdown Door Speed", 1f,
				"Basically tells the maximum speed a lockdown door can reach randomly, so (1 to n). It can reach the maximum value of 50.");
			randomizedLockdownDoorSpeed.Value = Mathf.Clamp(randomizedLockdownDoorSpeed.Value, 1f, 50f);

			enableFieldTripCheatMode = Config.Bind(mainSec, "Enable field trip cheat", false, "If True, campfire frenzy will automatically launch the right item; In the picnic, it\'ll automatically take out the not-so-wished foods.");

			enableMarkersWithPathfinding = Config.Bind(mainSec, "Enable marker pathfind", true, "If True, every time you place a marker, it\'ll generate a path to make it easier to find the marked spot.");
			enableMarkerAutoDisable = Config.Bind(mainSec, "Enable marker auto-disable", true, "If True, every time you step on a marker, it\'ll be automatically removed.");

			enableNegativeUniqueness = Config.Bind(genSec, "Enable different negative seeds", true, "If True, negative seeds will generate differently from positive, making a new whole set of possibilities.");
			enableNonSquaryLayouts = Config.Bind(genSec, "Enable non-squary layouts", false, "If True, every room that has a square border by default won\'t have it anymore (includes modded rooms).");

			LoadingEvents.RegisterOnAssetsLoaded(Info, PostLoad(), true); // Post load because GeneratorManagement *might* be misused
		}
		public static void Log(string log)
		{
			if (enableDebugLogs.Value)
				plug.Logger.LogDebug(log);
		}
		public static void LogWarning(string log)
		{
			if (enableDebugLogs.Value)
				plug.Logger.LogWarning(log);
		}

		IEnumerator PostLoad()
		{
			yield return 3;
			yield return "Balancing weights...";


			foreach (var sco in Resources.FindObjectsOfTypeAll<SceneObject>())
			{
				var ld = sco.levelObject;
				if (!ld)
					continue;

				if (enableNPCWeightBalance.Value) // Npc Balance 
				{
					Dictionary<string, List<WeightedNPC>> npcPairs = [];
					for (int i = 0; i < sco.potentialNPCs.Count; i++)
					{
						string assemblyName = sco.potentialNPCs[i].selection.GetType().Assembly.FullName;
						if (npcPairs.TryGetValue(assemblyName, out var npcs))
							npcs.Add(sco.potentialNPCs[i]);
						else
							npcPairs.Add(assemblyName, [sco.potentialNPCs[i]]);
					}

					foreach (var weightedNpc in npcPairs)
					{
						int midVal = weightedNpc.Value.Count;
						weightedNpc.Value.ForEach(x => { x.weight = 100 + x.weight / midVal; /*Log($"NPC: For assembly \"{x.selection.name}\", we got the val of {x.weight}");*/ });
					}
				}

				if (enableItemWeightBalance.Value) // Item Balance 
				{
					Dictionary<string, List<WeightedItemObject>> npcPairs = [];
					for (int i = 0; i < ld.potentialItems.Length; i++)
					{
						string assemblyName = ld.potentialItems[i].selection.GetType().Assembly.FullName;
						if (npcPairs.TryGetValue(assemblyName, out var npcs))
							npcs.Add(ld.potentialItems[i]);
						else
							npcPairs.Add(assemblyName, [ld.potentialItems[i]]);
					}

					foreach (var weightedNpc in npcPairs)
					{
						int midVal = weightedNpc.Value.Count;
						weightedNpc.Value.ForEach(x => { x.weight = 100 + x.weight / midVal; /*Log($"ITEM: For assembly \"{x.selection.name}\", we got the val of {x.weight}");*/ });
					}

				}

				if (enableRandomEventWeightBalance.Value) // Item Balance 
				{
					Dictionary<string, List<WeightedRandomEvent>> npcPairs = [];
					for (int i = 0; i < ld.randomEvents.Count; i++)
					{
						string assemblyName = ld.randomEvents[i].selection.GetType().Assembly.FullName;
						if (npcPairs.TryGetValue(assemblyName, out var npcs))
							npcs.Add(ld.randomEvents[i]);
						else
							npcPairs.Add(assemblyName, [ld.randomEvents[i]]);
					}

					foreach (var weightedNpc in npcPairs)
					{
						int midVal = weightedNpc.Value.Count;
						weightedNpc.Value.ForEach(x => { x.weight = 100 + x.weight / midVal; /*Log($"EVENT: For assembly \"{x.selection.name}\", we got the val of {x.weight}");*/ });
					}

				}

			}

			yield return "Adding Renderer Container to everything...";
			if (enableAdditionalCulling.Value)
				Resources.FindObjectsOfTypeAll<Renderer>().Do(x => AdditionalCullingPatches.CreateRendererContainer(x.transform));

			yield return "Modifying every room asset to include no squary layouts...";
			if (enableNonSquaryLayouts.Value)
			{

				HashSet<IntVector2> touchedPositions = [];
				Queue<IntVector2> positionsToGo = [];
				foreach (var room in Resources.FindObjectsOfTypeAll<RoomAsset>())
				{
					if (room.secretCells.Count == 0) // Nothing to do here, move on
						continue;

					// Methods to be used in this system
					void getNeighborCells(IntVector2 position)
					{
						for (int i = 0; i < Directions.Count; i++)
						{
							var nextPos = position + ((Direction)i).ToIntVector2();
							if (!touchedPositions.Contains(nextPos) &&  // If it's not a cell that has been touched before
								!positionsToGo.Contains(nextPos) &&  // If it hasn't been registered yet
								!isPositionOutOfBounds(nextPos) &&  // Can't be out of bounds
								room.secretCells.Contains(nextPos)) // Needs to be a secret cell to make sense
								positionsToGo.Enqueue(nextPos);
						}
					}
					bool isPositionOutOfBounds(IntVector2 position) => !room.cells.Exists(x => x.pos == position);
					bool IsCellNextToOutOfBounds(IntVector2 position)
					{
						var cell = room.cells.Find(x => x.pos == position);

						if (cell.type != 0 && cell.type < 15)
							return false;

						for (int i = 0; i < Directions.Count; i++)
						{
							if (isPositionOutOfBounds(position + ((Direction)i).ToIntVector2())) // If position is out of bounds
								return true;
						}
						return false;
					}
					void removeCellFromRoom(IntVector2 position)
					{
						room.secretCells.Remove(position);
						room.cells.RemoveAll(x => x.pos == position);
						room.lockedCells.Remove(position);
					}

					void DFSForHiddenCellRemoval(IntVector2 pos)
					{
						positionsToGo.Clear();
						positionsToGo.Enqueue(pos);

						while (positionsToGo.Count != 0) // DFS
						{
							var position = positionsToGo.Dequeue();
							removeCellFromRoom(position);
							getNeighborCells(position);
							touchedPositions.Add(position);
						}
					}

					// Actual loop for getting the cells removed

					for (int i = 0; i < room.secretCells.Count; i++)
					{
						if (!touchedPositions.Contains(room.secretCells[i]) && IsCellNextToOutOfBounds(room.secretCells[i]))
						{
							DFSForHiddenCellRemoval(room.secretCells[i]);
							i = 0; // resets the loop basically to avoid missing out any cells
						}
					}

					touchedPositions.Clear(); // Resets the hashset
				}
			}
			


		}
		public const string mainSec = "Tweak Settings", balanceSec = "Balancing Settings", genSec = "Generator Settings", cheatSec = "Cheat Settings";
	}
}
