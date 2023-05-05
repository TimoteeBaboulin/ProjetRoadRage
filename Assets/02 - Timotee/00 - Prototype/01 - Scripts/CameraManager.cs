using Managers;
using UnityEngine;

public class CameraManager : MonoBehaviour{
	private Vector3 _position;
	private Quaternion _rotation;

	private Transform _transform;

	private void Awake(){
		_transform = transform;

		_position = _transform.position;
		_rotation = _transform.rotation;
	}

	private void OnEnable(){
		GameManager.OnRestart += ResetCameraPosition;
	}

	private void OnDisable(){
		GameManager.OnRestart -= ResetCameraPosition;
	}

	public void ResetCameraPosition(){
		_transform.position = _position;
		_transform.rotation = _rotation;
	}
}