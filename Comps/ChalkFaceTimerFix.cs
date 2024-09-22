using UnityEngine;

namespace TweaksPlus.Comps
{
	internal class ChalkFaceTimerFix : MonoBehaviour
	{
		void FixedUpdate()
		{
			if (!changeTimer) return;

			chalkles.audMan.audioDevice.time = chalkles.audMan.audioDevice.clip.length * (chalkles.charge / chalkles.setTime);
		}

		public void SetTimerFix(bool timer) => changeTimer = timer;

		bool changeTimer = false;

		[SerializeField]
		public ChalkFace chalkles;
	}
}
