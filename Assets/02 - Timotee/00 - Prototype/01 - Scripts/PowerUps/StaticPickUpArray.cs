using UnityEngine;

namespace PowerUps{
	public class StaticPickUpArray : MonoBehaviour{
		public static StaticPickUpArray Instance;

		[SerializeField] private PickUp[] _pickUps;
		[SerializeField] [Range(0, 1)] private float _pickUpChance;

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
			if (Random.Range(0f, 1) > _pickUpChance) return null;
			return _pickUps[Random.Range(0, _pickUps.Length)];
		}
	}
}