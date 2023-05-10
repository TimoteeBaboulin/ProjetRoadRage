using UnityEngine;

namespace Managers{
	public class PoliceManager : MonoBehaviour{
		public static PoliceManager Instance;

		public static float DangerTime => Instance._dangerTime;
		[SerializeField] private float _dangerTime;

		public static AnimationCurve DangerCurve => Instance._dangerCurve;
		[SerializeField] private AnimationCurve _dangerCurve;
		
		private void Awake(){
			if (Instance != null) return;
			Instance = this;
		}
	}
}