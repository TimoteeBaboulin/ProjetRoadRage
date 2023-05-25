using UnityEngine;

namespace PowerUps{
	public class PickUpSpot : MonoBehaviour{
		private GameObject _currentPickUp;
		[SerializeField] private ParticleSystem _particles;

		private void OnEnable(){
			var pickUp = StaticPickUpArray.GeneratePickUp();
			if (pickUp==null) return;

			_particles.Play();
			_currentPickUp = Instantiate(pickUp.gameObject, transform);
		}

		private void OnDisable(){
			_particles.Clear();
			if (_currentPickUp==null) return;

			Destroy(_currentPickUp);
			_currentPickUp = null;
		}
	}
}