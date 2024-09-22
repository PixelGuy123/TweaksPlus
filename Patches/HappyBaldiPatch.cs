using HarmonyLib;

namespace TweaksPlus.Patches
{
	[HarmonyPatch(typeof(HappyBaldi), "SpawnWait", MethodType.Enumerator)]
    class HappyBaldiPatch
    {
		static System.Exception Finalizer(System.Exception __exception) =>
			Plugin.enableHappyBaldiFix.Value ? null : __exception;
    }
}
