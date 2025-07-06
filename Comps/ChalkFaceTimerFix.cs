using UnityEngine;

namespace TweaksPlus.Comps
{
	internal class ChalkFaceTimerFix : MonoBehaviour
	{
		void FixedUpdate()
		{
			if (!changeTimer || maxClipLength == -1f) return;

			float targetTime = maxClipLength * (chalkles.charge / chalkles.setTime);
			float currentTime = chalkles.audMan.audioDevice.time;

			if (Mathf.Abs(currentTime - targetTime) > smallInterval) // Prevents flickering by adding a *small* time gap
			{
				chalkles.audMan.audioDevice.time = targetTime;
			}
		}

		public void SetTimerFix(bool timer)
		{
			changeTimer = timer;
			maxClipLength = chalkles.setTime;
		}

		bool changeTimer = false;
		float maxClipLength = -1f;
		[SerializeField]
		float smallInterval = 0.05f;
		public ChalkFace chalkles;
	}
}
