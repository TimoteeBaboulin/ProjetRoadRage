using System;
using Managers;
using Unity.Mathematics;
using UnityEngine;

namespace UI{
	public class UISpeedometer : MonoBehaviour{
		[SerializeField] private GameObject _anchor;
		[SerializeField] private float _minimumSpeed;
		[SerializeField] private float _maximumSpeed;
		[SerializeField] private float _zeroSpeedRotation;

		private float _speed;

		private void OnEnable(){
			GameManager.OnGameLost += GameLost;
		}

		private void OnDisable(){
			GameManager.OnGameLost -= GameLost;
		}

		private void Update(){
			if (!GameManager.GameRunning || _speed >= 1) return;
			Debug.Log("Game Running");
			_speed = TerrainManager.SpeedIncrease;
			Vector3 rotation = new Vector3(0, 0, Mathf.Lerp(_minimumSpeed, _maximumSpeed, _speed));
			_anchor.transform.rotation = Quaternion.Euler(rotation);
		}

		private void GameLost(){
			Debug.Log("Lost Game");
			_speed = 0;
			_anchor.transform.rotation = Quaternion.Euler(0,0,_zeroSpeedRotation);
		}
	}
}