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

		[SerializeField] private AudioSource _crashAudio;
		
		private Vector3 _baseLocalPosition;

		
		[SerializeField] private CarPieceHandler[] _fronts;
		[SerializeField] private CarPieceHandler[] _backs;
		private CarPieceHandler _activeFront;
		private CarPieceHandler _activeBack;

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

		private void OnDisable(){
			_activeFront?.gameObject.SetActive(false);
			_activeBack?.gameObject.SetActive(false);
		}

		public void Thrash(Vector3 explosionOrigin){
			gameObject.tag = "Untagged";
			float side = Random.Range(0f, 1f) > 0.5f ? 1 : -1;
			
			float x, y, z, terrainSpeed;
			terrainSpeed = 1 + TerrainManager.SpeedIncrease;
			x = Random.Range(_minExplosionStrength, _maxExplosionStrength) * side;
			y = Random.Range(_minExplosionStrength, _maxExplosionStrength) * terrainSpeed;
			z = (TerrainManager.Instance.Speed / 4) + (Random.Range(_minExplosionStrength, _maxExplosionStrength));
			_rigidbody.isKinematic = false;
			_rigidbody.AddForce(x, y, z);
			_rigidbody.AddTorque(x, y, z);
			
			ScoreManager.AddScore(_pointsWorth);
			ScoreManager.Crashes++;
			_crashAudio.Play();
		}

		private void GenerateCar(){
			Color color = StaticCarArray.GenerateColor();
			
			_activeFront = _fronts[Random.Range(0, _fronts.Length)];
			_activeBack = _backs[Random.Range(0, _backs.Length)];
			
			_activeFront.SetColor(color);
			_activeBack.SetColor(color);
			
			_activeBack.gameObject.SetActive(true);
			_activeFront.gameObject.SetActive(true);

			// var parts = StaticCarArray.GenerateParts(out var color);
			//
			// while(_modelParent.transform.childCount > 0){
			// 	Transform child = _modelParent.transform.GetChild(0);
			// 	child.SetParent(StaticCarArray.GameObject.transform);
			// 	child.gameObject.SetActive(false);
			// }
			//
			// foreach(var part in parts){
			// 	Transform partTransform = part.transform;
			// 	partTransform.parent = _modelParent.transform;
			// 	partTransform.localPosition = Vector3.zero;
			// 	part.SetColor(color);
			// }
		}
	}
}