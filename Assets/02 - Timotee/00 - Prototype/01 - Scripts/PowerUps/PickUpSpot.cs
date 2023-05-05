using UnityEngine;

namespace PowerUps{
	public class PickUpSpot : MonoBehaviour{
		private GameObject _currentPickUp;

		private void OnEnable(){
			var pickUp = StaticPickUpArray.GeneratePickUp();
			if (pickUp==null) return;

			_currentPickUp = Instantiate(pickUp.gameObject, transform);
		}

		private void OnDisable(){
			if (_currentPickUp==null) return;

			Destroy(_currentPickUp);
			_currentPickUp = null;
		}
	}
}