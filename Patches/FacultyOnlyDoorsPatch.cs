using HarmonyLib;
using MTM101BaldAPI.Registers;
using UnityEngine;

namespace TweaksPlus.Patches;

[HarmonyPatch(typeof(FacultyOnlyDoor))]
static class FacultyOnlyDoorsPatch
{
    [HarmonyPatch("OnTriggerStay"), HarmonyPostfix]
    static void DetectNPCStudentsToo(FacultyOnlyDoor __instance, Collider other)
    {
        if (!Plugin.enableFacultyDoorDetectNPCStudents.Value)
            return;


        if (other.CompareTag("NPC"))
        {
            var npc = other.GetComponent<NPC>();

            if (!npc.Navigator.isActiveAndEnabled)
                return; // It means the NPC doesn't move by default (like Bully)

            var meta = npc.GetMeta();
            if (meta != null && meta.tags.Contains("student"))
            {
                Vector3 vector = __instance.cam1.transform.position;
                if (Vector3.Distance(__instance.cam1.transform.position, other.transform.position) > Vector3.Distance(__instance.cam2.transform.position, other.transform.position))
                {
                    vector = __instance.cam2.transform.position;
                }
                if (PhysicsManager.RaycastEntity(npc.Navigator.Entity, vector, __instance.layerMask, QueryTriggerInteraction.Ignore, 1000f, true))
                {
                    if (!__instance.alarmSounded)
                    {
                        __instance.alarmSounded = true;
                        __instance.audioManager.PlaySingle(__instance.audAlarm);
                    }
                    __instance.playerDetected = true;
                }
            }
        }
    }

}