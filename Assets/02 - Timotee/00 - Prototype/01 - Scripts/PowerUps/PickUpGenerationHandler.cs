using UnityEngine;

namespace PowerUps{
	public class PickUpGenerationHandler : MonoBehaviour{
		private static int _droughtCounter;
		private static int _pityDepth = 5;

		[SerializeField] private PickUpSpot[] _pickUpSpots;
		[SerializeField] [Range(0, 1)] private float _spawnChance;
		
		private float Threshold => (((1 - _spawnChance) / Mathf.Pow(_pityDepth, 2)) * Mathf.Pow(_droughtCounter, 2)) + _spawnChance;
		
		private GameObject _activeSpot;

		private void OnEnable(){
			if (Random.Range(0f,1) > Threshold){
				_droughtCounter++;
				return;
			}

			_droughtCounter = 0;
			_activeSpot = _pickUpSpots[Random.Range(0,_pickUpSpots.Length)].gameObject;
			_activeSpot.SetActive(true);
		}

		private void OnDisable(){
			_activeSpot?.SetActive(false);
		}
	}
}