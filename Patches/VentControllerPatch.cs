using HarmonyLib;
using UnityEngine;
using System.Reflection.Emit;
using TweaksPlus.Comps;
using System.Collections.Generic;

namespace TweaksPlus.Patches
{
	[ConditionalPatchConfigWithDesc(Plugin.mainSec, "Enable dynamic vent speed", "If True, aside from the default vent speed, it's also added by the velocity of the entity that's getting inside the vent.", true)]
	[HarmonyPatch(typeof(VentController))]
	internal static class VentControllerPatch
	{
		[HarmonyPatch("OnTriggerEnter")]
		[HarmonyTranspiler]

		static IEnumerable<CodeInstruction> MakeADifferentTravelStatus(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(true,
				new(OpCodes.Ldarg_1),
				new(OpCodes.Ldstr, "Player"),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(Component), "CompareTag", [typeof(string)])),
				new(OpCodes.Newobj, AccessTools.Constructor(typeof(VentTravelStatus), [typeof(EntityOverrider), typeof(Vector3), typeof(float), typeof(bool)]))
				)
			.Set(OpCodes.Newobj, AccessTools.Constructor(typeof(SpeedTravelVentStatus), [typeof(EntityOverrider), typeof(Vector3), typeof(float), typeof(bool), typeof(float)]))
			.InsertAndAdvance(
				new(OpCodes.Ldloc_0), // Not only adds a new constructor, gets the speed from the entity too
				Transpilers.EmitDelegate((Entity e) =>
				{
					float mag = e.Velocity.magnitude;
					return !float.IsNaN(mag) ? Mathf.Pow(mag, 7.5f) : 0f; // Make sure it's not NAN since the player can do that (not sure for PlayerEntity tho..., but just in case);
				})
				)
			.MatchForward(false,
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(VentController), "entryGrate")),
				new(OpCodes.Ldstr, "VentGrateOpen")
				)
			.InsertAndAdvance(
				new(OpCodes.Ldloc_1),
				Transpilers.EmitDelegate((EntityOverrider overrider) =>
				{
					if (overrider.entity.CompareTag("NPC"))
						overrider.entity.GetComponent<NPC>().navigationStateMachine.DestinationEmpty();

					overrider.entity.gameObject.layer = overrider.entity.defaultLayer;
				})
				)
			.InstructionEnumeration();

		[HarmonyPatch("Update")]
		[HarmonyPrefix]
		static void GetSpeedValueBeforeChange(float ___speed, out float __state) =>
			__state = ___speed;
		[HarmonyPatch("Update")]
		[HarmonyPostfix]
		static void SetSpeedValueBackAfterChange(ref float ___speed, float __state) =>
			___speed = __state;
		

		[HarmonyPatch("Update")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> ChangeSpeedDynamically(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(true,
				new CodeMatch(OpCodes.Stloc_0) // There's only one instruction at the beginning, so lol
				)
			.Advance(1)
			.InsertAndAdvance(
				new(OpCodes.Ldarg_0),
				new(OpCodes.Dup), // This instruction is useful, it'll duplicate the instance of VentController reference to the top of the stack, which means
								  // the Stfld instruction below will work fine since it'll have a instance of this class stored in the stack. How genius!!
				new(CodeInstruction.LoadField(typeof(VentController), "speed")), // gets the current speed value available
				new(OpCodes.Ldloc_0),
				Transpilers.EmitDelegate((VentTravelStatus status) => status is SpeedTravelVentStatus speedStatus ? speedStatus.speed : 0f), // Gets the speed from the status
				new(OpCodes.Add), // Adds both values
				new(CodeInstruction.StoreField(typeof(VentController), "speed")) // Set the value reference back to speed itself
				)

			.MatchForward(false, 
				new(OpCodes.Ldloc_0),
				new(OpCodes.Ldc_I4_1),
				new(CodeInstruction.StoreField(typeof(VentTravelStatus), "state"))
				)
			.InsertAndAdvance(
				
				new(OpCodes.Ldloc_0),
				CodeInstruction.LoadField(typeof(VentTravelStatus), "overrider"), // Gets the overrider from the travel status
				new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(EntityOverrider), "entity")), // gets entity
				new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Component), "gameObject")), // get the object from entity

				new(OpCodes.Ldc_I4_S, 19), // first argument given

				new(OpCodes.Call, AccessTools.PropertySetter(typeof(GameObject), "layer")) // sets the layer to the first argument given

				//new(Transpilers.EmitDelegate((VentTravelStatus status) => status.overrider.entity.gameObject.layer = 19)) // Change layer again lol
				)
			.InstructionEnumeration();
	}



	internal class SpeedTravelVentStatus(EntityOverrider overrider, Vector3 start, float targetProgress, bool camera, float targetSpeed) : VentTravelStatus(overrider, start, targetProgress, camera)
	{
		public float speed = targetSpeed;
	}
}
