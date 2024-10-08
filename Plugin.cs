﻿using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace TweaksPlus
{
    [BepInPlugin("pixelguy.pixelmodding.baldiplus.tweaksplus", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
		internal static ConfigEntry<bool> enableAutoMapFillCheck, enableChalklesInstaDisable, enableChalklesProportionalSpawn,
			enableItemUsageInPitstop, enableItemDescRevealInStorageLocker, enableMrsPompDynamicTimer, enableNavigatorTargettingImprovement,
			enableHappyBaldiFix, enableNegativeUniqueness, enablePlaytimeBullying, enablePrincipalNPCLecture, enableBullyGettingDetention,
			enableNPCActualDetention, enableRuleFreeZoneForNPCs, enableNullMapTileFix, enableBeansBullying;

		internal static ConfigEntry<float> mrsPompTimerFactor, chalklesSizeFactor;
        private void Awake()
        {
			Harmony h = new("pixelguy.pixelmodding.baldiplus.tweaksplus");
			h.PatchAll();

			enableAutoMapFillCheck = Config.Bind(mainSec, "Enable map forced fillup", true, "If True, for any room that you enter, all the map tiles will be forcefully revealed, regardless of their shape.");
			enableChalklesInstaDisable = Config.Bind(mainSec, "Enable Chalkles instant disable", true, "If True, the moment you leave a room with Chalkles will instantly reset him (like it used to be before v0.4).");
			enableChalklesProportionalSpawn = Config.Bind(mainSec, "Enable Chalkles proportional spawn", true, "If True, Chalkles\' spawn time will be proportional to the estimated size of the room it is currently in.");
			enableItemUsageInPitstop = Config.Bind(mainSec, "Enable item usage in the Pitstop", true, "If True, the player will be able of using any item from their inventory in the shop (feature implemented since 0.7)");
			enableItemDescRevealInStorageLocker = Config.Bind(mainSec, "Enable item description in storage locker", true, "If True, the item description will be displayed in a Storage Locker.");
			enableMrsPompDynamicTimer = Config.Bind(mainSec, "Enable Mrs Pomp dynamic timer", true, "If True, Mrs Pomp\'s timer will be proportional to how far away you are from the classroom.");
			enableNavigatorTargettingImprovement = Config.Bind(mainSec, "Enable Navigator improvement", true, "If True, the npc navigators will always avoid non-safe entity cells.");
			enableHappyBaldiFix = Config.Bind(mainSec, "Enable Happy Baldi fix", true, "If True, Happy Baldi no longer crashes if Baldi is missing.");
			enableNegativeUniqueness = Config.Bind(mainSec, "Enable different negative seeds", true, "If True, negative seeds will generate differently from positive, making a whole set of possibilities.");
			enablePlaytimeBullying = Config.Bind(mainSec, "Enable Playtime bullying", true, "If True, cutting Playtime\'s rope will count as bullying for five seconds.");
			enablePrincipalNPCLecture = Config.Bind(mainSec, "Enable Principal lecture on NPCs", true, "If True, Principal will always give lecture to NPCs after giving them *actual* detention.");
			enableBullyGettingDetention = Config.Bind(mainSec, "Enable Bully detention", true, "If True, Bully will have actual detention when caught.");
			enableNPCActualDetention = Config.Bind(mainSec, "Enable NPC actual detention", true, "If True, NPCs will always be frozen when sent to detention, for five seconds.");
			enableRuleFreeZoneForNPCs = Config.Bind(mainSec, "Enable rule free zone for npcs", true, "If True, NPCs will have their guilt cleared in Rule Free zones (like Playground).");
			mrsPompTimerFactor = Config.Bind(mainSec, "Mrs Pomp distance factor", 3.6f, "Determines how long will be the timer by the distance multiplied by this constant/factor.");
			chalklesSizeFactor = Config.Bind(mainSec, "Chalkles charge factor", 1.65f, "Determines how long will take for Chalkles to charge by getting the magnitude of the size of the room multiplied by this constant/factor.");
			enableNullMapTileFix = Config.Bind(mainSec, "Null map tile fix", true, "If True, Mrs Pomp\'s icon will no longer be invisible if the center of the room is an empty tile.");
			enableBeansBullying = Config.Bind(mainSec, "Beans bullying", true, "If True, Beans will be breaking the bullying rule for spitting gums and hitting them at somebody.");
		}

		const string mainSec = "Tweak Settings";
    }
}
