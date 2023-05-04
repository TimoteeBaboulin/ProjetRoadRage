using System;
using UnityEngine;

public class SnowPlow : MonoBehaviour{
	private void OnTriggerEnter(Collider other){
		if (!other.CompareTag("Obstacle")) return;

		
		ObstacleCar car = other.GetComponent<ObstacleCar>();
		
		car.Thrash(transform.position);
	}
}