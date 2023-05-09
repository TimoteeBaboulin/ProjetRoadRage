using Obstacles;
using UnityEngine;

namespace PowerUps{
	public class SnowPlow : MonoBehaviour{
		private void OnTriggerEnter(Collider other){
			if (!other.CompareTag("Obstacle")) return;
			
			var car = other.GetComponent<ObstacleCar>();

			car.Thrash(transform.position);
		}
	}
}