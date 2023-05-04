using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleCar : MonoBehaviour{
	[SerializeField] private float _bonusSpeed;

	[SerializeField] private float _minExplosionStrength;
	[SerializeField] private float _maxExplosionStrength;

	private Vector3 _baseLocalPosition;
	private float _speed;

	private Rigidbody _rigidbody;

	public void Thrash(Vector3 explosionOrigin){
		gameObject.tag = "Untagged";
		float side;
		float xDifference = explosionOrigin.x - transform.position.x;
		if (Mathf.Abs(xDifference) < 0.1f){
			side = Random.Range(0f, 1f) > 0.5f ? 1 : -1;
		} else{
			side = xDifference > 0 ? -1 : 1;
		}

		float x, y, z, terrainSpeed;
		terrainSpeed = 1 + TerrainManager.SpeedIncrease;
		x = Random.Range(_minExplosionStrength, _maxExplosionStrength) * terrainSpeed * side;
		y = Random.Range(_minExplosionStrength, _maxExplosionStrength) * terrainSpeed;
		z = Random.Range(_minExplosionStrength, _maxExplosionStrength) * terrainSpeed;
		_rigidbody.isKinematic = false;
		_rigidbody.AddForce(x, y, z);
	}
	
	private void Awake(){
		_baseLocalPosition = transform.localPosition;
		_rigidbody = GetComponent<Rigidbody>();
	}

	private void OnEnable(){
		transform.localPosition = _baseLocalPosition;
		transform.rotation = Quaternion.identity;
		_speed = TerrainManager.Instance.Speed * _bonusSpeed;
		_rigidbody.isKinematic = true;
		gameObject.tag = "Obstacle";
	}

	private void Update(){
		if (GameManager.Paused || transform.position.z > 100) return;
		
		Vector3 position = transform.position;
		position.z -= _speed * Time.deltaTime;
		transform.position = position;
	}
}