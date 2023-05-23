using System;
using Managers;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Obstacles{
	public class ObstacleCar : MonoBehaviour{
		[SerializeField] private float _bonusSpeed;

		[SerializeField] private float _minExplosionStrength;
		[SerializeField] private float _maxExplosionStrength;

		[SerializeField] private GameObject _modelParent;

		[SerializeField] private bool _haveFixedModel;

		[SerializeField] private float _pointsWorth;

		private Vector3 _baseLocalPosition;

		private Rigidbody _rigidbody;
		private float _speed;

		private void Awake(){
			_baseLocalPosition = transform.localPosition;
			_rigidbody = GetComponent<Rigidbody>();
		}

		private void Update(){
			if (!_modelParent.activeSelf || GameManager.Paused || transform.position.z > 100) return;

			var position = transform.position;
			position.z -= _speed * Time.deltaTime;
			transform.position = position;
			if (position.z < CameraManager.Instance.transform.position.z)
				_modelParent.SetActive(false);
		}

		private void OnEnable(){
			//Reset values in case it got thrashed
			transform.localPosition = _baseLocalPosition;
			transform.rotation = Quaternion.identity;
			_rigidbody.isKinematic = true;
			gameObject.tag = "Obstacle";
			
			//Set speed to the current value
			_speed = TerrainManager.Instance.Speed * _bonusSpeed;
			
			//If it's a normie car, generate a new one
			if (!_haveFixedModel) GenerateCar();
			
			//Make sure you can see the car
			_modelParent.SetActive(true);
		}

		public void Thrash(Vector3 explosionOrigin){
			gameObject.tag = "Untagged";
			float side = Random.Range(0f, 1f) > 0.5f ? 1 : -1;
			
			float x, y, z, terrainSpeed;
			terrainSpeed = 1 + TerrainManager.SpeedIncrease;
			x = Random.Range(_minExplosionStrength, _maxExplosionStrength) * side;
			y = Random.Range(_minExplosionStrength, _maxExplosionStrength) * terrainSpeed;
			z = TerrainManager.Instance.Speed + Random.Range(_minExplosionStrength, _maxExplosionStrength) / 2;
			_rigidbody.isKinematic = false;
			_rigidbody.AddForce(x, y, z);
			
			ScoreManager.AddScore(_pointsWorth);
			ScoreManager.Crashes++;
		}

		private void GenerateCar(){
			var parts = StaticCarArray.GenerateParts();

			while(_modelParent.transform.childCount > 0){
				Transform child = _modelParent.transform.GetChild(0);
				child.SetParent(StaticCarArray.GameObject.transform);
				child.gameObject.SetActive(false);
			}

			foreach(var part in parts){
				part.transform.parent = _modelParent.transform;
				part.transform.localPosition = Vector3.zero;
			}
		}
	}
}