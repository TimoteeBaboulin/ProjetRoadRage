using UnityEngine;

namespace PowerUps{
	public class StaticPickUpArray : MonoBehaviour{
		public static StaticPickUpArray Instance;

		[SerializeField] private PickUp[] _pickUps;

		private void Awake(){
			if (Instance==null){
				Instance = this;
				return;
			}

			Destroy(gameObject);
		}

		public static PickUp GeneratePickUp(){
			return Instance.GeneratePickUpLocal();
		}

		private PickUp GeneratePickUpLocal(){
			return _pickUps[Random.Range(0, _pickUps.Length)];
		}
	}
}