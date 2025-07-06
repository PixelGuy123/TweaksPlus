using UnityEngine;

namespace TweaksPlus.Comps
{
	internal class ChalkFaceTimerFix : MonoBehaviour
	{
		void FixedUpdate()
		{
			if (!changeTimer) return;

			float targetTime = chalkles.audMan.audioDevice.clip.length * (chalkles.charge / chalkles.setTime);
			float currentTime = chalkles.audMan.audioDevice.time;

			if (Mathf.Abs(currentTime - targetTime) > float.Epsilon) // Avoids flickering by adding a *small* time gap
			{
				chalkles.audMan.audioDevice.time = targetTime;
			}
		}

		public void SetTimerFix(bool timer) => changeTimer = timer;

		bool changeTimer = false;

		[SerializeField]
		public ChalkFace chalkles;
	}
}
