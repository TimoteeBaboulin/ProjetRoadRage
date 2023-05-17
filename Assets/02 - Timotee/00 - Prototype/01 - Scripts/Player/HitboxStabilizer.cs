using UnityEngine;

namespace Player{
	public class HitboxStabilizer : MonoBehaviour{
		[SerializeField] private Transform _base;

		private void Update(){
			transform.position = _base.position;
		}
	}
}