using Managers;
using UnityEngine;

namespace Obstacles{
	public class ObstacleCar : MonoBehaviour{
		[SerializeField] private float _bonusSpeed;

		[SerializeField] private float _minExplosionStrength;
		[SerializeField] private float _maxExplosionStrength;

		[SerializeField] private GameObject _modelParent;

		private Vector3 _baseLocalPosition;

		private Rigidbody _rigidbody;
		private float _speed;

		private void Awake(){
			_baseLocalPosition = transform.localPosition;
			_rigidbody = GetComponent<Rigidbody>();
		}

		private void Update(){
			if (GameManager.Paused || transform.position.z > 100) return;

			var position = transform.position;
			position.z -= _speed * Time.deltaTime;
			transform.position = position;
			if (position.z < CameraManager.Instance.transform.position.z)
				_modelParent.SetActive(false);
		}

		private void OnEnable(){
			transform.localPosition = _baseLocalPosition;
			transform.rotation = Quaternion.identity;
			_speed = TerrainManager.Instance.Speed * _bonusSpeed;
			_rigidbody.isKinematic = true;
			gameObject.tag = "Obstacle";
			_modelParent.SetActive(true);
		}

		public void Thrash(Vector3 explosionOrigin){
			gameObject.tag = "Untagged";
			float side;
			var xDifference = explosionOrigin.x - transform.position.x;
			if (Mathf.Abs(xDifference) < 0.1f)
				side = Random.Range(0f, 1f) > 0.5f ? 1 : -1;
			else
				side = xDifference > 0 ? -1 : 1;

			float x, y, z, terrainSpeed;
			terrainSpeed = 1 + TerrainManager.SpeedIncrease;
			x = Random.Range(_minExplosionStrength, _maxExplosionStrength) * terrainSpeed * side;
			y = Random.Range(_minExplosionStrength, _maxExplosionStrength) / 2 * terrainSpeed;
			z = TerrainManager.Instance.Speed + Random.Range(_minExplosionStrength, _maxExplosionStrength);
			_rigidbody.isKinematic = false;
			_rigidbody.AddForce(x, y, z);
		}
	}
}