using UnityEngine;

public class PowerUpHitbox : MonoBehaviour{
	[SerializeField] private PlayerCar _car;

	private void OnTriggerEnter(Collider other){
		if (!other.CompareTag("PickUp")) return;

		other.GetComponent<PickUp>().Activate(_car);
	}
}