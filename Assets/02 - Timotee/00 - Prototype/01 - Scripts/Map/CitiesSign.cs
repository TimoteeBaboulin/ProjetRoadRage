using UnityEngine;

namespace Map{
	public class CitiesSign : MonoBehaviour{
		private static int _count;

		[SerializeField] private GameObject _sign;
		
		private void OnEnable(){
			_count++;
			if (_count < 10) return;
			
			_sign.SetActive(true);
			_count = 0;
		}

		private void OnDisable(){
			_sign.SetActive(false);
		}
	}
}