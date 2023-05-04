using UnityEngine;

public class StaticPickUpArray : MonoBehaviour{
	public static StaticPickUpArray Instance;

	[SerializeField] private PickUp[] _pickUps;
	[SerializeField] [Range(0, 1)] private float _pickUpChance;
	
	public static PickUp GeneratePickUp(){
		return Instance.GeneratePickUpLocal();
	}

	private void Awake(){
		if (Instance==null){
			Instance = this;
			return;
		}
		
		Destroy(gameObject);
	}

	private PickUp GeneratePickUpLocal(){
		if (Random.Range(0f, 1) > _pickUpChance) return null;
		return _pickUps[Random.Range(0, _pickUps.Length)];
	}
}