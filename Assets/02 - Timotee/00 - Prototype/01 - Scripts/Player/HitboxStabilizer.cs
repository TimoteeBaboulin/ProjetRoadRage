using UnityEngine;

namespace Player{
	public class HitboxStabilizer : MonoBehaviour{
		private Quaternion _rotation;

		private void Awake(){
			_rotation = transform.rotation;
		}

		private void Update(){
			transform.rotation = _rotation;
		}
	}
}