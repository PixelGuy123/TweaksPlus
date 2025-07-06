using UnityEngine;

namespace TweaksPlus.Comps
{
	internal class ChalkFaceTimerFix : MonoBehaviour
	{
		void Awake()
		{
			if (!chalkles) return;
			maxClipLength = chalkles.audSpawn.subDuration;
		}
		void FixedUpdate()
		{
			if (!changeTimer || maxClipLength == -1f) return;

			float targetTime = maxClipLength * (chalkles.charge / chalkles.setTime);
			float currentTime = chalkles.audMan.audioDevice.time;

			if (Mathf.Abs(currentTime - targetTime) > float.Epsilon) // Avoids flickering by adding a *small* time gap
			{
				chalkles.audMan.audioDevice.time = targetTime;
			}
		}

		public void SetTimerFix(bool timer) => changeTimer = timer;

		bool changeTimer = false;
		float maxClipLength = -1f;
		public ChalkFace chalkles;
	}
}
