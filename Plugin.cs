using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace TweaksPlus
{
    [BepInPlugin("pixelguy.pixelmodding.baldiplus.tweaksplus", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
		internal static ConfigEntry<bool> enableAutoMapFillCheck, enableChalklesInstaDisable, enableChalklesProportionalSpawn,
			enableItemUsageInPitstop, enableItemDescRevealInStorageLocker, enableMrsPompDynamicTimer, enableNavigatorTargettingImprovement,
			enableHappyBaldiFix, enableNegativeUniqueness, enablePlaytimeBullying;
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
		}

		const string mainSec = "Tweak Settings";
    }
}
