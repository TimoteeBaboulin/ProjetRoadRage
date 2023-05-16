using UnityEngine;

namespace Managers{
	public class PoliceManager : MonoBehaviour{
		public static PoliceManager Instance;
		[SerializeField] private float _dangerTime;
		[SerializeField] private AnimationCurve _dangerCurve;

		public static float DangerTime => Instance._dangerTime;

		public static AnimationCurve DangerCurve => Instance._dangerCurve;

		private void Awake(){
			if (Instance!=null) return;
			Instance = this;
		}
	}
}