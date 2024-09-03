using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace TweaksPlus
{
    [BepInPlugin("pixelguy.pixelmodding.baldiplus.tweaksplus", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
		internal static ConfigEntry<bool> enableItemAcceptorCheck, enableAutoMapFillCheck;
        private void Awake()
        {
			Harmony h = new("pixelguy.pixelmodding.baldiplus.tweaksplus");
			h.PatchAll();

			enableItemAcceptorCheck = Config.Bind(mainSec, "Enable Item Acceptors display", true, "If True, every time you look at an IITemAcceptor object (such as a Coin Door) with the required item, the interaction display will popup in the screen");
			enableAutoMapFillCheck = Config.Bind(mainSec, "Enable map forced fillup", true, "If True, for any room that you enter, all the map tiles will be forcefully revealed, regardless of their shape.");
        }

		const string mainSec = "Tweak Settings";
    }
}
