using System.Collections;
using Managers;
using Obstacles;
using UnityEngine;

namespace PowerUps{
	public class SnowPlow : MonoBehaviour{
		private void OnTriggerEnter(Collider other){
			if (!other.CompareTag("Obstacle")) return;

			var car = other.GetComponent<ObstacleCar>();

			car ??= other.GetComponentInParent<ObstacleCar>();

			car.Thrash(transform.position);
			CameraManager.ShakeCamera(0.1f);
		}

		private void OnEnable(){
			StartCoroutine(SpawnCoroutine());
		}

		private IEnumerator SpawnCoroutine(){
			float timer = 0;

			while(timer < .3f){
				transform.localScale = Vector3.one * (timer/.3f);
				yield return null;

				timer += Time.deltaTime;
			}
			
			transform.localScale = Vector3.one;
		}
	}
}