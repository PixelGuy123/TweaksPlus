using UnityEngine;

namespace TweaksPlus.Comps
{
	internal class ChalkFaceTimerFix : MonoBehaviour
	{
		public void SetTimerFix(bool timer)
		{
			changeTimer = timer;
			maxClipLength = chalkles.audSpawn.subDuration;

			if (changeTimer && maxClipLength > 0f && chalkles.setTime > 0f)
			{
				// pitch = (audio length) / (charge duration)
				float targetPitch = maxClipLength / chalkles.setTime;
				chalkles.audMan.pitchModifier = Mathf.Clamp(targetPitch, 0.1f, 2f);
			}
			else
			{
				chalkles.audMan.pitchModifier = 1f; // Reset pitch if not syncing
			}
		}

		bool changeTimer = false;
		float maxClipLength = -1f;
		public ChalkFace chalkles;
	}
}
